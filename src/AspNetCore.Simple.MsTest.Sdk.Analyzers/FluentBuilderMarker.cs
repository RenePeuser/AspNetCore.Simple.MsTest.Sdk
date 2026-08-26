using System.Linq;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers
{
    /// <summary>
    ///     Shared recognition of the <c>[FluentBuilder]</c> marker for both fluent-chain diagnostics
    ///     (MSTESTSDK001 dangling chain, MSTESTSDK002 unawaited terminal).
    ///     <para>
    ///         The marker is matched by its full display name rather than by a symbol reference, so the
    ///         analyzers stay free of a compile-time dependency on the SDK assembly.
    ///     </para>
    /// </summary>
    internal static class FluentBuilderMarker
    {
        /// <summary>The terminal every fluent chain must end with.</summary>
        internal const string TerminalMethodName = "ExecuteAsync";

        private const string FluentBuilderAttributeName =
            "AspNetCore.Simple.MsTest.Sdk.FluentAssertions.FluentBuilderAttribute";

        /// <summary>
        ///     True when the type itself carries <c>[FluentBuilder]</c>, or implements an interface that
        ///     does — the concrete builders are internal and reached through their marked interfaces.
        /// </summary>
        internal static bool CarriesFluentBuilderAttribute(ITypeSymbol? type)
        {
            if (type is null)
            {
                return false;
            }

            return HasAttribute(type) || type.AllInterfaces.Any(HasAttribute);
        }

        private static bool HasAttribute(ISymbol symbol)
        {
            return symbol.GetAttributes()
                         .Any(a => a.AttributeClass?.ToDisplayString() == FluentBuilderAttributeName);
        }
    }
}
