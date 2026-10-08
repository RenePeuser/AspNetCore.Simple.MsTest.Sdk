using System;
using System.Linq;
using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddAssertServiceExtension
    {
        /// <summary>
        /// Registers all assertion services and their dependencies in the DI container.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddAssertService(this IServiceCollection services,
                                            Assembly? consumerAssembly = null)
        {
            consumerAssembly ??= Assembly.GetCallingAssembly();

            // 1. Register the text decorator bound to the consumer test assembly - the plain vs ANSI
            // decision follows the consumer, never how the sdk itself was compiled.
            services.AddTextDecorator(consumerAssembly);

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

            // 5. Register output builder (unchanged - still needed for Human mode)
            services.AddAssertOutputBuilder();

            // 6. Register output mode infrastructure
            services.AddOutputModeService();
            services.AddAiOutputTransformer();

            // 7. Register output mode render strategies (extensible - consumers can add their own)
            services.AddHumanModeRenderStrategy();
            services.AddAiModeRenderStrategy();
            services.AddHybridModeRenderStrategy();

            // 8. Register output mode renderer (delegates to strategies)
            services.AddOutputModeRenderer();

            // Note: IEmbeddedFileLocalizer registration requires IConfiguration and should be done at app startup

            // 9. Register the service itself (now depends on IOutputModeRenderer)
            services.AddSingletonIfNotExists<IAssertService, AssertService>();
        }
    }

    /// <summary>
    /// Represents an assertion service for comparing objects in tests.
    /// Core service that performs deep object comparisons with automatic diff generation.
    /// </summary>
    internal interface IAssertService
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
                                        IOutputModeRenderer outputModeRenderer) : IAssertService
    {
        public void ObjectsAreEqual<T>(ObjectAssertContext<T> context)
        {
            // 1. Use comparison strategy to perform type-specific comparison
            var result = comparisonStrategy.Compare(context);

            // 2. Serialize current object for writing (if needed)
            var currentFormatted = result.FormattedCurrent;

            // 3. Write response if configured
            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context);

            if (shouldWriteResponse)
            {
                // Determine ResponseWriteMode based on context
                SdkTrace.WriteLine($"[AssertService] IsEmptyAnonymousObjectForCodeGeneration={context.IsEmptyAnonymousObjectForCodeGeneration}, IsDebug={context.CallingAssembly.IsCompiledInDebug()}");

                // C# code generation is only valid for inline expectations (empty anonymous object).
                // Snapshot based asserts (*.json) must stay on the file modes, otherwise the C# writer
                // and a json writer would both claim the request and ResponseWriter throws.
                var isDebugMode = context.CallingAssembly.IsCompiledInDebug();

                // Mirrors CSharpObjectResponseWriter.CanHandle. Without the *.json guard a snapshot
                // could end up in a mode no writer accepts, and ResponseWriter would silently no-op.
                var expectationIsSnapshotFile = context.ExpectedResultFile
                                                       .EmbeddedFileName
                                                       .EndsWith(".json", StringComparison.OrdinalIgnoreCase);

                var mode = isDebugMode && context.IsEmptyAnonymousObjectForCodeGeneration && expectationIsSnapshotFile.IsFalse()
                               ? ResponseWriteMode.GenerateCSharpObject
                               : (context.ExpectedResultFile.EmbeddedFile?.Exists ?? false)
                                   ? ResponseWriteMode.DifferencesOnly
                                   : ResponseWriteMode.OverwriteAll;

                SdkTrace.WriteLine($"[AssertService] Determined Mode={mode}");

                responseWriter.Write(context, currentFormatted, context.ExpectedResultFile,
                                     mode);
            }

            // 4. No differences -> means all fine or the dev force to ignore all diffs by difference func
            if (result.Differences.IsEmpty)
            {
                return;
            }

            // 5. Assert schema matches or values match
            if (result.HasSchemaMismatch || result.Differences.Any())
            {
                // Set failure type for HTTP response contexts (if not already set)
                if (context is IHttpResponseContext httpContext && httpContext.FailureType == HttpAssertionFailureType.None)
                {
                    httpContext.FailureType = result.HasSchemaMismatch
                                                  ? HttpAssertionFailureType.SchemaMismatch
                                                  : HttpAssertionFailureType.SnapshotMismatch;
                }

                // Build comprehensive output using the comparison result
                // Output mode renderer delegates to human builder (default) or AI transformer based on mode
                var error = outputModeRenderer.Render(context,
                                                      result.Differences,
                                                      result.FormattedExpected,
                                                      result.FormattedCurrent);

                Assert.That.Fail(error);
            }
        }
    }
}