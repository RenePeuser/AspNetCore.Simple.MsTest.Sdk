using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Versioning
{
    /// <summary>
    ///     The api reads its version from ?ver=…, the x-custom-version header and the media type parameter
    ///     ";v=" - none of them is a common name. Endpoint validation has to ask the api's ApiVersionReader
    ///     where it reads the version from instead of guessing; no test setting is involved.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("Api Versioning")]
    public class ApiVersionReaderDetectionTests : ApiTestBase
    {
        private const string NumberUrl = "api/versioning-header/number";

        private const string DetectionFix = "Check ApiVersionParameterDetector: it has to find the api's ApiVersionReader (Asp.Versioning service or legacy ApiVersioningOptions) and read its names via IApiVersionParameterSource.AddParameters.";

        [TestMethod]
        public Task Custom_Query_Name_Of_The_Api_Is_Detected()
        {
            return Client.AssertGet(NumberUrl)
                         .WithQueryParameter("ver", 2)
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Custom_Header_Name_Of_The_Api_Is_Detected()
        {
            return Client.AssertGet(NumberUrl)
                         .WithHeader("x-custom-version", "2")
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Media_Type_Parameter_Of_The_Api_Is_Detected()
        {
            return Client.AssertGet(NumberUrl)
                         .WithHeader("Accept", "application/json;v=2")
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Media_Type_Parameter_Selects_V1_As_Well()
        {
            return Client.AssertGet(NumberUrl)
                         .WithHeader("Accept", "application/json;v=1")
                         .Produces<int>(HttpStatusCode.OK)
                         .ExpectedResponse(1)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Custom_Header_Version_2_Does_Not_Accept_The_V1_Response_Type()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(NumberUrl)
                                                                                                    .WithHeader("x-custom-version", "2")
                                                                                                    .Produces<int>(HttpStatusCode.OK)
                                                                                                    .ExpectedResponse(1)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "x-custom-version: 2 selects V2, which declares string. Accepting <int> means the custom header was not read.",
                                                                                        fix: DetectionFix)
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V2 endpoint exists - only the type is wrong. AMBIGUOUS ENDPOINT MATCH would mean the custom header was ignored.",
                                 fix: DetectionFix);
        }

        [TestMethod]
        public async Task Common_Header_Name_The_Api_Does_Not_Read_Is_Ignored()
        {
            // "version" is in the sdk's fallback list, but this api never reads it. The api answers 400 -
            // validation must not pretend V2 was selected.
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(NumberUrl)
                                                                                                    .WithHeader("version", "2")
                                                                                                    .Produces<string>(HttpStatusCode.OK)
                                                                                                    .ExpectedResponse("one")
                                                                                                    .ExecuteAsync(),
                                                                                        because: "The api does not read a 'version' header. Validating against V2 would make the test pass in the sdk and fail against the real api.",
                                                                                        fix: "Check ApiVersionResolver: once the api's reader is detected only its names count - the common fallback names must not be used.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "AMBIGUOUS ENDPOINT MATCH",
                                 because: "Without a version the api reads, V1 and V2 both match - the report has to show both.",
                                 fix: "Check that the 'version' header is not taken as requested version.");
        }
    }
}
