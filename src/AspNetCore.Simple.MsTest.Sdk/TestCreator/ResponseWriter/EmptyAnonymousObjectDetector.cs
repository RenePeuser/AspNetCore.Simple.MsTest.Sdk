using System;
using System.Text.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal interface IEmptyAnonymousObjectDetector
    {
        bool IsEmptyAnonymousObject<T>(T obj,
                                       string expressionText);
    }

    internal sealed class EmptyAnonymousObjectDetector : IEmptyAnonymousObjectDetector
    {
        public bool IsEmptyAnonymousObject<T>(T obj,
                                              string expressionText)
        {
            // Layer 1: CallerArgumentExpression check
            var trimmedExpression = expressionText.Trim();

            if (trimmedExpression.EndsWith("{ }", StringComparison.Ordinal) ||
                trimmedExpression.EndsWith("{}", StringComparison.Ordinal))
            {
                return true;
            }

            // Layer 2: Type reflection check
            var type = typeof(T);

            if (type.Name.Contains("AnonymousType", StringComparison.Ordinal) &&
                type.GetProperties().Length == 0)
            {
                return true;
            }

            // Layer 3: JSON serialization check (fallback)
            if (obj != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(obj);

                    if (json == "{}")
                    {
                        return true;
                    }
                }
#pragma warning disable CA1031
                catch
#pragma warning restore CA1031
                {
                    // If serialization fails, it's not an empty anonymous object
                    return false;
                }
            }

            return false;
        }
    }

    internal static class AddEmptyAnonymousObjectDetectorExtension
    {
        public static void AddEmptyAnonymousObjectDetector(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEmptyAnonymousObjectDetector, EmptyAnonymousObjectDetector>();
        }
    }
}