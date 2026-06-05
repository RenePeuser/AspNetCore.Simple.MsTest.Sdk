namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Defines the type of HTTP assertion failure for context-specific error reporting.
    /// Used by pipeline steps to indicate what kind of validation failed.
    /// </summary>
    public enum HttpAssertionFailureType
    {
        /// <summary>
        /// No specific failure type set (default).
        /// </summary>
        None = 0,

        /// <summary>
        /// Response type doesn't match expected schema.
        /// Example: Expected Person but got UnknownResponse.
        /// </summary>
        SchemaMismatch = 1,

        /// <summary>
        /// HTTP status code doesn't match expected value.
        /// Example: Expected 201 Created but got 400 Bad Request.
        /// </summary>
        StatusCodeMismatch = 2,

        /// <summary>
        /// JSON values differ from snapshot (snapshot testing).
        /// Example: Expected name="Goku" but got name="Vegeta".
        /// </summary>
        SnapshotMismatch = 3,

        /// <summary>
        /// Content-Type header doesn't match expected value.
        /// Example: Expected application/json but got text/html.
        /// </summary>
        ContentTypeMismatch = 4
    }
}