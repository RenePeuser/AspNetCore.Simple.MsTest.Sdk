using System.Collections.Immutable;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class DifferenceExtensions
    {
        internal static string ToResultTable(this IImmutableList<Difference> differences, string objectName1, string objectName2)
        {
            if (differences.IsEmpty())
            {
                return string.Empty;
            }

            var table = new ConsoleTable(nameof(Difference.MemberPath), objectName1, objectName2);
            differences.ForEach(dif => table.AddRow(dif.MemberPath, dif.Value1, dif.Value2));
            return table.ToString();
        }
    }
}
