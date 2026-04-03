using System;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddParameterReplacerExtension
    {
        public static void AddParameterReplacer(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IParameterReplacer, ParameterReplacer>();
        }
    }

    /// <summary>
    /// Service for replacing parameters in JSON strings.
    /// Handles both directions: placeholder → value (for comparison) and value → placeholder (for snapshot writing).
    /// </summary>
    public interface IParameterReplacer
    {
        /// <summary>
        /// Replaces placeholders with actual values in a JSON string.
        /// Used when reading/comparing JSON - converts $placeholder$ to actual values.
        /// </summary>
        /// <param name="json">The JSON string containing placeholders</param>
        /// <param name="parameters">Parameters as (Key, Value) tuples where Key is the placeholder</param>
        /// <returns>JSON string with placeholders replaced by values</returns>
        string ResolveParameters(string json,
                                 params (string Key, object? Value)[] parameters);

        /// <summary>
        /// Replaces actual values with placeholders in a JSON string.
        /// Used when writing snapshots - converts actual values back to $placeholder$.
        /// Works on JToken level for accurate property and full-text replacement.
        /// </summary>
        /// <param name="json">The JSON string containing actual values</param>
        /// <param name="parameters">Parameters as (Key, Value) tuples where Key is the placeholder and Value is what to replace</param>
        /// <returns>JSON string with values replaced by placeholders</returns>
        string ReplaceWithPlaceholders(string json,
                                       params (string Key, object? Value)[] parameters);
    }

    /// <summary>
    /// Implementation of parameter replacement service.
    /// Combines string-based replacement (for reading) and JToken-based replacement (for writing).
    /// </summary>
    internal sealed class ParameterReplacer : IParameterReplacer
    {
        public string ResolveParameters(string json,
                                        params (string Key, object? Value)[] parameters)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json;
            }

            var replacedString = json;

            // Important to replace the parameters after ordering, because the order can change the
            // position of the parameters in the json and if we replace before, we can end up with
            // wrong replacements
            var sortedParameters = parameters.OrderByDescending(p => p.Key.Length);

            foreach (var keyValue in sortedParameters)
            {
                if (keyValue.Key.IsNullOrWhiteSpace())
                {
                    continue;
                }

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
                // -------------------------------------------------
                // Json sample
                // {
                //   "ReferenceId": null,  < If value is null this must be
                // }

                if (keyValue.Value.IsNull())
                {
                    // We have to take care about "$MyParam$" <- So also the " have to gone with
                    replacedString = replacedString.Replace($"\"{keyValue.Key}\"", "null");

                    // And if someone use it correctly already $MyParam$ <- we have to replace it also
                    replacedString = replacedString.Replace(keyValue.Key, "null");

                    continue;
                }

                var type = keyValue.Value.GetType();

                if (keyValue.Value.IsNotNull() &&
                    type.IsPrimitive)
                {
                    var primitiveTypeValue = keyValue.Value.ToString();

                    if (primitiveTypeValue.IsNull())
                    {
                        continue;
                    }

                    if (type.EqualsTo(typeof(bool)))
                    {
                        primitiveTypeValue = primitiveTypeValue.ToLowerInvariant();
                    }

                    replacedString = replacedString.Replace($"\"{keyValue.Key}\"", primitiveTypeValue, StringComparison.Ordinal);
                }

                // Only replace full placeholder tokens to prevent corrupting other placeholders/words.
                replacedString = replacedString.Replace(keyValue.Key, keyValue.Value.ToString(), StringComparison.Ordinal);
            }

            return replacedString;
        }

        public string ReplaceWithPlaceholders(string json,
                                              params (string Key, object? Value)[] parameters)
        {
            if (json.IsNullOrWhiteSpace() || parameters.IsNullOrEmpty())
            {
                return json;
            }

            var root = JToken.Parse(json);

            ApplySmartReplacements(root, parameters);

            return root.ToString(Formatting.Indented);
        }

        // ============================================================
        // Private helper methods for smart replacement logic
        // ============================================================

        private static void ApplySmartReplacements(JToken root,
                                                   params (string key, object? Value)[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return;
            }

            foreach (var (key, value) in parameters)
            {
                if (key.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var propertyName = key.Trim('$');

                // 🔥 Property-Replacement IMMER versuchen – auch bei null
                ReplaceByProperty(root, propertyName, key,
                                  value);

                // FullText nur wenn value != null
                if (value != null && value.ToString()?.Length >= 3)
                {
                    ReplaceFullText(root, key, value);
                }
            }
        }

        private static bool ReplaceByProperty(JToken token,
                                              string propertyName,
                                              string placeholder,
                                              object? originalValue)
        {
            var replaced = false;

            if (token is JProperty prop &&
                string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (originalValue == null && prop.Value.Type == JTokenType.Null)
                {
                    prop.Value = placeholder;

                    return true;
                }

                if (originalValue != null && JToken.DeepEquals(prop.Value, JToken.FromObject(originalValue)))
                {
                    prop.Value = placeholder;

                    return true;
                }
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    replaced |= ReplaceByProperty(child, propertyName, placeholder,
                                                  originalValue);
                }
            }

            return replaced;
        }

        private static void ReplaceFullText(JToken token,
                                            string placeholder,
                                            object? originalValue)
        {
            if (originalValue == null)
            {
                return;
            }

            var originalText = originalValue.ToString();

            if (originalText.IsNullOrWhiteSpace() || originalText.Length < 3)
            {
                return;
            }

            if (token is JValue value && value.Type == JTokenType.String)
            {
                var s = (string?)value.Value;

                if (s.IsNullOrWhiteSpace() || s.Contains(placeholder))
                {
                    return;
                }

                var escaped = Regex.Escape(originalText);
                var pattern = $@"\b{escaped}\b";

                var updated = ReplaceOutsidePlaceholders(s,
                                                         text => Regex.Replace(text,
                                                                               pattern,
                                                                               placeholder,
                                                                               RegexOptions.CultureInvariant));

                value.Value = ReplaceOutsidePlaceholders(updated,
                                                         text => text.Replace(originalText,
                                                                              placeholder,
                                                                              StringComparison.Ordinal));
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    ReplaceFullText(child, placeholder, originalValue);
                }
            }
        }

        private static string ReplaceOutsidePlaceholders(string input,
                                                         Func<string, string> replacer)
        {
            if (input.Contains('$').IsFalse())
            {
                return replacer(input);
            }

            var parts = input.Split('$');

            for (var i = 0; i < parts.Length; i += 2)
            {
                parts[i] = replacer(parts[i]);
            }

            return string.Join("$", parts);
        }
    }
}
