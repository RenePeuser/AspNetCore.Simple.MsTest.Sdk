using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ConsoleTables;
using Extensions.Pack;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class DeserializeException : Exception
    {
        public DeserializeException(string message) : base(message)
        {
        }
    }

    public static class DifferenceExtensions
    {
        public static string ToResultTable(this IImmutableList<Difference> differences, string objectName1, string objectName2)
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

    public record CompareResult(bool WasEqual, string Differences);

    public class ObjectComparer
    {
        // Hint NUnit will set prefix "because" !!
        private const string DefaultTitle = "objects are not equal";

        public CompareResult Compare<T>(Expression<Func<T>> object1, Expression<Func<T>> object2)
        {
            return Compare(object1, object2, input => input, DefaultTitle);
        }

        public CompareResult Compare<T>(Expression<Func<T>> object1, Expression<Func<T>> object2, string title)
        {
            return Compare(object1, object2, input => input, title);
        }

        public CompareResult Compare<T>(Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc)
        {
            return Compare(object1, object2, orderFunc, DefaultTitle);
        }


        public CompareResult Compare<T>(Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc, string title)
        {
            return Compare(object1, object2, orderFunc, title, _ => true);
        }

        public CompareResult Compare<T>(Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc, string title, Func<Difference, bool> filter)
        {
            var obj1 = orderFunc(object1.Compile()());
            var obj2 = orderFunc(object2.Compile()());

            var comparer = new Comparer<T>();
            var differences = comparer.CalculateDifferences(obj1, obj2).ToImmutableList();
            var filteredDifferences = differences.Where(filter).ToImmutableList();
            var resultTable = filteredDifferences.ToResultTable(object1.NameOf(), object2.NameOf());

            var differencesAsString = GetOutputString(resultTable, obj1, obj2, title);

            if (filteredDifferences.Any())
            {
                return new CompareResult(false, differencesAsString);
            }

            return new CompareResult(true, string.Empty);
        }

        public CompareResult Compare<T>(Expression<Func<string>> json, Expression<Func<T>> objectExpression)
        {
            return Compare(json, objectExpression, item => item);
        }

        public CompareResult Compare<T>(Expression<Func<string>> json, Expression<Func<T>> objectExpression, string title)
        {
            return Compare(json, objectExpression, item => item, title);
        }

        public CompareResult Compare<T>(Expression<Func<string>> json, Expression<Func<T>> object2, Func<T, T> orderFunc)
        {
            return Compare(json, object2, orderFunc, string.Empty);
        }

        public CompareResult Compare<T>(Expression<Func<string>> json, Expression<Func<T>> object2, Func<T, T> orderFunc, string title)
        {
            T object1;
            var object1AsJson = json.Compile()();
            try
            {
                // Can be null for comparison!
                object1 = object1AsJson.FromJsonStringAs<T>()!;
            }
            catch (Exception)
            {
                throw new DeserializeException($"The given json for: '{json.NameOf()}' was not possible to convert into type: {typeof(T).Name}. Json was:{Environment.NewLine}{Environment.NewLine}{object1AsJson}");
            }

            var obj2 = object2.Compile()();
            var orderedObject1 = orderFunc(object1);
            var orderedObject2 = orderFunc(obj2);

            var differences = new Comparer<T>(new ComparisonSettings()).CalculateDifferences(orderedObject1, orderedObject2).ToImmutableList();

            if (differences.IsEmpty())
            {
                return new CompareResult(true, string.Empty);
            }

            var resultTable = differences.ToResultTable(json.NameOf(), object2.NameOf());

            var outputString = GetOutputString(resultTable, orderedObject1, orderedObject2, title);

            return new CompareResult(false, outputString);
        }

        private static string GetOutputString(string resultTable, object? expectedResult, object? current, string title)
        {
            var expectedResultAsJson = expectedResult.ToJson();
            var currentResultAsJson = current.ToJson();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            if (title.IsNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(resultTable);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(currentResultAsJson);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(expectedResultAsJson);
            return stringBuilder.ToString();
        }
    }
}
