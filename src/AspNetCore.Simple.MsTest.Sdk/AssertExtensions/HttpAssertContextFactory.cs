using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Factory methods for creating HttpAssertContext instances.
    /// Provides convenient creation methods for different scenarios.
    /// </summary>
    internal static class HttpAssertContextFactory
    {
        // ============================================================
        // Generic Context Factory Methods
        // ============================================================

        /// <summary>
        /// Creates a context with explicit HttpMethod override.
        /// Used by method-specific extensions (GET, POST, etc.) that set the HTTP method.
        /// </summary>
        public static HttpAssertContext<TResult> FromContext<TResult>(HttpAssertContext<TResult> publicContext,
                                                                      HttpMethod httpMethod)
        {
            return publicContext with { HttpMethod = httpMethod };
        }

        /// <summary>
        /// Creates a context from individual parameters (for backward compatibility with old extension signatures).
        /// </summary>
        public static HttpAssertContext<TResult> FromParameters<TResult>(HttpClient client,
                                                                         string url,
                                                                         string payloadAsJson,
                                                                         string expectedResult,
                                                                         Func<TResult?, TResult?> orderFunc,
                                                                         HttpMethod httpMethod,
                                                                         Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                         (string Key, object? Value)[] parameters,
                                                                         Assembly callingAssembly,
                                                                         string payloadAsJsonParameterName = "",
                                                                         string expectedResultParameterName = "",
                                                                         string callerFilePath = "",
                                                                         bool isSuccessStatusCode = true,
                                                                         bool writeResponse = false)
        {
            return new HttpAssertContext<TResult>
                   {
                       Client = client,
                       Url = url,
                       PayloadAsJson = payloadAsJson,
                       ExpectedObjectAsJson = expectedResult,
                       Current = default,
                       HttpMethod = httpMethod,
                       OrderFunc = orderFunc,
                       DifferenceFunc = differenceFunc,
                       Parameters = parameters,
                       CallingAssembly = callingAssembly,
                       WriteResponse = writeResponse,
                       IsSuccessStatusCode = isSuccessStatusCode,
                       CallerFilePath = callerFilePath,
                       PayloadParameterName = payloadAsJsonParameterName,
                       ExpectedResultParameterName = expectedResultParameterName,
                       CurrentResultParameterName = "Current response"
                   };
        }

        // ============================================================
        // String Result Convenience Methods
        // ============================================================

        /// <summary>
        /// Creates a string context from individual parameters.
        /// Used for HTTP calls without response comparison (just execution).
        /// </summary>
        public static HttpAssertContext<string> CreateFrom(HttpClient client,
                                                           string url,
                                                           string payloadAsJson,
                                                           HttpMethod httpMethod,
                                                           (string Key, object? Value)[] parameters,
                                                           Assembly callingAssembly,
                                                           string payloadAsJsonParameterName = "",
                                                           string callerFilePath = "",
                                                           bool isSuccessStatusCode = true,
                                                           bool writeResponse = false)
        {
            return new HttpAssertContext<string>
                   {
                       Client = client,
                       Url = url,
                       PayloadAsJson = payloadAsJson,
                       ExpectedObjectAsJson = "IgnoreResponse", // Special marker for no comparison
                       Current = default,
                       HttpMethod = httpMethod,
                       OrderFunc = item => item,
                       DifferenceFunc = item => item,
                       Parameters = parameters,
                       CallingAssembly = callingAssembly,
                       WriteResponse = writeResponse,
                       IsSuccessStatusCode = isSuccessStatusCode,
                       CallerFilePath = callerFilePath,
                       PayloadParameterName = payloadAsJsonParameterName,
                       ExpectedResultParameterName = string.Empty,
                       CurrentResultParameterName = "Current response",
                       ShowTokenInCurl = false
                   };
        }
    }
}
