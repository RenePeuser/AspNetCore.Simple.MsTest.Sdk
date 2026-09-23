using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    ///     The one place every static assert entry - http and object route alike - resolves its services
    ///     from. There is no second container and no hand-wired copy anywhere else.
    /// </summary>
    public static partial class HttpClientAssertExtensions
    {
        private static readonly Lock ServiceProviderGate = new();

        private static IServiceProvider? _serviceProvider;

        private static bool _serviceProviderIsDefault;

        /// <summary>
        ///     Hands the host's provider to all static assert extensions.
        ///     Call this method once during test initialization (e.g., in [AssemblyInitialize])
        ///     after registering services via services.AddAssertableHttpClient().
        ///     The provider has to stay alive for the whole test run - it is resolved from on every assert.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered services</param>
        public static void Setup(IServiceProvider serviceProvider)
        {
            lock (ServiceProviderGate)
            {
                _serviceProvider = serviceProvider;
                _serviceProviderIsDefault = false;
                _jsonSerializerOptions = serviceProvider.GetRequiredService<JsonSerializerOptions>();
                _customAssertableHttpClient = null;
            }
        }

        /// <summary>
        ///     Resolves a service for an assert issued by <paramref name="consumerAssembly" />.
        /// </summary>
        internal static T GetService<T>(Assembly consumerAssembly)
            where T : notnull
        {
            var serviceProvider = Volatile.Read(ref _serviceProvider) ?? EnsureDefaultServiceProvider(consumerAssembly);

            return serviceProvider.GetRequiredService<T>();
        }

        private static IServiceProvider EnsureDefaultServiceProvider(Assembly consumerAssembly)
        {
            lock (ServiceProviderGate)
            {
                if (_serviceProvider.IsNull())
                {
                    _serviceProvider = CreateDefaultServiceProvider(consumerAssembly);
                    _serviceProviderIsDefault = true;
                }

                return _serviceProvider;
            }
        }

        /// <summary>
        ///     Until a test hands over its host's provider - and for pure object asserts, which never do -
        ///     the extensions run on the sdk's own container, built from the very same registrations.
        ///     It is bound to the first consumer assembly asking: every test assembly runs in its own
        ///     test host process, so that one decides plain vs ANSI output for the whole run.
        ///     Without a host there is no endpoint registry - the first endpoint validation then reports
        ///     the missing Setup instead of failing cryptically.
        /// </summary>
        private static ServiceProvider CreateDefaultServiceProvider(Assembly consumerAssembly)
        {
            // Without a host the environment is the only configuration source - TestSdkSettings__OutputMode
            // and friends bind exactly as they do in a host.
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration, consumerAssembly);

            // No host registers the api's options here - they are handed in through the property.
            services.Replace(ServiceDescriptor.Singleton(_jsonSerializerOptions));
            services.Replace(ServiceDescriptor.Singleton<IEndpointProvider, EmptyEndpointProvider>());

            return services.BuildServiceProvider();
        }
    }

    public static partial class HttpClientAssertExtensions
    {
        // Quickfix to hold the whole api compatible
        private const string IgnoreResponseComparison = "IgnoreResponse";

        private static JsonSerializerOptions _jsonSerializerOptions = JsonSerializerExtension.CreateDefaultOptions();

        private static IAssertableHttpClient? _customAssertableHttpClient;

        public static bool SkipEndpointValidation { get; set; }

        /// <summary>
        ///     The api's json options - one set for the http and the object route.
        ///     With a host, register them in its service collection - Setup() takes them from there.
        ///     Without one, assign them here: the sdk's own container is rebuilt with them on the next assert.
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptions
        {
            get => _jsonSerializerOptions;

            set
            {
                lock (ServiceProviderGate)
                {
                    _jsonSerializerOptions = value;

                    if (_serviceProviderIsDefault)
                    {
                        _serviceProvider = null;
                    }
                }
            }
        }

        // Output function
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }

        /// <summary>
        ///     Custom implementation of IAssertableHttpClient for intercepting HTTP assertions.
        ///     Allows developers to plug in their own assertion logic while maintaining type safety.
        ///     Defaults to the standard AssertableHttpClient implementation. Setup() resets it to that default.
        /// </summary>
        public static IAssertableHttpClient CustomAssertableHttpClient
        {
            get => AssertableHttpClientFor(Assembly.GetCallingAssembly());

            set => _customAssertableHttpClient = value;
        }

        private static IAssertableHttpClient AssertableHttpClientFor(Assembly consumerAssembly)
        {
            return _customAssertableHttpClient ?? GetService<IAssertableHttpClient>(consumerAssembly);
        }

