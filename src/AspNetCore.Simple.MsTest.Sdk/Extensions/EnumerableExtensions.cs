using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class EnumerableExtensions
    {
        internal static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        internal static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            var sourceList = source.ToList();
            sourceList.ForEach(action);
        }
    }
}
