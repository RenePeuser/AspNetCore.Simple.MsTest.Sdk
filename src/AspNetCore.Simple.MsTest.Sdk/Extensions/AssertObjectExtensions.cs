using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static class AssertObjectExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, string.Empty);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2, string title) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, title);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc, string title) where T : class
        {
            var obj1 = orderFunc(object1.Compile()());
            var obj2 = orderFunc(object2.Compile()());

            var differences = new Comparer<T>().CalculateDifferences(obj1, obj2);
            var resultTable = differences.ToResultTable(object1.NameOf(), object2.NameOf());

            Assert.IsTrue(differences.IsEmpty(), GetOutputString(resultTable, obj2, title));
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> objectExpression) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> objectExpression, string title) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, title);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> object2, Func<T, T> orderFunc) where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, string.Empty);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> object2, Func<T, T> orderFunc, string title) where T : class
        {
            T object1;
            var object1AsJson = json.Compile()();
            try
            {
                object1 = JsonSerializer.Deserialize<T>(object1AsJson, JsonSerializerOptions);
            }
            catch (Exception)
            {
                throw new DeserializeException($"The given json for: '{json.NameOf()}' was not possible to convert into type: {typeof(T).Name}. Json was:{Environment.NewLine}{Environment.NewLine}{object1AsJson}");
            }

            var obj2 = object2.Compile()();
            var orderedObject1 = orderFunc(object1);
            var orderedObject2 = orderFunc(obj2);

            var differences = new Comparer<T>(new ComparisonSettings()).CalculateDifferences(orderedObject1, orderedObject2);

            var resultTable = differences.ToResultTable(json.NameOf(), object2.NameOf());

            Assert.IsTrue(differences.IsEmpty(), GetOutputString(resultTable, obj2, title));
        }

        private static string GetOutputString(string resultTable, object responseObject, string title)
        {
            var responseJson = responseObject.ToJson();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();

            if (!string.IsNullOrWhiteSpace(title))
            {
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(resultTable);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current response was:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(responseJson);
            return stringBuilder.ToString();
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
