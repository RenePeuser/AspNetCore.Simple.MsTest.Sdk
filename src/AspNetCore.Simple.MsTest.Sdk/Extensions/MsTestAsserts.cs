using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static class MsTestAsserts
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input);
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<T>> object1, Expression<Func<T>> object2, Func<T, T> orderFunc) where T : class
        {
            var obj1 = orderFunc(object1.Compile()());
            var obj2 = orderFunc(object2.Compile()());

            var differences = new Comparer<T>().CalculateDifferences(obj1, obj2);

            Assert.IsTrue(differences.IsEmpty(), differences.ToResultTable(object1.NameOf(), object2.NameOf()));
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> objectExpression) where T : class
        {
            var object1 = JsonSerializer.Deserialize<T>(json.Compile()(), JsonSerializerOptions);
            var object2 = objectExpression.Compile()();
            var differences = new Comparer<T>(new ComparisonSettings()).CalculateDifferences(object1!, object2);

            Assert.IsTrue(differences.IsEmpty(), differences.ToResultTable(json.NameOf(), objectExpression.NameOf()));
        }

        public static void ObjectsAreEqual<T>(this Assert assert, Expression<Func<string>> json, Expression<Func<T>> object2, Func<T, T> orderFunc) where T : class
        {
            var object1 = JsonSerializer.Deserialize<T>(json.Compile()(), JsonSerializerOptions);
            var obj2 = object2.Compile()();
            var orderedObject1 = orderFunc(object1);
            var orderedObject2 = orderFunc(obj2);

            var differences = new Comparer<T>(new ComparisonSettings()).CalculateDifferences(orderedObject1, orderedObject2);

            Assert.IsTrue(differences.IsEmpty(), differences.ToResultTable(json.NameOf(), object2.NameOf()));
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
