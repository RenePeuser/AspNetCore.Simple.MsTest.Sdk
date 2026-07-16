namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Initial fluent state after <c>Client.Post/Get/Put/Patch/Delete(url)</c>.
    /// Configures the request (body, parameters, headers) and transitions to the
    /// response/expectation stage.
    ///
    /// <para>
    /// Body input is EXPLICIT — no rate-heuristic. Pick the method that matches your intent:
    /// <list type="bullet">
    /// <item><see cref="WithBody{T}"/> — a C# object (serialized to JSON).</item>
    /// <item><see cref="WithJsonString"/> — a raw JSON string, verbatim.</item>
    /// <item><see cref="WithEmbeddedJson"/> — an embedded-resource file name.</item>
    /// </list>
    /// </para>
    /// </summary>
    [FluentBuilder]
    public interface IHttpRequestConfiguring
    {
        /// <summary>Configures the request body from a C# object (serialized to JSON).</summary>
        IHttpRequestConfiguring WithBody<T>(T body);

        /// <summary>Configures the request body from a raw JSON string (used verbatim).</summary>
        IHttpRequestConfiguring WithJsonString(string bodyJson);

        /// <summary>Configures the request body from an embedded-resource JSON file name.</summary>
        IHttpRequestConfiguring WithEmbeddedJson(string embeddedFileName);

        /// <summary>Configures placeholder parameters ($Token$) substituted in request/response JSON.</summary>
        IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters);

        /// <summary>Adds a custom HTTP request header.</summary>
        IHttpRequestConfiguring WithHeader(string key, string value);

        // ============================================================
        // Transition to response configuration (expected body).
        // The <T> is MANDATORY on all three — it drives deserialization AND the chain state.
        // ============================================================

        /// <summary>Sets the expected response from a C# object (serialized to JSON).</summary>
        IHttpResponseConfiguring<TResult> Returns<TResult>(TResult expected);

        /// <summary>Sets the expected response from a raw JSON string (used verbatim).</summary>
        IHttpResponseConfiguring<TResult> ReturnsJsonString<TResult>(string expectedJson);

        /// <summary>Sets the expected response from an embedded-resource JSON file name.</summary>
        IHttpResponseConfiguring<TResult> ReturnsEmbeddedJson<TResult>(string embeddedFileName);

        // ============================================================
        // Transition to status-only expectation (no response body validation).
        // ============================================================

        /// <summary>Switches to status-only expectations (no response body comparison).</summary>
        IHttpExpectationConfiguring ExpectingResponse();
    }
}
