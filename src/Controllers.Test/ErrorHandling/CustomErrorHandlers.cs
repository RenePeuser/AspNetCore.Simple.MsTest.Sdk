using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;

namespace Controllers.Test.ErrorHandling
{
    /// <summary>A failure only this project knows how to explain.</summary>
    public sealed class DomainRuleViolatedException : Exception
    {
        public DomainRuleViolatedException()
        {
        }

        public DomainRuleViolatedException(string message)
            : base(message)
        {
        }

        public DomainRuleViolatedException(string message,
                                           Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>A failure the <see cref="SilentErrorHandler" /> claims but has nothing to say about.</summary>
    public sealed class NothingToSayException : Exception
    {
        public NothingToSayException()
        {
        }

        public NothingToSayException(string message)
            : base(message)
        {
        }

        public NothingToSayException(string message,
                                     Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// A consumer's own error handler - the public extension point. Registered in <see cref="ApiTestBase" />
    /// AFTER AddAssertableHttpClient, so it also proves the catch-all does not win just by coming first.
    /// </summary>
    public sealed class DomainRuleErrorHandler : TestErrorHandler<DomainRuleViolatedException>
    {
        public const string Heading = "DOMAIN RULE VIOLATED";

        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             DomainRuleViolatedException exception)
        {
            return Task.FromResult($"{Heading}: {exception.Message}");
        }
    }

    /// <summary>
    /// Claims <see cref="NothingToSayException" /> and answers with nothing - the next compatible handler
    /// has to take over instead of the bare fallback.
    /// </summary>
    public sealed class SilentErrorHandler : TestErrorHandler<NothingToSayException>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             NothingToSayException exception)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
