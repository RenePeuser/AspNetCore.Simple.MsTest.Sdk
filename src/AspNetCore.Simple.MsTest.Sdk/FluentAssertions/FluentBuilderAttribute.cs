using System;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions
{
    /// <summary>
    /// Marks a fluent-builder interface whose chain MUST be terminated with <c>ExecuteAsync()</c>.
    ///
    /// <para>
    /// The MSTESTSDK001 Roslyn analyzer uses this marker (instead of guessing type names) to detect
    /// a dangling fluent chain — an expression statement whose result type carries this attribute but
    /// is never awaited/executed. For a test SDK that is the worst failure mode: the request is never
    /// sent and the test silently turns GREEN.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FluentBuilderAttribute : Attribute
    {
    }
}