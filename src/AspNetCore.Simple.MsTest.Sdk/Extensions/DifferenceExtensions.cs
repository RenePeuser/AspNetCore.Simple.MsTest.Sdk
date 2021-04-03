using System.Collections.Generic;
using ConsoleTables;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class DifferenceExtensions
    {
        public static string ToResultTable(this IEnumerable<Difference> differences, string objectName1, string objectName2)
        {
            if (differences.IsEmpty())
            {
                return string.Empty;
            }

            var table = new ConsoleTable(nameof(Difference.MemberPath), objectName1, objectName2, nameof(Difference.DifferenceType));
            differences.ForEach(dif => table.AddRow(dif.MemberPath, dif.Value1, dif.Value2, dif.DifferenceType));
            return table.ToString();
        }
    }
}
