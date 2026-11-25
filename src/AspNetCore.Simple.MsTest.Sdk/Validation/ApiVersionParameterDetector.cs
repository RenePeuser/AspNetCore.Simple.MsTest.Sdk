using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Asp.Versioning;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    /// <summary>
    ///     Where the api reads its version from - mirrors ApiVersionParameterLocation of both
    ///     Asp.Versioning and the legacy Microsoft.AspNetCore.Mvc.Versioning package.
    /// </summary>
    internal enum ApiVersionParameterKind
    {
        Query,
        Header,
        Path,
        MediaTypeParameter
    }

    internal sealed record ApiVersionParameter(string Name,
                                               ApiVersionParameterKind Kind);

    /// <summary>
    ///     Asks the api itself where it reads the version from, so endpoint validation reads it from
    ///     the same place - a custom header like "x-my-version" works without any test setting.
    ///     <para>
    ///         Every ApiVersionReader (url segment, query string, header, media type and the
    ///         combined reader) is an IApiVersionParameterSource: AddParameters reports each name
    ///         together with its location. Both versioning packages define that contract, the
    ///         legacy one is reached by reflection because the sdk does not reference it.
    ///     </para>
    ///     Priority: <see cref="TestSdkSettings.ApiVersionReader" />, then the reader of the
    ///     Asp.Versioning registration, then the reader of the legacy registration. An empty result
    ///     means "unknown" - the resolver then falls back to the common names.
    /// </summary>
    internal static class ApiVersionParameterDetector
    {
        private const string LegacyAssembly = "Microsoft.AspNetCore.Mvc.Versioning";
        private const string LegacyReaderType = "Microsoft.AspNetCore.Mvc.Versioning.IApiVersionReader, " + LegacyAssembly;
        private const string LegacyOptionsType = "Microsoft.AspNetCore.Mvc.Versioning.ApiVersioningOptions, " + LegacyAssembly;

        public static ImmutableList<ApiVersionParameter> Detect(IServiceProvider? serviceProvider,
                                                                TestSdkSettings testSdkSettings)
        {
            // An explicit reader in the test settings always wins
            if (testSdkSettings.ApiVersionReader.IsNotNull())
            {
                return Describe(testSdkSettings.ApiVersionReader, testSdkSettings);
            }

            if (serviceProvider.IsNull())
            {
                return ImmutableList<ApiVersionParameter>.Empty;
            }

            foreach (var reader in FindReaders(serviceProvider, testSdkSettings))
            {
                var parameters = Describe(reader, testSdkSettings);

                if (parameters.Any())
                {
                    return parameters;
                }
            }

            return ImmutableList<ApiVersionParameter>.Empty;
        }

        /// <summary>
        ///     Reports every parameter an ApiVersionReader of either package declares.
        /// </summary>
        internal static ImmutableList<ApiVersionParameter> Describe(object reader,
                                                                    TestSdkSettings testSdkSettings)
        {
            try
            {
                var parameterSource = reader.GetType()
                                            .GetInterfaces()
                                            .FirstOrDefault(i => i.Name == "IApiVersionParameterSource");

                var addParameters = parameterSource?.GetMethod("AddParameters");
                var contextType = addParameters?.GetParameters().SingleOrDefault()?.ParameterType;

                if (addParameters.IsNull() || contextType.IsNull() || contextType.IsInterface.IsFalse())
                {
                    return ImmutableList<ApiVersionParameter>.Empty;
                }

                // The context interface differs per package - a proxy implements whichever one it is
                var context = DispatchProxy.Create(contextType, typeof(ParameterCollector));
                addParameters.Invoke(reader, [context]);

                return ((ParameterCollector)context).Parameters
                                                    .Distinct()
                                                    .ToImmutableList();
            }
#pragma warning disable CA1031 // Detection must never break a test - unknown simply means "use the common names"
            catch (Exception exception)
#pragma warning restore CA1031
            {
                testSdkSettings.LogAction($"Api version parameters of '{reader.GetType().FullName}' could not be detected, the common names are used instead: {exception.Message}");

                return ImmutableList<ApiVersionParameter>.Empty;
            }
        }

        private static IEnumerable<object> FindReaders(IServiceProvider serviceProvider,
                                                       TestSdkSettings testSdkSettings)
        {
            // Asp.Versioning registers the configured reader as service (AddApiVersioning)
            var reader = TryGet(() => serviceProvider.GetService<IApiVersionReader>(), testSdkSettings);

            if (reader.IsNotNull())
            {
                yield return reader;
            }

            // Legacy Microsoft.AspNetCore.Mvc.Versioning - only present when the api references it
            var legacyReaderType = Type.GetType(LegacyReaderType, throwOnError: false);

            if (legacyReaderType.IsNull())
            {
                yield break;
            }

            var legacyReader = TryGet(() => serviceProvider.GetService(legacyReaderType), testSdkSettings)
                               ?? TryGet(() => GetLegacyReaderFromOptions(serviceProvider), testSdkSettings);

            if (legacyReader.IsNotNull())
            {
                yield return legacyReader;
            }
        }

        private static object? GetLegacyReaderFromOptions(IServiceProvider serviceProvider)
        {
            var optionsType = Type.GetType(LegacyOptionsType, throwOnError: false);

            if (optionsType.IsNull())
            {
                return null;
            }

            var options = serviceProvider.GetService(typeof(IOptions<>).MakeGenericType(optionsType));
            var value = options?.GetType().GetProperty(nameof(IOptions<object>.Value))?.GetValue(options);

            return value?.GetType().GetProperty("ApiVersionReader")?.GetValue(value);
        }

        private static object? TryGet(Func<object?> resolve,
                                      TestSdkSettings testSdkSettings)
        {
            try
            {
                return resolve();
            }
#pragma warning disable CA1031 // Detection must never break a test
            catch (Exception exception)
#pragma warning restore CA1031
            {
                testSdkSettings.LogAction($"Api version reader could not be resolved: {exception.Message}");

                return null;
            }
        }
    }

    /// <summary>
    ///     Stands in for IApiVersionParameterDescriptionContext of either versioning package and
    ///     records every AddParameter(name, location) call.
    /// </summary>
#pragma warning disable CA1852 // DispatchProxy derives from it at runtime
    internal class ParameterCollector : DispatchProxy
#pragma warning restore CA1852
    {
        public List<ApiVersionParameter> Parameters { get; } = [];

        protected override object? Invoke(MethodInfo? targetMethod,
                                          object?[]? args)
        {
            if (targetMethod?.Name == "AddParameter" &&
                args is [string name, { } location] &&
                name.IsNotNullOrWhiteSpace() &&
                Enum.TryParse<ApiVersionParameterKind>(location.ToString(), out var kind))
            {
                Parameters.Add(new ApiVersionParameter(name, kind));
            }

            return null;
        }
    }
}
