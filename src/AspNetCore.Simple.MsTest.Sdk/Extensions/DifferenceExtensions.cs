using System.Collections.Immutable;
using System.IO;
using System.Linq;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class DifferenceExtensions
    {
        private static readonly TestCreatorSettings TestCreatorSettings = new TestCreatorSettings();
        
        internal static string ToResultTable(this IImmutableList<Difference> differences,
                                             string objectName1,
                                             string objectName2)
        {
            if (differences.IsEmpty())
            {
                return string.Empty;
            }

            if (AssertObjectExtensions.ResponseFileFullPath.IsFalse())
            {
                var matched = TestCreatorSettings.ResponseFolderName;

                foreach (var legacyResponseFolderName in TestCreatorSettings.LegacyResponseFolderNames)
                {
                    if (objectName1.Contains($".{legacyResponseFolderName}."))
                    {
                        matched = legacyResponseFolderName;
                        break;
                    }
                }

                objectName1 = objectName1.Split($".{matched}.").Last();
            }
            
            if (differences.Count == 1 &&
                differences[0].MemberPath.IsNullOrWhiteSpace())
            {
                var tableWithoutMemberPath = new ConsoleTable(objectName1, objectName2, "MismatchType") { Options = { EnableCount = false } };
                differences.ForEach(dif => tableWithoutMemberPath.AddRow(dif.Value1, dif.Value2, dif.MismatchType));

                return tableWithoutMemberPath.ToString();
            }

            var table = new ConsoleTable(nameof(Difference.MemberPath), objectName1, objectName2, "MismatchType") { Options = { EnableCount = false } };
            differences.ForEach(dif =>
                                {
                                    switch (dif.MismatchType)
                                    {
                                        case MismatchType.ValueDifference:
                                            table.AddRow(dif.MemberPath, dif.Value1, dif.Value2, dif.MismatchType);
                                            break;
                                        case MismatchType.MissingInFirst:
                                            table.AddRow(dif.MemberPath, "Property missing", dif.Value2, dif.MismatchType);
                                            break;
                                        case MismatchType.MissingInSecond:
                                            table.AddRow(dif.MemberPath, dif.Value1, "Property missing", dif.MismatchType);
                                            break;
                                    }
                                });

            var stringTable = table.ToString();
            return stringTable;
        }
    }
}
