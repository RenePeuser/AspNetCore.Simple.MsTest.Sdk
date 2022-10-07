using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static async Task GetAsAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return;
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> GetAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync("Not successful").ConfigureAwait(false));
        }

        public static async Task<byte[]> GetFileStreamAsByteArray(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var fileStreamResult = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await using (fileStreamResult.ConfigureAwait(false))
                {
                    var memoryStream = new MemoryStream();
                    await using (memoryStream.ConfigureAwait(false))
                    {
                        await fileStreamResult.CopyToAsync(memoryStream).ConfigureAwait(false);
                        var byteArray = memoryStream.ToArray();
                        return byteArray;
                    }
                }
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not success full.").ConfigureAwait(false));
        }

        public static async Task<InMemoryFile> GetFileAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var fileStreamResult = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await using (fileStreamResult.ConfigureAwait(false))
                {
                    var memoryStream = new MemoryStream();
                    await using (memoryStream.ConfigureAwait(false))
                    {
                        await fileStreamResult.CopyToAsync(memoryStream).ConfigureAwait(false);
                        var byteArray = memoryStream.ToArray();
                        return new InMemoryFile(byteArray, "test");
                    }
                }
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not success full.").ConfigureAwait(false));
        }

        public static async IAsyncEnumerable<InMemoryFile> GetFilesFromZipResponseAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var fileStreamResult = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await using (fileStreamResult.ConfigureAwait(false))
                {
                    using var zipArchive = new ZipArchive(fileStreamResult);
                    foreach (var entry in zipArchive.Entries)
                    {
                        var stream = entry.Open();
                        var memoryStream = new MemoryStream();
                        await using (memoryStream.ConfigureAwait(false))
                        {
                            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
                            var byteArray = memoryStream.ToArray();
                            yield return new InMemoryFile(byteArray, entry.Name);
                        }
                    }
                }
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not success full.").ConfigureAwait(false));
        }
    }
}
