using System.Reflection;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Attribute that locates use case request JSON files as test data for a given test method.
    /// </summary>
    /// <remarks>
    /// This attribute implements the <see cref="ITestDataSource"/> interface for use with parameterized tests.
    /// Its purpose is to dynamically locate and supply JSON resources representing use cases based on a specified request
    /// folder path and to allow additional parameters to be injected into each test case.
    ///
    /// <para>
    /// If no request folder is provided in the attribute constructor, the attribute automatically determines the
    /// request folder by removing the declaring class's name from its full namespace and appending a ".Requests" suffix.
    /// </para>
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
    /// separate test case. Each test case data object consists of the JSON resource identifier as well as any extra parameters
    /// provided via the attribute. The <see cref="GetDisplayName"/> method returns a descriptive name that combines the test method's name
    /// and the test data details.
    /// </para>
    ///
    /// <para>
    /// The attribute can be applied with or without an explicit request folder and additional parameters.
    /// </para>
    /// </remarks>
    /// <example>
    /// Applying the attribute without a path or additional parameters:
    /// <code>
    /// [RequestLocator]
    /// public void TestSomeUseCase(...) { }
    /// </code>
    /// In this scenario, the attribute automatically calculates the request folder from the test class's namespace
    /// and supplies just the JSON identifier for each test case.
    ///
    /// <para>
    /// Applying the attribute with an explicit folder and additional parameters:
    /// <code>
    /// [RequestLocator("Api.User.V1.Create.Status_200_Ok.Requests", "ExtraParam", 42)]
    /// public void TestSomeUseCase(...) { }
    /// </code>
    /// Here, the test case data will include the JSON identifier followed by the extra parameters "ExtraParam" and 42.
    /// </para>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class EnumTestCaseAttribute<T>(params object[] parameters) : Attribute, ITestDataSource where T : Enum
    {
        public object[] Parameters { get; } = parameters;

        /// <summary>
        /// Gets the test data by locating JSON resources in the computed or explicitly provided request folder.
        /// Each test data object contains the JSON resource identifier and any additional parameters.
        /// </summary>
        /// <param name="methodInfo">The test method's metadata information.</param>
        /// <returns>
        /// An enumerable collection of test data objects, where each object array begins with the JSON resource identifier,
        /// followed by any extra parameters.
        /// </returns>
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            if (typeof(T).IsEnum.IsFalse())
            {
                yield break;
            }

            // Filter to only include JSON files that contain the computed or provided path.
            var useCases = Enum.GetValues(typeof(T)).ToListOfType<T>();

            // Yield each identified use case as a separate test input.
            foreach (var useCase in useCases)
            {
                var parameters = GetParams(useCase, Parameters).ToArray();

                yield return parameters;
            }

            yield break;

            // Local function to combine the JSON resource with the extra parameters.
            static IEnumerable<object> GetParams(object useCase,
                                                 object[] parameters)
            {
                yield return useCase;

                foreach (var param in parameters)
                {
                    yield return param;
                }
            }
        }

        /// <summary>
        /// Generates a display name for each test case data variation.
        /// </summary>
        /// <param name="methodInfo">The test method's metadata information.</param>
        /// <param name="data">An array of objects representing the test case data.</param>
        /// <returns>
        /// A descriptive name that combines the test method's name with the test data details, aiding in test identification.
        /// </returns>
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