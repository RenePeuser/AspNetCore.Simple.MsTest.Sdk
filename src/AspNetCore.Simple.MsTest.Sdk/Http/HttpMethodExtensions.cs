using System.Net.Http;

namespace AspNetCore.Simple.MsTest.Sdk.Http
{
    /// <summary>
    /// Extends HttpMethod with the QUERY method defined in RFC 9535
    /// </summary>
    public static class HttpMethodExtensions
    {
        /// <summary>
        /// Represents the HTTP QUERY method as defined in RFC 9535.
        /// The QUERY method allows sending a request body with GET-like semantics.
        /// </summary>
        public static HttpMethod Query { get; } = new HttpMethod("QUERY");
    }
}
