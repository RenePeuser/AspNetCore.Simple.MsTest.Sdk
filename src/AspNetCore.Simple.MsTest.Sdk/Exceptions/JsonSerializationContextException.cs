using System.Text.Json;
#pragma warning disable CA1032 // Implement standard exception constructors
namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Wrapper exception that carries the HttpResponseContext along with the original JsonException.
    /// This allows error handlers to access the response content even when deserialization fails.
    /// </summary>
    internal sealed class JsonSerializationContextException : JsonException
    {
        /// <summary>
        /// The HttpResponseContext containing the response content that failed to deserialize.
        /// </summary>
        public IHttpResponseContext ResponseContext { get; }

        internal JsonSerializationContextException(IHttpResponseContext responseContext)
            : base("JSON deserialization failed - see ResponseContext for details")
        {
            ResponseContext = responseContext;
        }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors