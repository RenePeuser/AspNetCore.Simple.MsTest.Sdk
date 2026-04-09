using System.IO;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddSourceCodeExtractorExtension
    {
        public static void AddSourceCodeExtractor(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ISourceCodeExtractor, SourceCodeExtractor>();
        }
    }

    public interface ISourceCodeExtractor
    {
        /// <summary>
        /// Extracts the test call code starting from the given line number.
        /// Reads lines until a semicolon is found at the end of a line.
        /// </summary>
        /// <param name="filePath">Path to the source file</param>
        /// <param name="lineNumber">Line number where the call starts (1-based)</param>
        /// <param name="maxLines">Maximum number of lines to read (default: 10)</param>
        /// <returns>The extracted code or null if file not found</returns>
        string? ExtractCallCode(string filePath,
                                int lineNumber,
                                int maxLines = 10);
    }

    /// <summary>
    /// Extracts source code snippets from test files for error reporting.
    /// </summary>
    internal sealed class SourceCodeExtractor : ISourceCodeExtractor
    {
        public string? ExtractCallCode(string filePath,
                                       int lineNumber,
                                       int maxLines = 10)
        {
            if (File.Exists(filePath).IsFalse())
            {
                return null;
            }

            var lines = File.ReadAllLines(filePath);

            if (lineNumber < 1 || lineNumber > lines.Length)
            {
                return null;
            }

            var extractedLines = new List<string>();
            int? baseIndentation = null;

            // Start at CallerLineNumber, read until line ends with ';'
            for (var i = 0; i < maxLines && (lineNumber - 1 + i) < lines.Length; i++)
            {
                var line = lines[lineNumber - 1 + i]; // -1 because line numbers start at 1

                if (line.IsNullOrWhiteSpace())
                {
                    continue;
                }

                // Calculate base indentation from first non-empty line
                if (baseIndentation.HasValue.IsFalse())
                {
                    baseIndentation = CountLeadingWhitespace(line);
                }

                // Remove base indentation from current line
                var indentToRemove = baseIndentation ?? 0;
                var adjustedLine = RemoveLeadingWhitespace(line, indentToRemove);
                extractedLines.Add(adjustedLine);

                // Stop when line ends with semicolon
                if (line.TrimEnd().EndsWith(';'))
                {
                    break;
                }
            }

            if (extractedLines.Count == 0)
            {
                return null;
            }

            var extractCallCode = string.Join(Environment.NewLine, extractedLines);

            return extractCallCode;
        }

        private static int CountLeadingWhitespace(string line)
        {
            var count = 0;

            foreach (var ch in line)
            {
                if (ch is ' ' or '\t')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private static string RemoveLeadingWhitespace(string line, int count)
        {
            if (count <= 0 || line.Length <= count)
            {
                return line;
            }

            var removed = 0;

            for (var i = 0; i < line.Length && removed < count; i++)
            {
                if (line[i] == ' ' || line[i] == '\t')
                {
                    removed++;
                }
                else
                {
                    break;
                }
            }

            return removed > 0 ? line.Substring(removed) : line;
        }
    }
}
