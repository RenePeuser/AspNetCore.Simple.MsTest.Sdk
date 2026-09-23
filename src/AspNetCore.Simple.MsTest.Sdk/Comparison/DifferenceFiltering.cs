using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    public static class AddDifferenceFilteringExtension
    {
        public static void AddDifferenceFiltering(this IServiceCollection services)
        {
            services.AddTestSdkSettings();

            services.AddSingletonIfNotExists<IDifferenceFiltering, DifferenceFiltering>();
        }
    }

    /// <summary>
    /// Decides which differences survive before an assert decides pass or fail - and, for the
    /// response writers, which current values must not reach a snapshot.
    /// </summary>
    public interface IDifferenceFiltering
    {
        /// <summary>
        /// Applies the global <see cref="TestSdkSettings.DifferenceFunc"/>, then the per-assert func,
        /// and finally the global and per-assert predicates - a difference is kept only when both
        /// <see cref="TestSdkSettings.DifferenceFilter"/> and <paramref name="perAssertFilter"/> return <c>true</c>.
        /// </summary>
        ImmutableList<Difference> Apply(ImmutableList<Difference> differences,
                                        Func<ImmutableList<Difference>, IEnumerable<Difference>> perAssertFunc,
                                        Predicate<Difference>? perAssertFilter);
    }

    internal sealed class DifferenceFiltering(TestSdkSettings testSdkSettings) : IDifferenceFiltering
    {
        public ImmutableList<Difference> Apply(ImmutableList<Difference> differences,
                                               Func<ImmutableList<Difference>, IEnumerable<Difference>> perAssertFunc,
                                               Predicate<Difference>? perAssertFilter)
        {
            var afterGlobalFunc = testSdkSettings.DifferenceFunc(differences).ToImmutableList();
            var afterFunc = perAssertFunc(afterGlobalFunc).ToImmutableList();

            return afterFunc.Where(difference => testSdkSettings.DifferenceFilter(difference) &&
                                                 (perAssertFilter?.Invoke(difference) ?? true))
                            .ToImmutableList();
        }
    }
}
