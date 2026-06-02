using System;
using System.Linq.Expressions;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class ExpressionExtensions
    {
        internal static string NameOf(this Expression expression)
        {
            if (expression is not LambdaExpression lambdaExpression)
            {
                throw new ArgumentException("Expression is not a LambdaExpression");
            }

            string? name = null;

            if (lambdaExpression.Body is MemberExpression memberExpression)
            {
                name = memberExpression.Member.Name;
            }

            if (lambdaExpression.Body is UnaryExpression { Operand: MemberExpression member })
            {
                name = member.Member.Name;
            }

            if (lambdaExpression.Body is MethodCallExpression methodCallExpression)
            {
                name = methodCallExpression.Method.Name;
            }

            if (name.IsNull())
            {
                throw new ArgumentException("Unknown expression type for extracting name.", nameof(expression));
            }

            return name;
        }
    }
}