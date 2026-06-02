using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Terminal interface for HTTP assertions that only validate status codes without response body checks.
    /// This interface supports direct awaiting through GetAwaiter.
    /// </summary>
    public interface IHttpStatusAssertable
    {
        /// <summary>
        /// Executes the HTTP request and validates only the status code.
        /// </summary>
        /// <returns>Task that completes when assertion is done</returns>
        Task ExecuteAsync();

        /// <summary>
        /// Enables direct await on the assertable without calling ExecuteAsync.
        /// </summary>
        /// <returns>Task awaiter</returns>
        TaskAwaiter GetAwaiter();
    }
}