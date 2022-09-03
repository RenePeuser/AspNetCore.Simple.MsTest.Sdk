using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // This is only for dev who know what they are doing
        // With this method info you are able to intercept the existing assert functionality
        // to use external once
        public static MethodInfo? CustomAssertMethod { get; set; }


        private static async Task AssertHttpCall(this HttpClient client,
                                                 string url,
                                                 string payloadAsJson,
                                                 Func<HttpClient, string, string, Task> httpFunction,
                                                 Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            await httpFunction(client, url, jsonPayload).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<HttpClient, string, Task<TResult>> httpFunction,
                                                             HttpMethod httpMethod,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, (client, path, _) => httpFunction(client, path), httpMethod, callingAssembly);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<HttpClient, string, Task<TResult>> httpFunction,
                                                             HttpMethod httpMethod,
                                                             Assembly callingAssembly,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, (client, path, _) => httpFunction(client, path), httpMethod, callingAssembly, differenceFunc);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                          string url,
                                                          string payloadAsJson,
                                                          string resultAsJson,
                                                          Func<TResult, TResult> filterFunc,
                                                          Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                          HttpMethod httpMethod,
                                                          Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, httpFunction, httpMethod, callingAssembly, difference => difference);
        }

        private static async Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string resultAsJson,
                                                                   Func<TResult, TResult> filterFunc,
                                                                   Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                                   HttpMethod httpMethod,
                                                                   Assembly callingAssembly,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            if (CustomAssertMethod is not null)
            {
                return await AssertCustomHttpCall(client, url, payloadAsJson, resultAsJson, filterFunc, httpFunction, httpMethod, callingAssembly, differenceFunc).ConfigureAwait(false);
            }

            return await AssertHttpCallInternal(client, url, payloadAsJson, resultAsJson, filterFunc, httpFunction, httpMethod, callingAssembly, differenceFunc).ConfigureAwait(false);
        }

        private static async Task<TResult> AssertHttpCallInternal<TResult>(this HttpClient client,
                                                                           string url,
                                                                           string payloadAsJson,
                                                                           string resultAsJson,
                                                                           Func<TResult, TResult> filterFunc,
                                                                           Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                                           HttpMethod httpMethod,
                                                                           Assembly callingAssembly,
                                                                           Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var currentResult = await httpFunction(client, url, jsonPayload).ConfigureAwait(false);

            var httpCallInfo = $"{Environment.NewLine}Call: '{httpMethod} {url}' was not successful.";

            Assert.That.ObjectsAreEqual(() => resultAsJson, () => currentResult, filterFunc, httpCallInfo, callingAssembly, differenceFunc);

            return currentResult;
        }

        private static async Task<TResult> AssertCustomHttpCall<TResult>(this HttpClient client,
                                                                  string url,
                                                                  string payloadAsJson,
                                                                  string resultAsJson,
                                                                  Func<TResult, TResult> filterFunc,
                                                                  Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                                  HttpMethod httpMethod,
                                                                  Assembly callingAssembly,
                                                                  Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            if (CustomAssertMethod is null)
            {
                throw new InvalidOperationException("CustomAssertMethod have not to be null when calling AssertCustomHttpCall");
            }

            if (CustomAssertMethod.IsGenericMethod.IsFalse())
            {
                throw new InvalidOperationException("Assert delegate is not a generic method");
            }

            var genericMethod = CustomAssertMethod.MakeGenericMethod(typeof(TResult));
            var tasReturnType = genericMethod.Invoke(null, new object[] { client, url, payloadAsJson, resultAsJson, filterFunc, httpFunction, httpMethod, callingAssembly, differenceFunc });
            if (tasReturnType is Task task)
            {
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                if (resultProperty is null)
                {
                    throw new InvalidOperationException("Property of Result from executing task was not found, please check that your method have a Task<T> that a result exists.");
                }

                var returnValue = resultProperty.GetValue(task);
                if (returnValue is null)
                {
                    throw new InvalidOperationException("The return value of your async method was NULL which was unexpected, please check your code execution");
                }

                if (returnValue is TResult result)
                {
                    return result;
                }

                throw new InvalidOperationException($"The type of the return value from your {nameof(CustomAssertMethod)} is: {returnValue.GetType().Name} which does not expect type: {typeof(TResult).Name}");

            }

            throw new InvalidOperationException("Unknown result of invoked generic method");
        }
    }
}
