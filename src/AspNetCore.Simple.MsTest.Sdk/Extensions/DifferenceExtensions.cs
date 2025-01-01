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

            if (differences.Count == 1 &&
                differences[0].MemberPath.IsNullOrWhiteSpace())
            {
                var tableWithoutMemberPath = new ConsoleTable(objectName1, objectName2) { Options = { EnableCount = false } };
                differences.ForEach(dif => tableWithoutMemberPath.AddRow(dif.Value1, dif.Value2));
                return tableWithoutMemberPath.ToString();
            }

            var table = new ConsoleTable(nameof(Difference.MemberPath), objectName1, objectName2) { Options = { EnableCount = false } };
            differences.ForEach(dif => table.AddRow(dif.MemberPath, dif.Value1, dif.Value2));
            return table.ToString();
        }
    }
}
