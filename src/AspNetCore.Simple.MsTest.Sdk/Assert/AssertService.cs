using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAssertServiceExtension
    {
        /// <summary>
        /// Registers all assertion services and their dependencies in the DI container.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddAssertService(this IServiceCollection services)
        {
            // 1. Register text decorator (conditional on build configuration)
#if DEBUG
            services.AddPlainTextDecorator(); // No colors for Visual Studio Test Explorer
#else
            services.AddAnsiColorTextDecorator(); // Colors for CI/terminal
#endif

            // 2. Register all dependencies via their own extensions
            services.AddPrimitiveTypeConverter();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddJsonSerializer();

            // 3. Register comparison strategies (extensible system)
            services.AddComparisonStrategy();

            // 4. Register output strategies
            services.AddPrimitiveOutputStrategy();
            services.AddObjectOutputStrategy();
            services.AddHttpResponseOutputStrategy();

            // 5. Register output builder
            services.AddAssertOutputBuilder();

            // Note: IEmbeddedFileLocalizer registration requires IConfiguration and should be done at app startup

            // 6. Register the service itself
            services.AddSingletonIfNotExists<IAssertService, AssertService>();
        }
    }

    /// <summary>
    /// Represents an assertion service for comparing objects in tests.
    /// Core service that performs deep object comparisons with automatic diff generation.
    /// </summary>
    public interface IAssertService
    {
        /// <summary>
        /// Compares two objects and asserts they are equal using the provided context configuration.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">The assertion context containing all comparison parameters.</param>
        void ObjectsAreEqual<T>(ObjectAssertContext<T> context);
    }

    /// <summary>
    /// Implementation of <see cref="IAssertService"/> that contains the core assertion logic.
    /// This class encapsulates all dependencies needed for object assertions.
    /// All dependencies are injected via the primary constructor for testability and flexibility.
    /// </summary>
    internal sealed class AssertService(IComparisonStrategy comparisonStrategy,
                                        IResponseWriter responseWriter,
                                        IWriteResponseService writeResponseService,
                                        IAssertOutputBuilder outputBuilder) : IAssertService
    {
        public void ObjectsAreEqual<T>(ObjectAssertContext<T> context)
        {
            // 1. Use comparison strategy to perform type-specific comparison
            var result = comparisonStrategy.Compare(context);

            // 2. Serialize current object for writing (if needed)
            var currentFormatted = result.FormattedCurrent;

            // 3. Write response if configured
            if (writeResponseService.ShouldWriteResponse(context))
            {
                responseWriter.Write(context, currentFormatted, context.ExpectedResultFile);
            }

            // 4. Assert schema matches or values match
            if (result.HasSchemaMismatch || result.Differences.Any())
            {
                // Build comprehensive output using the comparison result
                var error = outputBuilder.BuildOutput(context,
                                                      result.Differences,
                                                      result.FormattedExpected,
                                                      result.FormattedCurrent);

                Assert.That.Fail(error);
            }
        }
    }
}
