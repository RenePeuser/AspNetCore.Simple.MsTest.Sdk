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
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    ///     The one place every static assert entry - http and object route alike - resolves its services
    ///     and its <see cref="TestSdkSettings" /> from. There is no second container, no hand-wired copy
    ///     and no global setting anywhere else.
    /// </summary>
    public static partial class HttpClientAssertExtensions
    {
        private static readonly Lock ServiceProviderGate = new();

        private static IServiceProvider? _serviceProvider;

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
            }
        }

        /// <summary>
        ///     Configures the sdk for test projects without a host - pure object asserts, for example.
        ///     Call it once in [AssemblyInitialize]; calling it again replaces the settings.
        ///     With a host, pass the settings to <c>AddAssertableHttpClient</c> / <c>AddTestSdkSettings</c> instead.
        /// </summary>
        /// <param name="configureSettings">Code-only settings applied on top of the environment variables.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Setup(Action<TestSdkSettings> configureSettings)
        {
            // The last frame where the consumer is still the caller - see ITextDecoratorProvider.
            var consumerAssembly = Assembly.GetCallingAssembly();

            lock (ServiceProviderGate)
            {
                _serviceProvider = CreateDefaultServiceProvider(consumerAssembly, configureSettings);
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

        /// <summary>
        ///     The json options of the application - they live in <see cref="TestSdkSettings" />.
        /// </summary>
        internal static JsonSerializerOptions JsonSerializerOptionsFor(Assembly consumerAssembly)
        {
            return GetService<JsonSerializerOptions>(consumerAssembly);
        }

        private static IServiceProvider EnsureDefaultServiceProvider(Assembly consumerAssembly)
        {
            lock (ServiceProviderGate)
            {
                return _serviceProvider ??= CreateDefaultServiceProvider(consumerAssembly, configureSettings: null);
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
        private static ServiceProvider CreateDefaultServiceProvider(Assembly consumerAssembly,
                                                                    Action<TestSdkSettings>? configureSettings)
        {
            // Without a host the environment is the only configuration source - TestSdkSettings__OutputMode
            // and friends bind exactly as they do in a host.
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration, configureSettings, consumerAssembly);

            // No EndpointDataSource without a host.
            services.Replace(ServiceDescriptor.Singleton<IEndpointProvider, EmptyEndpointProvider>());

            return services.BuildServiceProvider();
        }
    }

    public static partial class HttpClientAssertExtensions
    {
        // Quickfix to hold the whole api compatible
        private const string IgnoreResponseComparison = "IgnoreResponse";

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
                ShowTokenInCurl = GetService<TestSdkSettings>(callingAssembly).ShowTokenInCurl,
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

            await GetService<IAssertableHttpClient>(callingAssembly).AssertAsync(context).ConfigureAwait(false);
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
                ShowTokenInCurl = GetService<TestSdkSettings>(callingAssembly).ShowTokenInCurl,
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

            var result = await GetService<IAssertableHttpClient>(callingAssembly).AssertAsync(context).ConfigureAwait(false);

            return result;
        }
    }
}