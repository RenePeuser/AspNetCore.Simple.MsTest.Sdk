using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        /// <summary>
        /// Initializes all internal static fields with services resolved from the DI container.
        /// Call this method once during test initialization (e.g., in [AssemblyInitialize])
        /// after registering services via services.AddAssertableHttpClient().
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered services</param>
        public static void Setup(IServiceProvider serviceProvider)
        {
            // 1. Resolve core services
            _textDecorator = serviceProvider.GetRequiredService<ITextDecorator>();
            _primitiveTypeConverter = serviceProvider.GetRequiredService<IPrimitiveTypeConverter>();
            _jsonDiffer = serviceProvider.GetRequiredService<IJsonDiffer>();
            _parameterReplacer = serviceProvider.GetRequiredService<IParameterReplacer>();
            _writeResponseService = serviceProvider.GetRequiredService<IWriteResponseService>();
            _jsonSerializerInstance = serviceProvider.GetRequiredService<JsonSerializer>();
            _embeddedFileLocalizer = serviceProvider.GetRequiredService<IEmbeddedFileLocalizer>();
            _responseWriter = serviceProvider.GetRequiredService<IResponseWriter>();

            // 2. Update JsonSerializerOptions from DI
            _jsonSerializerOptions = serviceProvider.GetRequiredService<JsonSerializerOptions>();

            // 3. Resolve builders
            _httpCallInfoTableBuilder = serviceProvider.GetRequiredService<IHttpCallInfoTableBuilder>();
            _differencesTableBuilder = serviceProvider.GetRequiredService<IDifferencesTableBuilder>();
            _jsonSectionBuilder = serviceProvider.GetRequiredService<IJsonSectionBuilder>();
            _curlBuilder = serviceProvider.GetRequiredService<ICurlBuilder>();
            _curlFormatter = serviceProvider.GetRequiredService<ICurlFormatter>();

            // 5. Resolve output builder and assert service
            _outputBuilder = serviceProvider.GetRequiredService<IAssertOutputBuilder>();
            _assertService = serviceProvider.GetRequiredService<IAssertService>();

            // 6. Resolve HTTP handler and update _httpCallHandler
            var httpCallHandler = serviceProvider.GetRequiredService<IHttpCallHandler>();
            _httpCallHandler = (HttpCallHandler)httpCallHandler;

            // 7. Resolve pipeline (all steps are contained within it)
            _httpAssertionPipeline = serviceProvider.GetRequiredService<IHttpAssertionPipeline>();

            // 8. Resolve validation services
            _apiVersionResolver = serviceProvider.GetRequiredService<IApiVersionResolver>();
            _endpointValidator = serviceProvider.GetRequiredService<IEndpointValidator>();

            // 9. Most important: Set CustomAssertableHttpClient to DI instance
            _assertableHttpClientDefault = serviceProvider.GetRequiredService<IAssertableHttpClient>();
            CustomAssertableHttpClient = _assertableHttpClientDefault;

            _emptyEndpointProvider = serviceProvider.GetRequiredService<IEndpointProvider>();
            _endpointValidationOutputBuilder = serviceProvider.GetRequiredService<IEndpointValidationOutputBuilder>();

            _plainTextDecorator = serviceProvider.GetRequiredService<ITextDecorator>();
            _sourceCodeExtractor = serviceProvider.GetRequiredService<ISourceCodeExtractor>();
        }
    }

    public static partial class HttpClientAssertExtensions
    {
        // Text decorator - conditional on build configuration
#if DEBUG
        private static ITextDecorator _textDecorator = new PlainTextDecorator();
#else
        private static ITextDecorator _textDecorator = new AnsiColorTextDecorator();
#endif

        private static IPrimitiveTypeConverter _primitiveTypeConverter = new PrimitiveTypeConverter();

        private static IJsonDiffer _jsonDiffer = new JsonDiffer();

        private static IParameterReplacer _parameterReplacer = new ParameterReplacer();

        private static IResponseWriter _responseWriter = new ResponseWriter([
                                                                                new DifferenceResponseWriter(_jsonDiffer, new JsonPathWriter(), _parameterReplacer),
                                                                                new OverwriteAllResponseWriter(_parameterReplacer)
                                                                            ]);

        private static IWriteResponseService _writeResponseService = new WriteResponseService();

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

        private static IEmbeddedFileLocalizer _embeddedFileLocalizer = new EmbeddedFileLocalizer(new TestCreatorSettings(), JsonSerializerOptions);

        private static JsonSerializer _jsonSerializerInstance = new(JsonSerializerOptions);

        private static ITextDecorator _plainTextDecorator = new PlainTextDecorator();

        private static ICurlFormatter _curlFormatter = new CurlFormatter(_plainTextDecorator);

        // Builders for output strategies
        private static IHttpCallInfoTableBuilder _httpCallInfoTableBuilder = new HttpCallInfoTableBuilder(_textDecorator);

        private static IDifferencesTableBuilder _differencesTableBuilder = new DifferencesTableBuilder(_textDecorator);

        private static IJsonSectionBuilder _jsonSectionBuilder = new JsonSectionBuilder(_textDecorator);

        private static ICurlBuilder _curlBuilder = new CurlBuilder();

        // Output strategies for AssertService
        private static readonly PrimitiveOutputStrategy PrimitiveOutputStrategy = new();

        private static readonly ObjectOutputStrategy ObjectOutputStrategy = new(_differencesTableBuilder, _jsonSectionBuilder);

        private static readonly HttpResponseOutputStrategy HttpResponseOutputStrategy = new(_httpCallInfoTableBuilder,
                                                                                            _differencesTableBuilder,
                                                                                            _jsonSectionBuilder,
                                                                                            _curlBuilder,
                                                                                            _curlFormatter,
                                                                                            _textDecorator);

        private static readonly IAssertOutputStrategy[] OutputStrategies = [PrimitiveOutputStrategy, ObjectOutputStrategy, HttpResponseOutputStrategy];

        private static IAssertOutputBuilder _outputBuilder = new AssertOutputBuilder(OutputStrategies);

        // Comparison strategies (order matters - first match wins)
        private static readonly ISpecificComparisonStrategy StringComparisonStrategy = new StringComparisonStrategy();

        private static readonly ISpecificComparisonStrategy JsonComparisonStrategy = new JsonComparisonStrategy(_jsonDiffer, _jsonSerializerInstance, JsonSerializerOptions);

        private static readonly ISpecificComparisonStrategy[] SpecificComparisonStrategies = [StringComparisonStrategy, JsonComparisonStrategy];

        private static readonly IComparisonStrategy ComparisonStrategy = new ComparisonStrategy(SpecificComparisonStrategies);

        private static IAssertService _assertService = new AssertService(ComparisonStrategy,
                                                                         _responseWriter,
                                                                         _writeResponseService,
                                                                         _outputBuilder);

        // Pipeline (contains all steps internally)
        private static IHttpAssertionPipeline _httpAssertionPipeline = new HttpAssertionPipeline(new IHttpAssertionStep[]
                                                                                                 {
                                                                                                     new StatusCodeValidationStep(_outputBuilder), new ContentTypeHeaderValidationStep(_outputBuilder), new ContentFormatValidationStep(_outputBuilder),
                                                                                                     new JsonComparisonStep(_primitiveTypeConverter,
                                                                                                                            _assertService,
                                                                                                                            _parameterReplacer,
                                                                                                                            _writeResponseService,
                                                                                                                            JsonSerializerOptions)
                                                                                                 });

        private static IApiVersionResolver _apiVersionResolver = new ApiVersionResolver();

        private static IEndpointProvider _emptyEndpointProvider = new EmptyEndpointProvider();

        private static ISourceCodeExtractor _sourceCodeExtractor = new SourceCodeExtractor();

        private static IEndpointValidationOutputBuilder _endpointValidationOutputBuilder = new EndpointValidationOutputBuilder(_curlFormatter, _sourceCodeExtractor);

        private static IEndpointValidator _endpointValidator = new EndpointValidator(_emptyEndpointProvider, _endpointValidationOutputBuilder);

        private static IAssertableHttpClient _assertableHttpClientDefault = new AssertableHttpClient.AssertableHttpClient(_httpCallHandler,
                                                                                                                          _parameterReplacer,
                                                                                                                          _httpAssertionPipeline,
                                                                                                                          _primitiveTypeConverter,
                                                                                                                          JsonSerializerOptions,
                                                                                                                          _endpointValidator);

        /// <summary>
        /// Custom implementation of IAssertableHttpClient for intercepting HTTP assertions.
        /// Allows developers to plug in their own assertion logic while maintaining type safety.
        /// Defaults to the standard AssertableHttpClient implementation.
        /// </summary>
        public static IAssertableHttpClient CustomAssertableHttpClient { get; set; } = _assertableHttpClientDefault;

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
                                                bool writResponse = false,
                                                [CallerMemberName] string callerMemberName = "",
                                                [CallerLineNumber] int callerLineNumber = 0)
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
                                                      writResponse,
                                                      callerMemberName,
                                                      callerLineNumber);
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
                                                                  bool writResponse = false,
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
                                              payloadAsJsonParameterName,
                                              expectedResultParameterName,
                                              callerFilePath,
                                              isSuccessStatusCode,
                                              writResponse,
                                              callerMemberName,
                                              callerLineNumber);
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
                                                                        bool writResponse = false,
                                                                        [CallerMemberName] string callerMemberName = "",
                                                                        [CallerLineNumber] int callerLineNumber = 0)
        {
            // Resolve embedded files once here - this avoids duplicate resolution later in the pipeline
            var payloadFile = _embeddedFileLocalizer.LocalizeRequestFile(payloadAsJson, callerFilePath, callingAssembly);
            var expectedResultFile = _embeddedFileLocalizer.LocalizeResponseFile(expectedResult, callerFilePath, callingAssembly);

            // Resolve parameters in payload once here - ready-to-use for HTTP call
            var resolvedPayload = _parameterReplacer.ResolveParameters(payloadFile.Content, parameters);

            var resolvedExpectedJson = _parameterReplacer.ResolveParameters(expectedResultFile.Content, parameters);

            // URL parameter replacement - replace placeholders in URL with actual values
            // This is the only preprocessing needed here, all other logic is handled by AssertableHttpClient
            var resolvedUrl = _parameterReplacer.ReplaceInUrl(url, parameters);

            var targetIsPrimitiveType = typeof(TResult).IsPrimitive || typeof(TResult).EqualsTo(typeof(string));

            var apiVersion = _apiVersionResolver.Resolve(url, client);

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
                              ShowTokenInCurl = false,
                              TypeIsPrimitiveType = targetIsPrimitiveType,
                              Url = resolvedUrl,
                              WriteResponse = writResponse,
                              ApiVersion = apiVersion
                          };

            var result = await CustomAssertableHttpClient.AssertAsync(context).ConfigureAwait(false);

            return result;
        }
    }
}
