using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();

        private static readonly CurlBuilder CurlBuilder = new();

        private static readonly OutputFormatter OutputFormatter = new(new CurlFormatter());

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

        private static readonly AssertService AssertService = new(PrimitiveTypeConverter,
                                                                          JsonDiffer,
                                                                          new CurlFormatter(),
                                                                          new CurlPrinter(new CurlFormatter()),
                                                                          OutputFormatter,
                                                                          ResponseWriter,
                                                                          WriteResponseService,
                                                                          EmbeddedFileLocalizer,
                                                                          JsonSerializerInstance,
                                                                          JsonSerializerOptions,
                                                                          ParameterReplacer);

        private static readonly AssertableHttpClient AssertableHttpClientDefault = new(HttpOutputFormatter,
                                                                                       CurlBuilder,
                                                                                       OutputFormatter,
                                                                                       PrimitiveTypeConverter,
                                                                                       JsonDiffer,
                                                                                       ResponseWriter,
                                                                                       WriteResponseService,
                                                                                       EmbeddedFileLocalizer,
                                                                                       _httpCallHandler,
                                                                                       JsonSerializerOptions,
                                                                                       AssertService,
                                                                                       ParameterReplacer);

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

        // Context-based string overload (no expected result comparison)
        internal static Task AssertHttpCallAsync(HttpAssertContext<string> context)
        {
            return AssertHttpCallAsync<string>(context);
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

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
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
            // Create public context directly - no need for internal context
            var context = new HttpAssertContext<TResult>
            {
                Client = client,
                Url = url,
                PayloadAsJson = payloadAsJson,
                ExpectedObjectAsJson = expectedResult,
                CurrentObject = default,
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
                CurrentResultParameterName = "Current response"
            };

            return AssertHttpCallAsync(context);
        }

        internal static Task<TResult> AssertHttpCallAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // URL parameter replacement - replace placeholders in URL with actual values
            // This is the only preprocessing needed here, all other logic is handled by AssertableHttpClient
            var sortedParameters = context.Parameters.OrderByDescending(p => p.Key.Length);
            var url = context.Url;

            foreach (var valueTuple in sortedParameters)
            {
                url = url.Replace(valueTuple.Key, valueTuple.Value?.ToString());
            }

            // Update context with resolved URL and ShowTokenInCurl from static field using record 'with' expression
            var updatedContext = context with
            {
                Url = url,
                ShowTokenInCurl = ShowTokenInCurl
            };

            // Delegate to the configured IAssertableHttpClient (default or custom implementation)
            // This allows for type-safe interception of HTTP assertions while maintaining backward compatibility
            return CustomAssertableHttpClient.AssertAsync(updatedContext);
        }
    }
}
