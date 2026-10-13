using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// A difference member path the fluent comparison vocabulary (<c>IgnorePaths</c>, <c>IgnoreProperty</c>)
    /// matches against <see cref="Difference.MemberPath"/>.
    ///
    /// <para>
    /// The differ writes paths like <c>id</c>, <c>[0].id</c>, <c>emails[0].emailAddress</c> or
    /// <c>items["key"].value</c> (keyed arrays). A pattern uses the same syntax plus <c>[*]</c> for "any
    /// index". Property names compare case-insensitively, so a naming policy that only changes the casing
    /// cannot turn an ignore into a silent no-op.
    /// </para>
    ///
    /// <para>
    /// A pattern matches its own path AND everything below it: ignoring <c>address</c> also ignores
    /// <c>address.street</c>. Ignoring a property means ignoring its whole value, and a missing nested
    /// object is reported on its children, not on the parent alone.
    /// </para>
    /// </summary>
    internal sealed class MemberPathPattern
    {
        private const string AnyIndex = "[*]";

        // Where the response body sits inside the compared HTTP envelope.
        private static readonly string[] BodyEnvelope = ["content", "value"];

        private readonly ImmutableList<string> _segments;

        private MemberPathPattern(ImmutableList<string> segments)
        {
            _segments = segments;
        }

        public override string ToString()
        {
            return Join(_segments);
        }

        public static MemberPathPattern Parse(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A member path must not be empty. Use the difference path format, e.g. 'id', '[*].id' or 'emails[*].emailAddress'.",
                                            nameof(path));
            }

            return new MemberPathPattern(Split(path));
        }

        /// <summary>
        /// Builds the full path a member access expression addresses, starting at the response root.
        /// Supports member access (<c>p => p.Address.Street</c>), indexers and array access
        /// (<c>p => p.Emails[0].EmailAddress</c>) and <c>Select</c> for "every element"
        /// (<c>p => p.Emails.Select(e => e.EmailAddress)</c>, or <c>list => list.Select(p => p.Id)</c> for a
        /// collection response). JSON names come from <c>[JsonPropertyName]</c> or the naming policy of
        /// <paramref name="options"/> — the same names the differ sees.
        /// </summary>
        public static MemberPathPattern FromExpression(LambdaExpression selector,
                                                       JsonSerializerOptions options)
        {
            var segments = new List<string>();

            AppendSegments(selector.Body, selector.Parameters[0], options, segments);

            if (segments.Count == 0)
            {
                throw new ArgumentException($"The selector '{selector}' selects the response itself, not a property of it.",
                                            nameof(selector));
            }

            return new MemberPathPattern(segments.ToImmutableList());
        }

        /// <summary>
        /// True when <paramref name="memberPath"/> is this pattern's path or lies below it. On the HTTP path
        /// the differ compares the whole response envelope, so a body property shows up as
        /// <c>content.value.name</c> — the pattern is tried against that full path AND against the
        /// body-relative remainder, so <c>"name"</c> and <c>"content.value.name"</c> both work.
        /// </summary>
        public bool Matches(string memberPath)
        {
            var pathSegments = Split(memberPath);

            return MatchesFrom(pathSegments, 0) ||
                   (StartsWithBodyEnvelope(pathSegments) && MatchesFrom(pathSegments, BodyEnvelope.Length));
        }

        private bool MatchesFrom(ImmutableList<string> pathSegments,
                                 int offset)
        {
            if (pathSegments.Count - offset < _segments.Count)
            {
                return false;
            }

            for (var i = 0; i < _segments.Count; i++)
            {
                if (SegmentMatches(_segments[i], pathSegments[offset + i]).IsFalse())
                {
                    return false;
                }
            }

            return true;
        }

        private static bool StartsWithBodyEnvelope(ImmutableList<string> pathSegments)
        {
            return pathSegments.Count > BodyEnvelope.Length &&
                   BodyEnvelope.Select((segment, i) => segment.Equals(pathSegments[i], StringComparison.OrdinalIgnoreCase))
                               .All(matches => matches);
        }

        private static bool SegmentMatches(string patternSegment,
                                           string pathSegment)
        {
            if (patternSegment == AnyIndex)
            {
                return IsIndex(pathSegment);
            }

            return IsIndex(patternSegment)
                       ? patternSegment.Equals(pathSegment, StringComparison.Ordinal)
                       : patternSegment.Equals(pathSegment, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsIndex(string segment)
        {
            return segment.StartsWith('[');
        }

        private static void AppendSegments(Expression expression,
                                           ParameterExpression root,
                                           JsonSerializerOptions options,
                                           List<string> segments)
        {
            expression = StripConvert(expression);

            switch (expression)
            {
                case ParameterExpression parameter when parameter == root:
                    return;

                case MemberExpression { Expression: not null } member:
                    AppendSegments(member.Expression, root, options, segments);
                    segments.Add(JsonName(member.Member, options));

                    return;

                case BinaryExpression { NodeType: ExpressionType.ArrayIndex } arrayIndex:
                    AppendSegments(arrayIndex.Left, root, options, segments);
                    segments.Add($"[{Evaluate(arrayIndex.Right)}]");

                    return;

                case MethodCallExpression { Method.Name: "get_Item", Object: not null, Arguments.Count: 1 } indexer:
                    AppendSegments(indexer.Object, root, options, segments);
                    segments.Add(IndexSegment(Evaluate(indexer.Arguments[0])));

                    return;

                case MethodCallExpression { Method.Name: nameof(Enumerable.Select), Arguments.Count: 2 } select
                    when select.Method.DeclaringType == typeof(Enumerable) &&
                         StripQuote(select.Arguments[1]) is LambdaExpression { Parameters.Count: 1 } elementSelector:
                    AppendSegments(select.Arguments[0], root, options, segments);
                    segments.Add(AnyIndex);
                    AppendSegments(elementSelector.Body, elementSelector.Parameters[0], options, segments);

                    return;

                default:
                    throw new ArgumentException($"'{expression}' is not supported in a property selector. Use member access (r => r.A.B), an index (r => r.Items[0].Id) or Select for every element (r => r.Items.Select(i => i.Id)).");
            }
        }

        private static string IndexSegment(object? index)
        {
            return index is string key
                       ? $"[{JsonSerializer.Serialize(key)}]"
                       : $"[{index}]";
        }

        private static object? Evaluate(Expression expression)
        {
            return expression is ConstantExpression constant
                       ? constant.Value
                       : Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        private static string JsonName(MemberInfo member,
                                       JsonSerializerOptions options)
        {
            var attribute = member.GetCustomAttribute<JsonPropertyNameAttribute>();

            if (attribute != null)
            {
                return attribute.Name;
            }

            return options.PropertyNamingPolicy?.ConvertName(member.Name) ?? member.Name;
        }

        private static Expression StripConvert(Expression expression)
        {
            while (expression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            {
                expression = unary.Operand;
            }

            return expression;
        }

        private static Expression StripQuote(Expression expression)
        {
            return expression is UnaryExpression { NodeType: ExpressionType.Quote } quote
                       ? quote.Operand
                       : expression;
        }

        private static ImmutableList<string> Split(string path)
        {
            var segments = ImmutableList.CreateBuilder<string>();
            var current = new StringBuilder();
            var i = 0;

            while (i < path.Length)
            {
                var c = path[i];

                if (c == '.')
                {
                    Flush();
                    i++;

                    continue;
                }

                if (c == '[')
                {
                    Flush();

                    // Keyed array index: ["some.key"] — the key may itself contain '.' or ']'.
                    var end = i + 1 < path.Length && path[i + 1] == '"'
                                  ? path.IndexOf("\"]", i + 2, StringComparison.Ordinal) + 1
                                  : path.IndexOf(']', i + 1);

                    if (end <= i)
                    {
                        end = path.Length - 1;
                    }

                    segments.Add(path[i..(end + 1)]);
                    i = end + 1;

                    continue;
                }

                current.Append(c);
                i++;
            }

            Flush();

            return segments.ToImmutable();

            void Flush()
            {
                if (current.Length > 0)
                {
                    segments.Add(current.ToString());
                    current.Clear();
                }
            }
        }

        private static string Join(IEnumerable<string> segments)
        {
            var builder = new StringBuilder();

            foreach (var segment in segments)
            {
                if (builder.Length > 0 && IsIndex(segment).IsFalse())
                {
                    builder.Append('.');
                }

                builder.Append(segment);
            }

            return builder.ToString();
        }
    }
}
