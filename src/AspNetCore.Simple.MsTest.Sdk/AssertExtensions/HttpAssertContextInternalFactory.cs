using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Factory methods for creating HttpAssertContextInternal instances.
    /// Separates object creation logic from data objects.
    /// </summary>
    internal static class HttpAssertContextInternalFactory
    {
        // ============================================================
        // Converter Methods
        // ============================================================

        /// <summary>
        /// Converts a non-generic HttpAssertContextInternal to a generic one.
        /// Used for internal conversions where we need to add a result type.
        /// </summary>
        public static HttpAssertContextInternal<TResult> ToGeneric<TResult>(HttpAssertContextInternal nonGenericContext,
                                                                            string expectedResult)
        {
            return new HttpAssertContextInternal<TResult>
            {
                Client = nonGenericContext.Client,
                Url = nonGenericContext.Url,
                PayloadAsJson = nonGenericContext.PayloadAsJson ?? string.Empty,
                ExpectedResult = expectedResult,
                HttpMethod = nonGenericContext.HttpMethod,
                FilterFunc = item => item,
                DifferenceFunc = item => item,
                Parameters = nonGenericContext.Parameters,
                CallingAssembly = nonGenericContext.CallingAssembly,
                PayloadParameterName = nonGenericContext.PayloadParameterName,
                ExpectedResultParameterName = string.Empty,
                CallerFilePath = nonGenericContext.CallerFilePath,
                IsSuccessStatusCode = nonGenericContext.IsSuccessStatusCode,
                WriteResponse = nonGenericContext.WriteResponse
            };
        }

        // ============================================================
        // Generic Context Factory Methods
        // ============================================================

        /// <summary>
        /// Creates an internal context from the public context.
        /// All required values should be in the context (Clean API - Level 3).
        /// </summary>
        public static HttpAssertContextInternal<TResult> FromContext<TResult>(HttpAssertContext<TResult> publicContext,
                                                                              HttpMethod httpMethod)
        {
            return new HttpAssertContextInternal<TResult>
            {
                Client = publicContext.Client,
                Url = publicContext.Url,
                PayloadAsJson = publicContext.PayloadAsJson ?? string.Empty,
                ExpectedResult = publicContext.ExpectedResult ?? string.Empty,
                HttpMethod = httpMethod,
                FilterFunc = publicContext.FilterFunc,
                DifferenceFunc = publicContext.DifferenceFunc,
                Parameters = publicContext.Parameters,
                CallingAssembly = publicContext.CallingAssembly,
                WriteResponse = publicContext.WriteResponse,
                IsSuccessStatusCode = publicContext.IsSuccessStatusCode,
                CallerFilePath = publicContext.CallerFilePath,
                PayloadParameterName = publicContext.PayloadParameterName,
                ExpectedResultParameterName = publicContext.ExpectedResultParameterName
            };
        }

        /// <summary>
        /// Creates an internal context from individual parameters (for backward compatibility).
        /// </summary>
        public static HttpAssertContextInternal<TResult> FromParameters<TResult>(HttpClient client,
                                                                                 string url,
                                                                                 string payloadAsJson,
                                                                                 string expectedResult,
                                                                                 Func<TResult, TResult> filterFunc,
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
            var httpAssertContextInternal = new HttpAssertContextInternal<TResult>
            {
                Client = client,
                Url = url,
                PayloadAsJson = payloadAsJson,
                ExpectedResult = expectedResult,
                FilterFunc = filterFunc,
                HttpMethod = httpMethod,
                DifferenceFunc = differenceFunc,
                Parameters = parameters,
                CallingAssembly = callingAssembly,
                PayloadParameterName = payloadAsJsonParameterName,
                ExpectedResultParameterName = expectedResultParameterName,
                CallerFilePath = callerFilePath,
                IsSuccessStatusCode = isSuccessStatusCode,
                WriteResponse = writeResponse
            };

            return httpAssertContextInternal;
        }

        // ============================================================
        // Non-Generic Context Factory Methods
        // ============================================================

        /// <summary>
        /// Creates an internal non-generic context from the public context.
        /// All required values should be in the context (Clean API - Level 3).
        /// </summary>
        public static HttpAssertContextInternal FromContext(HttpAssertContext publicContext,
                                                            HttpMethod httpMethod)
        {
            var httpAssertContextInternal = new HttpAssertContextInternal
            {
                Client = publicContext.Client,
                Url = publicContext.Url,
                PayloadAsJson = publicContext.PayloadAsJson ?? string.Empty,
                HttpMethod = httpMethod,
                Parameters = publicContext.Parameters,
                CallingAssembly = publicContext.CallingAssembly,
                WriteResponse = publicContext.WriteResponse,
                IsSuccessStatusCode = publicContext.IsSuccessStatusCode,
                CallerFilePath = publicContext.CallerFilePath,
                PayloadParameterName = publicContext.PayloadParameterName
            };

            return httpAssertContextInternal;
        }

        /// <summary>
        /// Creates an internal non-generic context from individual parameters (for backward compatibility).
        /// </summary>
        public static HttpAssertContextInternal CreateFrom(HttpClient client,
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
            var httpAssertContextInternal = new HttpAssertContextInternal
                                            {
                                                Client = client,
                                                Url = url,
                                                PayloadAsJson = payloadAsJson,
                                                HttpMethod = httpMethod,
                                                Parameters = parameters,
                                                CallingAssembly = callingAssembly,
                                                PayloadParameterName = payloadAsJsonParameterName,
                                                CallerFilePath = callerFilePath,
                                                IsSuccessStatusCode = isSuccessStatusCode,
                                                WriteResponse = writeResponse
                                            };

            return httpAssertContextInternal;
        }
    }
}
