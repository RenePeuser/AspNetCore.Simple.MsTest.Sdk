using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Tables
{
    public static class AddTableBuilderExtension
    {
        public static void AddTableBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITableBuilder, TableBuilder>();
        }
    }

    /// <summary>
    /// Service for building ASCII tables with customizable columns and rows.
    /// Replaces ConsoleTables package with a lightweight, dependency-free implementation.
    /// </summary>
    public interface ITableBuilder
    {
        /// <summary>
        /// Builds an ASCII table from column headers and row data.
        /// </summary>
        /// <param name="columns">Column headers</param>
        /// <param name="rows">Data rows (each row must have same number of items as columns)</param>
        /// <param name="enableCount">Whether to show row count (default: true)</param>
        /// <returns>Formatted ASCII table string</returns>
        string BuildTable(string[] columns,
                          IReadOnlyList<object[]> rows,
                          bool enableCount = true);

        /// <summary>
        /// Builds an ASCII table from a collection of objects.
        /// Uses reflection to extract properties as columns.
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <param name="objects">Collection of objects to display</param>
        /// <param name="enableCount">Whether to show row count (default: true)</param>
        /// <returns>Formatted ASCII table string</returns>
        string BuildTableFrom<T>(IEnumerable<T> objects,
                                 bool enableCount = true);
    }

    internal sealed partial class TableBuilder : ITableBuilder
    {
        // Regex to match ANSI escape codes
        [GeneratedRegex(@"\x1b\[[0-9;]*m", RegexOptions.Compiled)]
        private static partial Regex AnsiEscapeCodeRegex();

        public string BuildTable(string[] columns,
                                 IReadOnlyList<object[]> rows,
                                 bool enableCount = true)
        {
            if (columns.IsNullOrEmpty())
            {
                return string.Empty;
            }

            // Calculate column widths
            var columnWidths = CalculateColumnWidths(columns, rows);

            var stringBuilder = new StringBuilder();

            // Build top border (┌─┬─┐)
            stringBuilder.AppendLine(BuildTopBorder(columnWidths));

            // Build header row
            stringBuilder.AppendLine(BuildRow(columns.Select(c => (object)c).ToArray(), columnWidths));

            // Build middle separator (├─┼─┤)
            stringBuilder.AppendLine(BuildMiddleSeparator(columnWidths));

            // Build data rows
            foreach (var row in rows)
            {
                stringBuilder.AppendLine(BuildRow(row, columnWidths));
            }

            // Build bottom border (└─┴─┘)
            stringBuilder.Append(BuildBottomBorder(columnWidths));

            // Add count if enabled
            if (enableCount && rows.Count > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append($"\nCount: {rows.Count}");
            }

            return stringBuilder.ToString();
        }

        public string BuildTableFrom<T>(IEnumerable<T> objects,
                                        bool enableCount = true)
        {
            var objectList = objects.ToList();

            if (!objectList.Any())
            {
                return string.Empty;
            }

            // Get properties using reflection
            var properties = typeof(T).GetProperties();

            if (properties.Length == 0)
            {
                return string.Empty;
            }

            // Extract column headers
            var columns = properties.Select(p => p.Name).ToArray();

            // Extract rows
            var rows = new List<object[]>();

            foreach (var obj in objectList)
            {
                var row = properties.Select(p => p.GetValue(obj) ?? "null").ToArray();
                rows.Add(row);
            }

            return BuildTable(columns, rows, enableCount);
        }

        private static int[] CalculateColumnWidths(string[] columns,
                                                   IReadOnlyList<object[]> rows)
        {
            var columnCount = columns.Length;
            var widths = new int[columnCount];

            // Initialize with column header widths
            for (var i = 0; i < columnCount; i++)
            {
                widths[i] = GetVisibleLength(columns[i] ?? string.Empty);
            }

            // Update with data row widths
            foreach (var row in rows)
            {
                for (var i = 0; i < Math.Min(columnCount, row.Length); i++)
                {
                    var cellValue = row[i]?.ToString() ?? string.Empty;
                    var visibleLength = GetVisibleLength(cellValue);
                    widths[i] = Math.Max(widths[i], visibleLength);
                }
            }

            // Add padding (2 spaces per column)
            for (var i = 0; i < columnCount; i++)
            {
                widths[i] += 2;
            }

            return widths;
        }

        /// <summary>
        /// Gets the visible length of a string, excluding ANSI escape codes.
        /// ANSI codes like \x1b[31m (red) are invisible in terminal but count in string length.
        /// </summary>
        private static int GetVisibleLength(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            // Remove ANSI escape codes and get length
            var withoutAnsi = AnsiEscapeCodeRegex().Replace(text, string.Empty);

            return withoutAnsi.Length;
        }

        private static string BuildTopBorder(int[] columnWidths)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('┌');

            for (var i = 0; i < columnWidths.Length; i++)
            {
                stringBuilder.Append(new string('─', columnWidths[i]));

                if (i < columnWidths.Length - 1)
                {
                    stringBuilder.Append('┬');
                }
            }

            stringBuilder.Append('┐');

            return stringBuilder.ToString();
        }

        private static string BuildMiddleSeparator(int[] columnWidths)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('├');

            for (var i = 0; i < columnWidths.Length; i++)
            {
                stringBuilder.Append(new string('─', columnWidths[i]));

                if (i < columnWidths.Length - 1)
                {
                    stringBuilder.Append('┼');
                }
            }

            stringBuilder.Append('┤');

            return stringBuilder.ToString();
        }

        private static string BuildBottomBorder(int[] columnWidths)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('└');

            for (var i = 0; i < columnWidths.Length; i++)
            {
                stringBuilder.Append(new string('─', columnWidths[i]));

                if (i < columnWidths.Length - 1)
                {
                    stringBuilder.Append('┴');
                }
            }

            stringBuilder.Append('┘');

            return stringBuilder.ToString();
        }

        private static string BuildRow(object[] cells,
                                       int[] columnWidths)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("│ ");

            for (var i = 0; i < columnWidths.Length; i++)
            {
                var cellValue = i < cells.Length ? (cells[i]?.ToString() ?? string.Empty) : string.Empty;
                var targetWidth = columnWidths[i] - 2; // Subtract 2 for the padding we added
                var visibleLength = GetVisibleLength(cellValue);
                var paddingNeeded = targetWidth - visibleLength;

                // Append cell value with padding based on visible length
                stringBuilder.Append(cellValue);

                if (paddingNeeded > 0)
                {
                    stringBuilder.Append(new string(' ', paddingNeeded));
                }

                if (i < columnWidths.Length - 1)
                {
                    stringBuilder.Append(" │ ");
                }
                else
                {
                    stringBuilder.Append(" │");
                }
            }

            return stringBuilder.ToString();
        }
    }
}