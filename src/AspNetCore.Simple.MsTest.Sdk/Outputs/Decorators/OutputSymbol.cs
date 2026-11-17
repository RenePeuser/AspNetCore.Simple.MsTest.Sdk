using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Every symbol the sdk's output uses. Output code names the MEANING, the render strategy decides
    /// the glyph - an emoji for <see cref="OutputRendering.Graphics"/>, plain ASCII (or nothing) for
    /// <see cref="OutputRendering.PlainText"/>. A literal emoji in an output builder bypasses that choice.
    /// </summary>
    internal enum OutputSymbol
    {
        /// <summary>❌ - a failure header, an invalid input.</summary>
        Failure,

        /// <summary>⚠️ - problem and failure details.</summary>
        Warning,

        /// <summary>✅ - a suggested fix, a valid input.</summary>
        Success,

        /// <summary>✓ - a matching row in a comparison table.</summary>
        Match,

        /// <summary>✗ - a mismatching row in a comparison table.</summary>
        Mismatch,

        /// <summary>❌ - points at the failing line of a listing.</summary>
        Pointer,

        /// <summary>📦 - test or file information.</summary>
        Information,

        /// <summary>🌍 - the http request and response.</summary>
        Http,

        /// <summary>📄 - a json document, a file, a response body.</summary>
        Document,

        /// <summary>💡 - an explanation or a hint.</summary>
        Hint,

        /// <summary>📝 - the assert call.</summary>
        AssertCall,

        /// <summary>🔍 - differences, matches, type validation.</summary>
        Search,

        /// <summary>📊 - details, a comparison.</summary>
        Details,

        /// <summary>💭 - the context of an assert.</summary>
        Context,

        /// <summary>📋 - a schema mismatch, a detail list.</summary>
        Clipboard,

        /// <summary>📸 - a snapshot mismatch.</summary>
        Snapshot,

        /// <summary>🚫 - an unexpected status code, a json type mismatch.</summary>
        Blocked,

        /// <summary>🔧 - what to do, an endpoint registration.</summary>
        Tool,

        /// <summary>🪤 - common traps.</summary>
        Trap,

        /// <summary>🚦 - a status code.</summary>
        StatusCode,

        /// <summary>🔎 - read the stack trace.</summary>
        Inspect,

        /// <summary>🔁 - reproduce locally.</summary>
        Reproduce,

        /// <summary>✍️ - creating a new snapshot.</summary>
        Write,

        /// <summary>🌐 - an external api.</summary>
        External
    }

    internal static class OutputSymbols
    {
        public static string Emoji(OutputSymbol symbol)
        {
            return symbol switch
            {
                OutputSymbol.Failure => "❌",
                OutputSymbol.Warning => "⚠️",
                OutputSymbol.Success => "✅",
                OutputSymbol.Match => "✓",
                OutputSymbol.Mismatch => "✗",
                OutputSymbol.Pointer => "❌",
                OutputSymbol.Information => "📦",
                OutputSymbol.Http => "🌍",
                OutputSymbol.Document => "📄",
                OutputSymbol.Hint => "💡",
                OutputSymbol.AssertCall => "📝",
                OutputSymbol.Search => "🔍",
                OutputSymbol.Details => "📊",
                OutputSymbol.Context => "💭",
                OutputSymbol.Clipboard => "📋",
                OutputSymbol.Snapshot => "📸",
                OutputSymbol.Blocked => "🚫",
                OutputSymbol.Tool => "🔧",
                OutputSymbol.Trap => "🪤",
                OutputSymbol.StatusCode => "🚦",
                OutputSymbol.Inspect => "🔎",
                OutputSymbol.Reproduce => "🔁",
                OutputSymbol.Write => "✍️",
                OutputSymbol.External => "🌐",
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, "Every output symbol needs an emoji.")
            };
        }

        /// <summary>
        /// The plain text stand-in. Only symbols that carry information on their own (a match column,
        /// a line pointer) get one - a symbol in front of a title is decoration and is dropped there,
        /// see <see cref="TextRenderStrategy"/>.
        /// </summary>
        public static string Ascii(OutputSymbol symbol)
        {
            return symbol switch
            {
                OutputSymbol.Failure => "[X]",
                OutputSymbol.Warning => "[!]",
                OutputSymbol.Success => "[OK]",
                OutputSymbol.Match => "yes",
                OutputSymbol.Mismatch => "no",
                OutputSymbol.Pointer => ">>",
                OutputSymbol.Information
                    or OutputSymbol.Http
                    or OutputSymbol.Document
                    or OutputSymbol.Hint
                    or OutputSymbol.AssertCall
                    or OutputSymbol.Search
                    or OutputSymbol.Details
                    or OutputSymbol.Context
                    or OutputSymbol.Clipboard
                    or OutputSymbol.Snapshot
                    or OutputSymbol.Blocked
                    or OutputSymbol.Tool
                    or OutputSymbol.Trap
                    or OutputSymbol.StatusCode
                    or OutputSymbol.Inspect
                    or OutputSymbol.Reproduce
                    or OutputSymbol.Write
                    or OutputSymbol.External => "[i]",
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, "Every output symbol needs an ascii stand-in.")
            };
        }
    }
}
