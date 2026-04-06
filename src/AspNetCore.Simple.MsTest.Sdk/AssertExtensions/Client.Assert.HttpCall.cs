using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Extensions.Pack;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static readonly CurlFormatter CurlFormatterInstance = new();

        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();

        private static readonly JsonDiffer JsonDiffer = new JsonDiffer();

        private static readonly ParameterReplacer ParameterReplacer = new();

        private static readonly ResponseWriter ResponseWriter = new ResponseWriter([
                                                                                       new DifferenceResponseWriter(JsonDiffer, new JsonPathWriter(), ParameterReplacer),
                                                                                       new OverwriteAllResponseWriter(ParameterReplacer)
                                                                                   ]);

        private static readonly WriteResponseService WriteResponseService = new();

        // You have the possible to set and pass the api settings specific json options
        public static JsonSerializerOptions JsonSerializerOptions
        {
            get => _jsonSerializerOptions;

            set
            {
                _jsonSerializerOptions = value;
                _httpCallHandler = new HttpCallHandler(new HttpRequestMessageBuilder(new JsonSerializer(_jsonSerializerOptions)));
            }
        }

        private static HttpCallHandler _httpCallHandler = new(new HttpRequestMessageBuilder(new JsonSerializer(JsonSerializerOptions)));

        // Output function
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }

        // Quickfix to hold the whole api compatible
        private const string IgnoreResponseComparison = "IgnoreResponse";

        private static JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new(new TestCreatorSettings(), JsonSerializerOptions);

        private static readonly JsonSerializer JsonSerializerInstance = new(JsonSerializerOptions);

        // Builders for output strategies
        private static readonly HttpCallInfoTableBuilder HttpCallInfoTableBuilder = new();

        private static readonly DifferencesTableBuilder DifferencesTableBuilder = new();

        private static readonly JsonSectionBuilder JsonSectionBuilder = new();

        private static readonly CurlBuilder CurlBuilder = new();

        // Output strategies for AssertService
        private static readonly PrimitiveOutputStrategy PrimitiveOutputStrategy = new();

        private static readonly ObjectOutputStrategy ObjectOutputStrategy = new(DifferencesTableBuilder, JsonSectionBuilder);

        private static readonly HttpResponseOutputStrategy HttpResponseOutputStrategy = new(HttpCallInfoTableBuilder,
                                                                                            DifferencesTableBuilder,
                                                                                            JsonSectionBuilder,
                                                                                            CurlBuilder,
                                                                                            CurlFormatterInstance);

        private static readonly IAssertOutputStrategy[] OutputStrategies = [PrimitiveOutputStrategy, ObjectOutputStrategy, HttpResponseOutputStrategy];

        private static readonly AssertOutputBuilder OutputBuilder = new(OutputStrategies);

        private static readonly AssertService AssertService = new(JsonDiffer,
                                                                  ResponseWriter,
                                                                  WriteResponseService,
                                                                  JsonSerializerInstance,
                                                                  JsonSerializerOptions,
                                                                  OutputBuilder);

        // Pipeline steps
        private static readonly StatusCodeValidationStep StatusCodeValidationStep = new(OutputBuilder);

        private static readonly ContentTypeHeaderValidationStep ContentTypeHeaderValidationStep = new(OutputBuilder);

        private static readonly ContentFormatValidationStep ContentFormatValidationStep = new(OutputBuilder);

        private static readonly JsonComparisonStep JsonComparisonStep = new(PrimitiveTypeConverter,
                                                                            AssertService,
                                                                            ParameterReplacer,
                                                                            WriteResponseService,
                                                                            JsonSerializerOptions);

        private static readonly HttpAssertionPipeline HttpAssertionPipeline = new(new IHttpAssertionStep[]
                                                                                  {
                                                                                      StatusCodeValidationStep, ContentTypeHeaderValidationStep, ContentFormatValidationStep,
                                                                                      JsonComparisonStep
                                                                                  });

        private static readonly AssertableHttpClient.AssertableHttpClient AssertableHttpClientDefault = new(_httpCallHandler,
                                                                                                            ParameterReplacer,
                                                                                                            HttpAssertionPipeline,
                                                                                                            PrimitiveTypeConverter,
                                                                                                            JsonSerializerOptions);

        /// <summary>
        /// Custom implementation of IAssertableHttpClient for intercepting HTTP assertions.
        /// Allows developers to plug in their own assertion logic while maintaining type safety.
        /// Defaults to the standard AssertableHttpClient implementation.
        /// </summary>
        public static IAssertableHttpClient CustomAssertableHttpClient { get; set; } = AssertableHttpClientDefault;

