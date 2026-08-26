using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    internal static class AddNonSeekableBodyErrorHandlerExtension
    {
        public static void AddNonSeekableBodyErrorHandler(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITestErrorHandler, NonSeekableBodyErrorHandler>();
        }
    }

    /// <summary>
    /// "The stream is not seekable" means something tried to rewind the request body without the body
    /// having been buffered. It is nearly always application middleware - typically an error handler
    /// re-reading the request to enrich its output - and NOT a defect of this sdk, which is what the
    /// generic handler used to claim.
    ///
    /// The important part of the message is the second-order effect: this exception is thrown from
    /// inside error handling, so it REPLACES whatever the endpoint originally failed with.
    /// </summary>
    internal sealed class NonSeekableBodyErrorHandler(ITextDecorator textDecorator)
        : TestErrorHandler<NotSupportedException>
    {
        protected override bool CanHandle(NotSupportedException exception)
        {
            return IsSeekFailure(exception);
        }

        /// <summary>
        /// Matches on the seek capability rather than the message text alone: the stack carries the
        /// Position setter or a Seek call when this is really a rewind attempt.
        /// </summary>
        private static bool IsSeekFailure(NotSupportedException exception)
        {
            var stack = exception.StackTrace ?? string.Empty;

            return stack.Contains("set_Position", StringComparison.Ordinal) ||
                   stack.Contains(".Seek(", StringComparison.Ordinal) ||
                   exception.Message.Contains("not seekable", StringComparison.OrdinalIgnoreCase);
        }

        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             NotSupportedException exception)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ REQUEST BODY WAS READ TWICE WITHOUT BUFFERING"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🌍 HTTP"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"Url",-10} : {context.Url}");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ What Happened"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("Something rewound the request body, but the body was never buffered.");
            sb.AppendLine("A request stream can only be read once unless Request.EnableBuffering() ran for it.");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("This exception comes from YOUR application pipeline, not from the test sdk."));
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🔎 Read The Stack Trace Below"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("The frame directly above the stream access names the culprit. If it sits in error");
            sb.AppendLine($"handling middleware, then {textDecorator.Error("the endpoint had ALREADY failed")} and this exception replaced");
            sb.AppendLine("the original one - fix the buffering first, then look at the real failure.");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("✅ How To Fix"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("1. Enable buffering for every request that carries a body. Register this as early");
            sb.AppendLine("   as possible - before error handling, logging and validation:");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("     public sealed class EnableRequestBufferingMiddleware : IMiddleware"));
            sb.AppendLine(textDecorator.Success("     {"));
            sb.AppendLine(textDecorator.Success("         public async Task InvokeAsync(HttpContext context, RequestDelegate next)"));
            sb.AppendLine(textDecorator.Success("         {"));
            sb.AppendLine(textDecorator.Success("             if (context.Request.ContentLength > 0 ||"));
            sb.AppendLine(textDecorator.Success("                 context.Request.Headers.ContainsKey(\"Transfer-Encoding\"))"));
            sb.AppendLine(textDecorator.Success("             {"));
            sb.AppendLine(textDecorator.Success("                 context.Request.EnableBuffering();"));
            sb.AppendLine(textDecorator.Success("             }"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("             await next(context).ConfigureAwait(false);"));
            sb.AppendLine(textDecorator.Success("         }"));
            sb.AppendLine(textDecorator.Success("     }"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("     // Startup"));
            sb.AppendLine(textDecorator.Success("     services.AddSingleton<EnableRequestBufferingMiddleware>();"));
            sb.AppendLine(textDecorator.Success("     app.UseMiddleware<EnableRequestBufferingMiddleware>();"));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("2. And make every reader tolerant - a component that only enriches output must");
            sb.AppendLine("   never throw and hide the real error:");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("     if (context.Request.Body.CanSeek)"));
            sb.AppendLine(textDecorator.Success("     {"));
            sb.AppendLine(textDecorator.Success("         context.Request.Body.Position = 0;"));
            sb.AppendLine(textDecorator.Success("     }"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.SectionTitle("🪤 Common Traps"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("  • Buffering only when the endpoint declares metadata (AcceptsMetadata from");
            sb.AppendLine("    [Consumes] / Accepts<T>()) silently skips every endpoint without it.");
            sb.AppendLine("  • Matching the content type exactly against \"application/json\" misses the");
            sb.AppendLine("    +json suffixes - PATCH is sent as application/merge-patch+json.");
            sb.AppendLine("  • Buffering after the component that reads the body is too late.");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Exception"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Type",-10} : {exception.GetType().Name}");
            sb.AppendLine($"{"Message",-10} : {exception.Message}");
            sb.AppendLine();

            if (exception.StackTrace.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(exception.StackTrace);
                sb.AppendLine();
            }

            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));

            return Task.FromResult(sb.ToString());
        }
    }
}
