using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Asp.Versioning;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ApiVersioning
{
    /// <summary>
    ///     Where endpoint validation reads the requested api version from: the api's own ApiVersionReader,
    ///     <see cref="TestSdkSettings.ApiVersionReader" /> as override, the common names as last resort.
    /// </summary>
    [TestClass]
    public sealed class ApiVersionResolverTests
    {
        private const string Url = "api/versioning/number";

        private static ApiVersionResolver CreateResolver(IApiVersionReader? appReader = null,
                                                         IApiVersionReader? overrideReader = null)
        {
            var services = new ServiceCollection();

            if (appReader is not null)
            {
                services.AddApiVersioning(options => options.ApiVersionReader = appReader);
            }

            var settings = new TestSdkSettings { ApiVersionReader = overrideReader };

            return new ApiVersionResolver(services.BuildServiceProvider(), settings);
        }

        private static string? Resolve(ApiVersionResolver resolver,
                                       string url,
                                       params (string Key, string Value)[] headers)
        {
            using var client = new HttpClient();

            var requestHeaders = new Dictionary<string, string>();

            foreach (var (key, value) in headers)
            {
                requestHeaders[key] = value;
            }

            return resolver.Resolve(url, client, requestHeaders);
        }

        [TestMethod]
        public void Every_Parameter_Of_A_Combined_Reader_Is_Described()
        {
            var reader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                  new QueryStringApiVersionReader("ver"),
                                                  new HeaderApiVersionReader("x-custom-version"),
                                                  new MediaTypeApiVersionReader("v"));

            var parameters = ApiVersionParameterDetector.Describe(reader, new TestSdkSettings());

            Assert.That.Contains(parameters,
                                 new ApiVersionParameter("ver", ApiVersionParameterKind.Query),
                                 because: "The query reader declares 'ver' - the combined reader has to pass it on.",
                                 fix: "Check that Describe invokes IApiVersionParameterSource.AddParameters and the proxy records AddParameter(name, location).");

            Assert.That.Contains(parameters,
                                 new ApiVersionParameter("x-custom-version", ApiVersionParameterKind.Header),
                                 because: "The header reader declares 'x-custom-version'.",
                                 fix: "Check that the location enum is mapped by name to ApiVersionParameterKind.");

            Assert.That.Contains(parameters,
                                 new ApiVersionParameter("v", ApiVersionParameterKind.MediaTypeParameter),
                                 because: "The media type reader declares the parameter 'v'.",
                                 fix: "Check that MediaTypeParameter is part of ApiVersionParameterKind.");
        }

        [TestMethod]
        public void Header_Name_Of_The_Api_Reader_Is_Detected()
        {
            var resolver = CreateResolver(appReader: new HeaderApiVersionReader("x-from-app"));

            Assert.That.AreEqual("2",
                                 Resolve(resolver, Url, ("x-from-app", "2")),
                                 because: "The api reads 'x-from-app' - validation has to read the same header without any setting.",
                                 fix: "Check that ApiVersionParameterDetector resolves Asp.Versioning's IApiVersionReader from the host provider.");
        }

        [TestMethod]
        public void Common_Names_Are_Ignored_Once_The_Api_Reader_Is_Known()
        {
            var resolver = CreateResolver(appReader: new HeaderApiVersionReader("x-from-app"));

            Assert.That.IsNull(Resolve(resolver, Url + "?api-version=2", ("x-api-version", "2")),
                               because: "The api reads neither ?api-version nor x-api-version. Taking them would validate against a version the api never selects.",
                               fix: "Check VersionParameterNames.From: with a detected reader only its names count.");
        }

        [TestMethod]
        public void Override_In_Test_Settings_Wins_Over_The_Api_Reader()
        {
            var resolver = CreateResolver(appReader: new HeaderApiVersionReader("x-from-app"),
                                          overrideReader: new QueryStringApiVersionReader("from-settings"));

            Assert.That.AreEqual("3",
                                 Resolve(resolver, Url + "?from-settings=3"),
                                 because: "TestSdkSettings.ApiVersionReader is the explicit override - it has to win.",
                                 fix: "Check that ApiVersionParameterDetector.Detect looks at the settings first.");

            Assert.That.IsNull(Resolve(resolver, Url, ("x-from-app", "2")),
                               because: "With an override the detected reader must not be mixed in.",
                               fix: "Check that Detect returns the override's parameters only.");
        }

        [TestMethod]
        public void Without_Any_Reader_The_Common_Names_Are_Used()
        {
            var resolver = CreateResolver();

            Assert.That.AreEqual("2",
                                 Resolve(resolver, Url + "?api-version=2"),
                                 because: "Without versioning services the common query name has to keep working - that is the previous behavior.",
                                 fix: "Check VersionParameterNames.From: an empty detection falls back to the common names.");

            Assert.That.AreEqual("1.0",
                                 Resolve(resolver, Url, ("x-api-version", "1.0")),
                                 because: "The common header names are the fallback as well.",
                                 fix: "Check that the common header names are used when nothing was detected.");
        }

        [TestMethod]
        public void Url_Segment_Wins_Over_Every_Other_Source()
        {
            var resolver = CreateResolver(appReader: new HeaderApiVersionReader("x-from-app"));

            Assert.That.AreEqual("1",
                                 Resolve(resolver, "api/v1/versioning/number", ("x-from-app", "2")),
                                 because: "The url segment is part of the route - it is read from the url, whatever reader is configured.",
                                 fix: "Check that ResolveFromUrlPath runs first.");
        }

        [TestMethod]
        public void Media_Type_Parameter_Is_Read_From_The_Client_Default_Accept_Header()
        {
            var resolver = CreateResolver(appReader: new MediaTypeApiVersionReader("v"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;v=2.0"));

            Assert.That.AreEqual("2.0",
                                 resolver.Resolve(Url, client),
                                 because: "A client configured once with 'Accept: application/json;v=2.0' asks for V2 on every call.",
                                 fix: "Check ResolveFromMediaType: it has to parse the client's default Accept header as well.");
        }
    }
}
