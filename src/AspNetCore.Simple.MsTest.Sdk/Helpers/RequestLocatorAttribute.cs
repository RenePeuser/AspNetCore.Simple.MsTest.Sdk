using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Attribute that locates use case request JSON files as test data for a given test method.
    /// </summary>
    /// <remarks>
    /// This attribute implements the <see cref="ITestDataSource"/> interface for use with parameterized tests.
    /// Its purpose is to dynamically locate and supply JSON resources representing use cases based on a specified request
    /// folder path. If no request folder is provided in the attribute constructor, the attribute automatically determines the
    /// request folder by removing the declaring class's name from its full namespace and appending a ".Requests" suffix.
    /// 
    /// <para>
    /// For example, if a test class is declared in the namespace:
    /// <c>Api.User.V1.Create.Status_200_Ok</c>,
    /// then by default, the attribute will look for JSON resources in the folder:
    /// <c>Api.User.V1.Create.Status_200_Ok.Requests</c>.
    /// </para>
    /// 
    /// <para>
    /// The attribute retrieves all the embedded manifest resource names from the test class’s assembly,
    /// filters them by a matching folder path and ".json" extension, and then prepares each found resource as a
    /// separate test case. The <see cref="GetDisplayName"/> method returns a descriptive name that combines the test method's name
    /// and the use case identifier.
    /// </para>
    /// </remarks>
    /// <example>
    /// Applying the attribute without a path:
    /// <code>
    /// [RequestUseCaseLocator]
    /// public void TestSomeUseCase(...) { }
    /// </code>
    /// The above will automatically calculate the request folder from the test class’s namespace.
    /// 
    /// <para>
    /// Applying the attribute with an explicit folder:
    /// <code>
    /// [RequestUseCaseLocator("Api.User.V1.Create.Status_200_Ok.Requests")]
    /// public void TestSomeUseCase(...) { }
    /// </code>
    /// </para>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequestLocatorAttribute(string requestFolder = "") : Attribute, ITestDataSource
    {
        /// <summary>
        /// Gets the test data by locating JSON resources in the computed or explicitly provided request folder.
        /// </summary>
        /// <param name="methodInfo">The test method's metadata information.</param>
        /// <returns>An enumerable collection of test data objects (each containing the use case identifier).</returns>
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // Use the specified requestFolder or compute it from the context.
            var currentPath = requestFolder;

            var declaringType = methodInfo.DeclaringType;

            if (declaringType == null)
            {
                yield break;
            }

            // Automatically determine folder path if no explicit requestFolder is provided.
            if (string.IsNullOrWhiteSpace(currentPath))
            {
                if (string.IsNullOrWhiteSpace(declaringType.FullName))
                {
                    yield break;
                }

                // Remove the class name portion from the full namespace and append ".Requests"
                var folderPath = declaringType.FullName.Replace($".{declaringType.Name}", string.Empty);
                currentPath = $"{folderPath}.Requests";
            }

            // Fetch all manifest resource names from the assembly.
            var manifestResourceNames = declaringType.Assembly.GetManifestResourceNames();

            // Filter to only include JSON files that contain the computed or provided path.
            var useCases = manifestResourceNames.Where(file => file.Contains(currentPath) && file.EndWith(".json"))
                                                .Select(item => item.Split('.').TakeLast(2).Aggregate((a,
                                                                                                       b) => $"{a}.{b}"))
                                                .ToImmutableList();

            // Yield each identified use case as a separate test input.
            foreach (var useCase in useCases)
            {
                yield return new object[] { useCase };
            }
        }

        /// <summary>
        /// Generates a display name for each test case data variation.
        /// </summary>
        /// <param name="methodInfo">The test method's metadata information.</param>
        /// <param name="data">An array of objects representing the test case data.</param>
        /// <returns>A descriptive name combining the method name and the test data details.</returns>
        public string GetDisplayName(MethodInfo methodInfo,
                                     object?[]? data)
        {
            if (data is null)
            {
                return methodInfo.Name;
            }

            // Flatten the data items into a comma-separated string and combine with the method name.
            var dataDisplay = string.Join(", ", data.Select(item => item?.ToString() ?? "null"));

            return $"{methodInfo.Name} ({dataDisplay})";
        }
    }
}
