using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    /// <summary>
    /// Removes the response headers that change on every call - see
    /// <see cref="TestSdkSettings.VolatileHeaderNames"/>.
    ///
    /// Applied to BOTH sides of a comparison and to whatever is written to disk. Doing it on one side
    /// only would trade recording noise for a permanently red test, and doing it at write time only
    /// would leave every snapshot recorded before this change failing.
    /// </summary>
    internal static class VolatileHeaderFilter
    {
        public static ImmutableList<KeyValuePair<string, ImmutableList<string>>> WithoutVolatileHeaders(
            this ImmutableList<KeyValuePair<string, ImmutableList<string>>>? headers,
            TestSdkSettings settings)
        {
            if (headers.IsNull() || headers.IsEmpty)
            {
                return ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty;
            }

            var volatileNames = settings.VolatileHeaderNames;

            if (volatileNames.Length.EqualsTo(0))
            {
                return headers;
            }

            var lookup = volatileNames.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            return headers.RemoveAll(header => lookup.Contains(header.Key));
        }

        public static SimpleHttpResponseMessage WithoutVolatileHeaders(this SimpleHttpResponseMessage message,
                                                                       TestSdkSettings settings)
        {
            return message with
            {
                Headers = message.Headers.WithoutVolatileHeaders(settings),
                TrailingHeaders = message.TrailingHeaders.WithoutVolatileHeaders(settings),
                Content = message.Content.IsNull()
                                     ? message.Content
                                     : message.Content with { Headers = message.Content.Headers.WithoutVolatileHeaders(settings) }
            };
        }
    }
}