#pragma warning disable CA1859
        private static Task AssertHttpCallAsync(this HttpClient client,
                                                string url,
                                                string payloadAsJson,
                                                HttpMethod httpMethod,
                                                (string Key, object? Value)[] parameters,
                                                Assembly callingAssembly,
                                                [CallerArgumentExpression(nameof(payloadAsJson))]
                                                string payloadAsJsonParameterName = "",
                                                [CallerFilePath] string callerFilePath = "",
                                                bool isSuccessStatusCode = true,
                                                bool writResponse = false)
#pragma warning restore CA1859
        {
            return client.AssertHttpCallAsync<string>(url,
                                                      payloadAsJson,
                                                      IgnoreResponseComparison,
                                                      item => item,
                                                      httpMethod,
                                                      parameters,
                                                      callingAssembly,
                                                      payloadAsJsonParameterName,
                                                      string.Empty,
                                                      callerFilePath,
                                                      isSuccessStatusCode,
                                                      writResponse);
        }

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                  string url,
                                                                  string payloadAsJson,
                                                                  string expectedResult,
                                                                  Func<TResult?, TResult?> filterFunc,
                                                                  HttpMethod httpMethod,
                                                                  (string Key, object? Value)[] parameters,
                                                                  Assembly callingAssembly,
                                                                  [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                  string payloadAsJsonParameterName = "",
                                                                  [CallerArgumentExpression(nameof(expectedResult))]
                                                                  string expectedResultParameterName = "",
                                                                  [CallerFilePath] string callerFilePath = "",
                                                                  bool isSuccessStatusCode = true,
                                                                  bool writResponse = false)
        {
            return client.AssertHttpCallAsync(url,
                                              payloadAsJson,
                                              expectedResult,
                                              filterFunc,
                                              httpMethod,
                                              difference => difference,
                                              parameters,
                                              callingAssembly,
                                              payloadAsJsonParameterName,
                                              expectedResultParameterName,
                                              callerFilePath,
                                              isSuccessStatusCode,
                                              writResponse);
        }

        private static async Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                        string url,
                                                                        string payloadAsJson,
                                                                        string expectedResult,
                                                                        Func<TResult?, TResult?> filterFunc,
                                                                        HttpMethod httpMethod,
                                                                        Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                        (string Key, object? Value)[] parameters,
                                                                        Assembly callingAssembly,
                                                                        [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                        string payloadAsJsonParameterName = "",
                                                                        [CallerArgumentExpression(nameof(expectedResult))]
                                                                        string expectedResultParameterName = "",
                                                                        [CallerFilePath] string callerFilePath = "",
                                                                        bool isSuccessStatusCode = true,
                                                                        bool writResponse = false)
        {
            // Resolve embedded files once here - this avoids duplicate resolution later in the pipeline
            var payloadFile = EmbeddedFileLocalizer.LocalizeRequestFile(payloadAsJson, callerFilePath, callingAssembly);
            var expectedResultFile = EmbeddedFileLocalizer.LocalizeResponseFile(expectedResult, callerFilePath, callingAssembly);

            // Resolve parameters in payload once here - ready-to-use for HTTP call
            var resolvedPayload = ParameterReplacer.ResolveParameters(payloadFile.Content, parameters);

            var resolvedExpectedJson = ParameterReplacer.ResolveParameters(expectedResultFile.Content, parameters);

            // URL parameter replacement - replace placeholders in URL with actual values
            // This is the only preprocessing needed here, all other logic is handled by AssertableHttpClient
            var resolvedUrl = ParameterReplacer.ReplaceInUrl(url, parameters);

            var targetIsPrimitiveType = typeof(TResult).IsPrimitive || typeof(TResult).EqualsTo(typeof(string));

            // Create public context directly - no need for internal context
            var context = new HttpAssertContext<TResult>
            {
                TypeIsPrimitiveType = targetIsPrimitiveType,
                Client = client,
                Url = resolvedUrl,
                PayloadAsJson = payloadAsJson,
                ExpectedObjectAsJson = expectedResult,
                Current = default,
                HttpMethod = httpMethod,
                OrderFunc = filterFunc,
                DifferenceFunc = differenceFunc,
                Parameters = parameters,
                CallingAssembly = callingAssembly,
                WriteResponse = writResponse,
                IsSuccessStatusCode = isSuccessStatusCode,
                CallerFilePath = callerFilePath,
                PayloadParameterName = payloadAsJsonParameterName,
                ExpectedResultParameterName = expectedResultParameterName,
                CurrentResultParameterName = "Current response",
                PayloadFile = payloadFile,
                ExpectedResultFile = expectedResultFile,
                ResolvedPayload = resolvedPayload,
                ResolvedExpectedJson = resolvedExpectedJson,
                ShowTokenInCurl = false,
                CurrentObject = null,
            };

            var result = await CustomAssertableHttpClient.AssertAsync(context).ConfigureAwait(false);

            return result;
        }
    }
}
