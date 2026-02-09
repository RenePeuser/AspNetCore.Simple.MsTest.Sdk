using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddJsonPathWriterExtension
    {
        internal static void AddJsonPathWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<JsonPathWriter>();
        }
    }

    internal sealed class JsonPathWriter
    {
        internal void AddOrUpdate(JToken root, string path, JToken value)
        {
            var existing = root.SelectToken(path);
            if (existing != null)
            {
                existing.Replace(value.DeepClone());
                return;
            }

            // create parents (simplified, robust)
            var segments = path.Split('.');
            var current = root;

            for (var i = 0; i < segments.Length - 1; i++)
            {
                var seg = segments[i];

                if (seg.Contains('['))
                {
                    var name = seg[..seg.IndexOf('[')];
                    var index = int.Parse(seg[(seg.IndexOf('[') + 1)..seg.IndexOf(']')]);

                    if (current[name] is not JArray arr)
                    {
                        arr = new JArray();
                        ((JObject)current)[name] = arr;
                    }

                    while (arr.Count <= index)
                        arr.Add(JValue.CreateNull());

                    if (arr[index] == null || arr[index]!.Type == JTokenType.Null)
                    {
                        arr[index] = new JObject();
                    }

                    current = arr[index]!;
                }
                else
                {
                    if (current[seg] == null)
                    {
                        ((JObject)current)[seg] = new JObject();
                    }

                    current = current[seg]!;
                }
            }

            ((JObject)current)[segments[^1]] = value.DeepClone();
        }

        internal void Remove(JToken root, string path)
        {
            var token = root.SelectToken(path);
            if (token == null)
            {
                return;
            }

            if (token.Parent is JProperty prop)
            {
                prop.Remove();
            }
            else
            {
                token.Remove();
            }
        }
    }
}