#pragma warning disable CA1859
        internal static async Task AssertHttpCallAsync(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       HttpMethod httpMethod,
                                                       (string Key, object? Value)[] parameters,
                                                       Assembly callingAssembly,
                                                       [CallerArgumentExpression(nameof(payloadAsJson))]
                                                       string payloadAsJsonParameterName = "",
                                                       [CallerFilePath] string callerFilePath = "",
                                                       bool isSuccessStatusCode = true,
                                                       bool writeResponse = false,
                                                       bool skipEndpointValidation = false,
                                                       HttpStatusCode? expectedHttpStatusCode = null,
                                                       IReadOnlyDictionary<string, string>? requestHeaders = null,
                                                       [CallerMemberName] string callerMemberName = "",
                                                       [CallerLineNumber] int callerLineNumber = 0)
#pragma warning restore CA1859
        {
            // Non-generic overload for endpoints without response body (e.g., 204 NoContent)
            // Creates context with ExpectedType = typeof(void) to match endpoint signature

            var parameterReplacer = GetService<IParameterReplacer>(callingAssembly);

            // Resolve embedded files once here
            var payloadFile = GetService<IEmbeddedFileLocalizer>(callingAssembly).LocalizeRequestFile(payloadAsJson, callerFilePath, callingAssembly);
            var expectedResultFile = new EmbeddedFileInfo(string.Empty, string.Empty, null);

            // Resolve parameters in payload
            var resolvedPayload = parameterReplacer.ResolveParameters(payloadFile.Content, parameters);
            var resolvedExpectedJson = string.Empty;

            // URL parameter replacement
            var resolvedUrl = parameterReplacer.ReplaceInUrl(url, parameters);

            var apiVersion = GetService<IApiVersionResolver>(callingAssembly).Resolve(url, client);

            // Create context with ExpectedType = typeof(void) for NoContent scenarios
            var context = new HttpAssertContext<string>
            {
                CallerFilePath = callerFilePath,
                CallerMemberName = callerMemberName,
                CallerLineNumber = callerLineNumber,
                CallingAssembly = callingAssembly,
                Client = client,
                Current = null,
                CurrentObject = null,
                CurrentResultParameterName = "Current response",
                DifferenceFunc = difference => difference,
                ExpectedType = typeof(void), // <-- Key difference: void for NoContent
                ExpectedObjectAsJson = IgnoreResponseComparison,
                ExpectedResultFile = expectedResultFile,
                ExpectedResultParameterName = string.Empty,
                HttpMethod = httpMethod,
                IsSuccessStatusCode = isSuccessStatusCode,
                Parameters = parameters,
                PayloadAsJson = payloadAsJson,
                PayloadFile = payloadFile,
                PayloadParameterName = payloadAsJsonParameterName,
                ResolvedExpectedJson = resolvedExpectedJson,
                ResolvedPayload = resolvedPayload,
                ShowTokenInCurl = ShowTokenInCurl,
                TypeIsPrimitiveType = true,
                Url = resolvedUrl,
                WriteResponse = writeResponse,
                ApiVersion = apiVersion,
                IgnoreResponse = false,
                SkipEndpointValidation = skipEndpointValidation,
                ExpectedHttpStatusCode = expectedHttpStatusCode,
                Expected = null,
                RequestHeaders = requestHeaders
            };

            await AssertableHttpClientFor(callingAssembly).AssertAsync(context).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                  string url,
                                                                  string payloadAsJson,
                                                                  string expectedResult,
                                                                  Func<TResult?, TResult?>? filterFunc,
                                                                  HttpMethod httpMethod,
                                                                  (string Key, object? Value)[] parameters,
                                                                  Assembly callingAssembly,
                                                                  bool isSuccessStatusCode = true,
                                                                  bool writeResponse = false,
                                                                  bool ignoreResponse = false,
                                                                  bool skipEndpointValidation = false,
                                                                  HttpStatusCode? expectedHttpStatusCode = null,
                                                                  [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                  string payloadAsJsonParameterName = "",
                                                                  [CallerArgumentExpression(nameof(expectedResult))]
                                                                  string expectedResultParameterName = "",
                                                                  [CallerFilePath] string callerFilePath = "",
                                                                  [CallerMemberName] string callerMemberName = "",
                                                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url,
                                              payloadAsJson,
                                              expectedResult,
                                              filterFunc,
                                              httpMethod,
                                              difference => difference,
                                              parameters,
                                              callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: isSuccessStatusCode,
                                              writeResponse: writeResponse,
                                              ignoreResponse: ignoreResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        internal static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   TResult expectedResponse,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?>? filterFunc,
                                                                   HttpMethod httpMethod,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool isSuccessStatusCode = true,
                                                                   bool writeResponse = false,
                                                                   bool ignoreResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   HttpStatusCode? expectedHttpStatusCode = null,
                                                                   Predicate<Difference>? differenceFilter = null,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                   string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResponse))]
                                                                   string expectedResponseParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            // Detect if expectedResponse should trigger C# code generation
            var isEmptyAnonymous = GetService<IEmptyAnonymousObjectDetector>(callingAssembly).IsEmptyAnonymousObject(expectedResponse, expectedResponseParameterName);
            SdkTrace.WriteLine($"[HttpCall with object] expectedResponseParameterName='{expectedResponseParameterName}', isEmptyAnonymous={isEmptyAnonymous}");

            return client.AssertHttpCallAsyncWithDetection(url,
                                                           payloadAsJson,
                                                           expectedResult,
                                                           filterFunc,
                                                           httpMethod,
                                                           differenceFunc,
                                                           parameters,
                                                           callingAssembly,
                                                           isEmptyAnonymous,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: expectedResponseParameterName,
                                                           callerFilePath: callerFilePath,
                                                           isSuccessStatusCode: isSuccessStatusCode,
                                                           writeResponse: writeResponse,
                                                           ignoreResponse: ignoreResponse,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           differenceFilter: differenceFilter,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        private static Task<TResult> AssertHttpCallAsyncWithDetection<TResult>(this HttpClient client,
                                                                               string url,
                                                                               string payloadAsJson,
                                                                               string expectedResult,
                                                                               Func<TResult?, TResult?>? filterFunc,
                                                                               HttpMethod httpMethod,
                                                                               Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                               (string Key, object? Value)[] parameters,
                                                                               Assembly callingAssembly,
                                                                               bool isEmptyAnonymous,
                                                                               bool isSuccessStatusCode = true,
                                                                               bool writeResponse = false,
                                                                               bool ignoreResponse = false,
                                                                               bool skipEndpointValidation = false,
                                                                               HttpStatusCode? expectedHttpStatusCode = null,
                                                                               Predicate<Difference>? differenceFilter = null,
                                                                               [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                               string payloadAsJsonParameterName = "",
                                                                               [CallerArgumentExpression(nameof(expectedResult))]
                                                                               string expectedResultParameterName = "",
                                                                               [CallerFilePath] string callerFilePath = "",
                                                                               [CallerMemberName] string callerMemberName = "",
                                                                               [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url,
                                              payloadAsJson,
                                              expectedResult,
                                              filterFunc,
                                              httpMethod,
                                              differenceFunc,
                                              parameters,
                                              callingAssembly,
                                              isEmptyAnonymous,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: isSuccessStatusCode,
                                              writeResponse: writeResponse,
                                              ignoreResponse: ignoreResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              differenceFilter: differenceFilter,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        internal static async Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                         string url,
                                                                         string payloadAsJson,
                                                                         string expectedResult,
                                                                         Func<TResult?, TResult?>? filterFunc,
                                                                         HttpMethod httpMethod,
                                                                         Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                         (string Key, object? Value)[] parameters,
                                                                         Assembly callingAssembly,
                                                                         bool? isEmptyAnonymous = null,
                                                                         bool isSuccessStatusCode = true,
                                                                         bool writeResponse = false,
                                                                         bool ignoreResponse = false,
                                                                         bool skipEndpointValidation = false,
                                                                         HttpStatusCode? expectedHttpStatusCode = null,
                                                                         Predicate<Difference>? differenceFilter = null,
                                                                         IReadOnlyDictionary<string, string>? requestHeaders = null,
                                                                         [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                         string payloadAsJsonParameterName = "",
                                                                         [CallerArgumentExpression(nameof(expectedResult))]
                                                                         string expectedResultParameterName = "",
                                                                         [CallerFilePath] string callerFilePath = "",
                                                                         [CallerMemberName] string callerMemberName = "",
                                                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            // Determine if the expected result type is primitive (used for validation logic)
            var targetIsPrimitiveType = typeof(TResult).IsPrimitive || typeof(TResult).EqualsTo(typeof(string));

            // EARLY VALIDATION: Check .json extension BEFORE any other processing
            // This provides the best error message with full context and suggested fix
            GetService<IJsonFileExtensionValidator>(callingAssembly).ValidatePayloadAndExpectedResult(payloadAsJson,
                                                                                                      expectedResult,
                                                                                                      targetIsPrimitiveType,
                                                                                                      callerFilePath,
                                                                                                      callerLineNumber);

            var embeddedFileLocalizer = GetService<IEmbeddedFileLocalizer>(callingAssembly);
            var parameterReplacer = GetService<IParameterReplacer>(callingAssembly);

            // Resolve embedded files once here - this avoids duplicate resolution later in the pipeline
            var payloadFile = embeddedFileLocalizer.LocalizeRequestFile(payloadAsJson, callerFilePath, callingAssembly);
            var expectedResultFile = embeddedFileLocalizer.LocalizeResponseFile(expectedResult, callerFilePath, callingAssembly);

            // Resolve parameters in payload once here - ready-to-use for HTTP call
            var resolvedPayload = parameterReplacer.ResolveParameters(payloadFile.Content, parameters);

            var resolvedExpectedJson = parameterReplacer.ResolveParameters(expectedResultFile.Content, parameters);

            // URL parameter replacement - replace placeholders in URL with actual values
            // This is the only preprocessing needed here, all other logic is handled by AssertableHttpClient
            var resolvedUrl = parameterReplacer.ReplaceInUrl(url, parameters);

            var apiVersion = GetService<IApiVersionResolver>(callingAssembly).Resolve(url, client);

            // PROTOTYPE: Detect empty anonymous object for code generation
            // Use provided detection result or fallback to JSON check.
            // A snapshot FILE whose content happens to be "{}" is not an inline empty anonymous
            // object - it is an empty snapshot waiting to be written. Treating it as one routes the
            // request to the C# writer, which refuses *.json, while both json writers refuse the
            // generator mode: no writer claims the request and the snapshot is silently never written.
            var expectationIsSnapshotFile = expectedResultFile.EmbeddedFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase);

            var detectedIsEmptyAnonymous = (isEmptyAnonymous ?? (resolvedExpectedJson == "{}")) &&
                                           expectationIsSnapshotFile.IsFalse();

            SdkTrace.WriteLine($"[HttpCall] resolvedExpectedJson='{resolvedExpectedJson}', isEmptyAnonymous={detectedIsEmptyAnonymous} (provided={isEmptyAnonymous})");

            // Create public context directly - no need for internal context
            var context = new HttpAssertContext<TResult>
            {
                CallerFilePath = callerFilePath,
                CallerMemberName = callerMemberName,
                CallerLineNumber = callerLineNumber,
                CallingAssembly = callingAssembly,
                Client = client,
                Current = default,
                CurrentObject = null,
                CurrentResultParameterName = "Current response",
                DifferenceFunc = differenceFunc,
                DifferenceFilter = differenceFilter ?? (static _ => true),
                ExpectedType = typeof(TResult),
                ExpectedObjectAsJson = expectedResult,
                ExpectedResultFile = expectedResultFile,
                ExpectedResultParameterName = expectedResultParameterName,
                HttpMethod = httpMethod,
                IsSuccessStatusCode = isSuccessStatusCode,
                OrderFunc = filterFunc,
                Parameters = parameters,
                PayloadAsJson = payloadAsJson,
                PayloadFile = payloadFile,
                PayloadParameterName = payloadAsJsonParameterName,
                ResolvedExpectedJson = resolvedExpectedJson,
                ResolvedPayload = resolvedPayload,
                ShowTokenInCurl = ShowTokenInCurl,
                TypeIsPrimitiveType = targetIsPrimitiveType,
                Url = resolvedUrl,
                WriteResponse = writeResponse,
                ApiVersion = apiVersion,
                IgnoreResponse = ignoreResponse,
                SkipEndpointValidation = skipEndpointValidation,
                ExpectedHttpStatusCode = expectedHttpStatusCode,
                Expected = default,
                IsEmptyAnonymousObjectForCodeGeneration = detectedIsEmptyAnonymous,
                RequestHeaders = requestHeaders
            };

            var result = await AssertableHttpClientFor(callingAssembly).AssertAsync(context).ConfigureAwait(false);

            return result;
        }
    }
}