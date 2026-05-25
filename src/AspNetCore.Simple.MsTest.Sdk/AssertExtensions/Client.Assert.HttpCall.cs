using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers;
using AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        /// <summary>
        ///     Initializes all internal static fields with services resolved from the DI container.
        ///     Call this method once during test initialization (e.g., in [AssemblyInitialize])
        ///     after registering services via services.AddAssertableHttpClient().
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered services</param>
        public static void Setup(IServiceProvider serviceProvider)
        {
            // 0. Detect calling assembly debug mode first
            var callingAssembly = Assembly.GetCallingAssembly();
            var useDebugDecorator = callingAssembly.IsCompiledInDebug();

            // 1. Resolve core services - use appropriate text decorator based on calling assembly
            if (useDebugDecorator)
            {
                _textDecorator = new PlainTextDecorator();
            }
            else
            {
                _textDecorator = new AnsiColorTextDecorator();
            }

            _primitiveTypeConverter = serviceProvider.GetRequiredService<IPrimitiveTypeConverter>();
            _jsonDiffer = serviceProvider.GetRequiredService<IJsonDiffer>();
            _parameterReplacer = serviceProvider.GetRequiredService<IParameterReplacer>();
            _writeResponseService = serviceProvider.GetRequiredService<IWriteResponseService>();
            _jsonSerializerInstance = serviceProvider.GetRequiredService<JsonSerializer>();
            _embeddedFileLocalizer = serviceProvider.GetRequiredService<IEmbeddedFileLocalizer>();
            _responseWriter = serviceProvider.GetRequiredService<IResponseWriter>();

            // 2. Update JsonSerializerOptions from DI
            _jsonSerializerOptions = serviceProvider.GetRequiredService<JsonSerializerOptions>();

            // 3. Resolve builders - use the correct text decorator
            var tableBuilder = new TableBuilder();
            _httpCallInfoTableBuilder = new HttpCallInfoTableBuilder(_textDecorator);
            _differencesTableBuilder = new DifferencesTableBuilder(tableBuilder, _textDecorator);
            _jsonSectionBuilder = new JsonSectionBuilder(_textDecorator);
            _curlBuilder = serviceProvider.GetRequiredService<ICurlBuilder>();
            _curlFormatter = new CurlFormatter(_textDecorator);
            _curlPrinter = serviceProvider.GetRequiredService<ICurlPrinter>();

            // 3.1. Create HTTP failure strategies
            var httpFailureOutputHelper = new HttpFailureOutputHelper();

            var httpFailureStrategies = new IHttpFailureOutputStrategy[]
                                        {
                                            new StatusCodeMismatchOutputStrategy(_textDecorator, httpFailureOutputHelper), new SchemaMismatchOutputStrategy(_textDecorator, httpFailureOutputHelper), new SnapshotMismatchOutputStrategy(_textDecorator, httpFailureOutputHelper),
                                            new ContentTypeMismatchOutputStrategy(_textDecorator, httpFailureOutputHelper)
                                        };

            var defaultHttpFailureStrategy = new DefaultHttpFailureOutputStrategy(_textDecorator, httpFailureOutputHelper);
            var httpFailureOutputBuilder = new HttpFailureOutputBuilder(httpFailureStrategies, defaultHttpFailureStrategy);

            // 4. Rebuild output strategies with the correct decorator
            var primitiveOutputStrategy = new PrimitiveOutputStrategy(_textDecorator);
            var objectOutputStrategy = new ObjectOutputStrategy(_differencesTableBuilder, _jsonSectionBuilder, _textDecorator);

            var httpResponseOutputStrategy = new HttpResponseOutputStrategy(httpFailureOutputBuilder,
                                                                            _httpCallInfoTableBuilder,
                                                                            _differencesTableBuilder,
                                                                            _jsonSectionBuilder,
                                                                            _curlBuilder,
                                                                            _curlFormatter);

            var outputStrategies = new IAssertOutputStrategy[] { primitiveOutputStrategy, objectOutputStrategy, httpResponseOutputStrategy };

            // 5. Create output builder and assert service with rebuilt strategies
            _outputBuilder = new AssertOutputBuilder(outputStrategies);

            var comparisonStrategy = new ComparisonStrategy(SpecificComparisonStrategies);

            _assertService = new AssertService(comparisonStrategy,
                                               _responseWriter,
                                               _writeResponseService,
                                               _outputBuilder);

            // 6. Resolve HTTP handler and update _httpCallHandler
            var httpCallHandler = serviceProvider.GetRequiredService<IHttpCallHandler>();
            _httpCallHandler = (HttpCallHandler)httpCallHandler;

            // 7. Rebuild pipeline with the updated components
            _httpAssertionPipeline = new HttpAssertionPipeline([
                                                                   new StatusCodeValidationStep(_outputBuilder),
                                                                   new ContentTypeHeaderValidationStep(_outputBuilder),
                                                                   new ContentFormatValidationStep(_outputBuilder),
                                                                   new JsonComparisonStep(_primitiveTypeConverter,
                                                                                          _assertService,
                                                                                          _parameterReplacer,
                                                                                          _writeResponseService,
                                                                                          _jsonSerializerOptions),
                                                                   new SuccessfulTestCurlPrinter(_curlPrinter)
                                                               ]);

            // 8. Resolve validation services
            _apiVersionResolver = serviceProvider.GetRequiredService<IApiVersionResolver>();

            _emptyEndpointProvider = serviceProvider.GetRequiredService<IEndpointProvider>();
            _sourceCodeExtractor = serviceProvider.GetRequiredService<ISourceCodeExtractor>();

            _endpointValidationOutputBuilder = new EndpointValidationOutputBuilder(tableBuilder, _curlBuilder, _curlFormatter,
                                                                                   _sourceCodeExtractor, _textDecorator);

            _endpointValidator = new EndpointValidator(_emptyEndpointProvider, _endpointValidationOutputBuilder);

            // 9. Resolve error handling strategy
            _testErrorHandlingStrategy = serviceProvider.GetRequiredService<ITestErrorHandlingStrategy>();

            // 10. Most important: Rebuild AssertableHttpClient with all updated components
            _assertableHttpClientDefault = new AssertableHttpClient.AssertableHttpClient(_httpCallHandler,
                                                                                         _parameterReplacer,
                                                                                         _httpAssertionPipeline,
                                                                                         _primitiveTypeConverter,
                                                                                         _jsonSerializerOptions,
                                                                                         _endpointValidator,
                                                                                         _testErrorHandlingStrategy);

            CustomAssertableHttpClient = _assertableHttpClientDefault;

            _plainTextDecorator = new PlainTextDecorator();
        }
    }

    public static partial class HttpClientAssertExtensions
    {
        public static bool SkipEndpointValidation { get; set; }

        // Quickfix to hold the whole api compatible
        private const string IgnoreResponseComparison = "IgnoreResponse";

        private static ITextDecorator _textDecorator = new PlainTextDecorator();

        private static IPrimitiveTypeConverter _primitiveTypeConverter = new PrimitiveTypeConverter();

        private static IJsonDiffer _jsonDiffer = new JsonDiffer();

        private static IParameterReplacer _parameterReplacer = new ParameterReplacer();

        private static IResponseWriter _responseWriter = new ResponseWriter([
                                                                                new DifferenceResponseWriter(_jsonDiffer, new JsonPathWriter(), _parameterReplacer),
                                                                                new OverwriteAllResponseWriter(_parameterReplacer)
                                                                            ]);

        private static IWriteResponseService _writeResponseService = new WriteResponseService();

        private static HttpCallHandler _httpCallHandler = new(new HttpRequestMessageBuilder(new JsonSerializer(JsonSerializerOptions)));

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
        private static readonly TableBuilder StaticTableBuilder = new();

        private static IHttpCallInfoTableBuilder _httpCallInfoTableBuilder = new HttpCallInfoTableBuilder(_textDecorator);

        private static IDifferencesTableBuilder _differencesTableBuilder = new DifferencesTableBuilder(StaticTableBuilder, _textDecorator);

        private static IJsonSectionBuilder _jsonSectionBuilder = new JsonSectionBuilder(_textDecorator);

        private static ICurlBuilder _curlBuilder = new CurlBuilder();

        private static ICurlPrinter _curlPrinter = new CurlPrinter(_curlFormatter, _curlBuilder);

        // HTTP failure output helper (default plain text decorator)
        private static readonly IHttpFailureOutputHelper _httpFailureOutputHelper = new HttpFailureOutputHelper();

        // HTTP failure strategies (default plain text decorator)
        private static readonly IHttpFailureOutputStrategy[] _httpFailureStrategies =
        [
            new StatusCodeMismatchOutputStrategy(_plainTextDecorator, _httpFailureOutputHelper),
            new SchemaMismatchOutputStrategy(_plainTextDecorator, _httpFailureOutputHelper),
            new SnapshotMismatchOutputStrategy(_plainTextDecorator, _httpFailureOutputHelper),
            new ContentTypeMismatchOutputStrategy(_plainTextDecorator, _httpFailureOutputHelper)
        ];

        private static readonly DefaultHttpFailureOutputStrategy _defaultHttpFailureStrategy = new(_plainTextDecorator, _httpFailureOutputHelper);

        private static readonly HttpFailureOutputBuilder _httpFailureOutputBuilder = new(_httpFailureStrategies, _defaultHttpFailureStrategy);

        // Output strategies for AssertService
        private static readonly PrimitiveOutputStrategy PrimitiveOutputStrategy = new(_plainTextDecorator);

        private static readonly ObjectOutputStrategy ObjectOutputStrategy = new(_differencesTableBuilder, _jsonSectionBuilder, _plainTextDecorator);

        private static readonly HttpResponseOutputStrategy HttpResponseOutputStrategy = new(_httpFailureOutputBuilder,
                                                                                            _httpCallInfoTableBuilder,
                                                                                            _differencesTableBuilder,
                                                                                            _jsonSectionBuilder,
                                                                                            _curlBuilder,
                                                                                            _curlFormatter);

        private static readonly IAssertOutputStrategy[] OutputStrategies =
        [
            PrimitiveOutputStrategy,
            ObjectOutputStrategy,
            HttpResponseOutputStrategy
        ];

        private static IAssertOutputBuilder _outputBuilder = new AssertOutputBuilder(OutputStrategies);

        // Comparison strategies (order matters - first match wins)
        private static readonly ISpecificComparisonStrategy StringComparisonStrategy = new StringComparisonStrategy();

        private static readonly ISpecificComparisonStrategy JsonComparisonStrategy = new JsonComparisonStrategy(_jsonDiffer, _jsonSerializerInstance, JsonSerializerOptions);

        private static readonly ISpecificComparisonStrategy[] SpecificComparisonStrategies =
        [
            StringComparisonStrategy,
            JsonComparisonStrategy
        ];

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

        // Note: Uses _plainTextDecorator as placeholder - will be recreated in Setup() with correct decorator based on calling assembly
        private static IEndpointValidationOutputBuilder _endpointValidationOutputBuilder = new EndpointValidationOutputBuilder(StaticTableBuilder, _curlBuilder, _curlFormatter,
                                                                                                                               _sourceCodeExtractor, _plainTextDecorator);

        private static IEndpointValidator _endpointValidator = new EndpointValidator(_emptyEndpointProvider, _endpointValidationOutputBuilder);

        // Error handling strategy - will be properly initialized in Setup()
        // Default implementation for static initialization
        internal static ITestErrorHandlingStrategy _testErrorHandlingStrategy = CreateDefaultErrorHandlingStrategy();

        private static IAssertableHttpClient _assertableHttpClientDefault = new AssertableHttpClient.AssertableHttpClient(_httpCallHandler,
                                                                                                                          _parameterReplacer,
                                                                                                                          _httpAssertionPipeline,
                                                                                                                          _primitiveTypeConverter,
                                                                                                                          JsonSerializerOptions,
                                                                                                                          _endpointValidator,
                                                                                                                          _testErrorHandlingStrategy);

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

        // Output function
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }

        /// <summary>
        ///     Custom implementation of IAssertableHttpClient for intercepting HTTP assertions.
        ///     Allows developers to plug in their own assertion logic while maintaining type safety.
        ///     Defaults to the standard AssertableHttpClient implementation.
        /// </summary>
        public static IAssertableHttpClient CustomAssertableHttpClient { get; set; } = _assertableHttpClientDefault;

        /// <summary>
        ///     Creates a default error handling strategy for static initialization.
        ///     This will be replaced with the proper DI-based strategy in Setup().
        /// </summary>
        private static TestErrorHandlingStrategy CreateDefaultErrorHandlingStrategy()
        {
            // Create minimal handlers for static initialization
            var tableBuilder = new TableBuilder();
            var curlBuilder = new CurlBuilder();
            var curlFormatter = new CurlFormatter(_plainTextDecorator);
            var sourceCodeExtractor = new SourceCodeExtractor();

            var problemDetailsOutputBuilder = new ProblemDetailsOutputBuilder(tableBuilder, curlBuilder, curlFormatter,
                                                                              sourceCodeExtractor, _plainTextDecorator);

            var handlers = new ITestErrorHandler[]
                           {
                               new ProblemDetailsErrorHandler(problemDetailsOutputBuilder), new InvalidJsonErrorHandler(curlBuilder, curlFormatter, sourceCodeExtractor), new JsonSerializationErrorHandler(curlBuilder, curlFormatter, sourceCodeExtractor),
                               new DefaultErrorHandler()
                           };

            return new TestErrorHandlingStrategy(handlers);
        }

