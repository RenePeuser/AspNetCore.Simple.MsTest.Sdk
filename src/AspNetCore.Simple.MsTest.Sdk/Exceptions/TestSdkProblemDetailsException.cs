using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CA1032 // Implement standard exception constructors
namespace AspNetCore.Simple.MsTest.Sdk
{
    public sealed class TestSdkProblemDetailsException : Exception
    {
        public TestSdkProblemDetailsException(string title,
                                              params (string key, string value)[] extensions) : this(HttpStatusCode.InternalServerError, title, string.Empty,
                                                                                                     extensions)
        {
        }

        public TestSdkProblemDetailsException(string title,
                                              string details,
                                              params (string key, string value)[] extensions) : this(HttpStatusCode.InternalServerError, title, details,
                                                                                                     extensions)
        {
        }

        public TestSdkProblemDetailsException(HttpStatusCode statusCode,
                                              string title,
                                              string details,
                                              params (string key, string value)[] extensions) : this(statusCode.ToInt(), title, details,
                                                                                                     extensions.ToImmutableDictionary(item => item.key, item => item.value.ToString()))
        {
        }

        public TestSdkProblemDetailsException(int statusCode,
                                              string title,
                                              string details,
                                              params (string key, string value)[] extensions) : this(statusCode.ToInt(), title, details,
                                                                                                     extensions.ToImmutableDictionary(item => item.key, item => item.value.ToString()))
        {
        }

        // i know this is evil with the conversion to immutable dictionary but a fast fix for now.

        public TestSdkProblemDetailsException(int statusCode,
                                              string title,
                                              string details,
                                              IImmutableDictionary<string, string> errorDetails) : base(title)
        {
            var problemDetails = new ProblemDetails
            {
                Title = title.IsEmpty() ? null : title,
                Detail = details.IsEmpty() ? null : details,
                Status = statusCode
            };

            errorDetails.OrderBy(item => item.Key).ForEach(keyValue =>
            {
                var key = keyValue.Key.Split(" ").Select(value => value.FirstCharToUpper()).Flatten().FirstCharToLower();
                problemDetails.Extensions.Add(key, keyValue.Value);
            });

            ProblemDetails = problemDetails;
        }

        /// <summary>
        /// Constructor that accepts a ProblemDetails object directly.
        /// Used when ProblemDetails is deserialized from HTTP response.
        /// </summary>
        public TestSdkProblemDetailsException(ProblemDetails problemDetails) : base(problemDetails?.Title ?? "ProblemDetails")
        {
            ProblemDetails = problemDetails ?? throw new ArgumentNullException(nameof(problemDetails));
        }

        public ProblemDetails ProblemDetails { get; }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors