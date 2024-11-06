using System.Collections.Immutable;
using System.Text.Json.Nodes;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class StringExtensions
    {
        public static string ResolveParameters(this string value, (string Key, object? Value)[] parameters)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            var replacedString = value;

            foreach (var keyValue in parameters)
            {
                // We have to take care of int, bool, long and so on
                // Json sample
                // {
                //   "Id": "$projectId$",
                // }
                // -------------------------------------------------
                // Json sample
                // {
                //   "Id": $projectId$,
                // }
                if (keyValue.Value.IsNotNull() && keyValue.Value.GetType().IsPrimitive)
                {
                    replacedString = replacedString.Replace($"{keyValue.Key}", keyValue.Value.ToString());
                }
                else
                {
                    replacedString = replacedString.Replace(keyValue.Key, keyValue.Value?.ToString());
                }
            }

            return replacedString;
        }
    }
}
