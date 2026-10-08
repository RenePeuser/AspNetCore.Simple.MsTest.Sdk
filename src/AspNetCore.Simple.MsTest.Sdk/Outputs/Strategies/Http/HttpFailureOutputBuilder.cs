using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    internal static class AddHttpFailureOutputBuilderExtension
    {
        /// <summary>
        /// Registers the HTTP failure output builder service.
        /// </summary>
        public static void AddHttpFailureOutputBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IHttpFailureOutputBuilder, HttpFailureOutputBuilder>();
        }
    }

    /// <summary>
    /// Orchestrator implementation that coordinates HTTP failure output strategies.
    /// Uses chain-of-responsibility pattern to find the right strategy for each failure type.
    /// </summary>
    internal sealed class HttpFailureOutputBuilder(IEnumerable<IHttpFailureOutputStrategy> strategies,
                                                   DefaultHttpFailureOutputStrategy defaultStrategy) : IHttpFailureOutputBuilder
    {
        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            // Find first strategy that can handle this failure type
            var strategy = strategies.FirstOrDefault(s => s.CanHandle(context.FailureType));

            // Use specific strategy or fallback to default (guaranteed to never be null)
            if (strategy != null)
            {
                strategy.BuildHeader(sb, context);
            }
            else
            {
                defaultStrategy.BuildHeader(sb, context);
            }
        }
    }
}