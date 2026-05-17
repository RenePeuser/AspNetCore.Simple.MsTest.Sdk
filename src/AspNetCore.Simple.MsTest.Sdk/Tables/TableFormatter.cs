using System.Collections;

namespace AspNetCore.Simple.MsTest.Sdk.Tables
{
    /// <summary>
    /// Static helper class for quick table formatting.
    /// Provides simplified API similar to ConsoleTables.From()
    /// </summary>
    public static class TableFormatter
    {
        /// <summary>
        /// Creates a table from a collection of objects.
        /// Static method for quick table generation without DI.
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <param name="objects">Collection of objects to display</param>
        /// <returns>Formatted ASCII table string</returns>
        public static string From<T>(IEnumerable<T> objects)
        {
            var tableBuilder = new TableBuilder();
            return tableBuilder.BuildTableFrom(objects, enableCount: false);
        }
    }
}