#pragma warning disable CA1859
        private static async Task AssertHttpCallAsync(this HttpClient client,
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
                                                      HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
                                                      [CallerMemberName] string callerMemberName = "",
                                                      [CallerLineNumber] int callerLineNumber = 0)
#pragma warning restore CA1859
        {
            // Non-generic overload for endpoints without response body (e.g., 204 NoContent)
            // Creates context with ExpectedType = typeof(void) to match endpoint signature

            // Resolve embedded files once here
            var payloadFile = _embeddedFileLocalizer.LocalizeRequestFile(payloadAsJson, callerFilePath, callingAssembly);
            var expectedResultFile = new EmbeddedFileInfo(string.Empty, string.Empty, null);

            // Resolve parameters in payload
            var resolvedPayload = _parameterReplacer.ResolveParameters(payloadFile.Content, parameters);
            var resolvedExpectedJson = string.Empty;

            // URL parameter replacement
            var resolvedUrl = _parameterReplacer.ReplaceInUrl(url, parameters);

            var apiVersion = _apiVersionResolver.Resolve(url, client);

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
                OrderFunc = item => item,
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
                ExpectedHttpStatusCode = expectedStatusCode
            };

            await CustomAssertableHttpClient.AssertAsync(context).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                  string url,
                                                                  string payloadAsJson,
                                                                  string expectedResult,
                                                                  Func<TResult?, TResult?> filterFunc,
                                                                  HttpMethod httpMethod,
                                                                  (string Key, object? Value)[] parameters,
                                                                  Assembly callingAssembly,
                                                                  bool isSuccessStatusCode = true,
                                                                  bool writeResponse = false,
                                                                  bool ignoreResponse = false,
                                                                  bool skipEndpointValidation = false,
                                                                  HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
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
                                              expectedStatusCode: expectedStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
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
                                                                        bool isSuccessStatusCode = true,
                                                                        bool writeResponse = false,
                                                                        bool ignoreResponse = false,
                                                                        bool skipEndpointValidation = false,
                                                                        HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
                                                                        [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                        string payloadAsJsonParameterName = "",
                                                                        [CallerArgumentExpression(nameof(expectedResult))]
                                                                        string expectedResultParameterName = "",
                                                                        [CallerFilePath] string callerFilePath = "",
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
                ExpectedHttpStatusCode = expectedStatusCode
            };

            var result = await CustomAssertableHttpClient.AssertAsync(context).ConfigureAwait(false);

            return result;
        }
    }
}