using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
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