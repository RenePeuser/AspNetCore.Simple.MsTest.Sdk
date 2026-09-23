using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class AddTestContextHelperExtension
    {
        public static void AddTestContextHelper(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITestContextHelper, TestContextHelper>();
        }
    }

    internal interface ITestContextHelper
    {
        /// <summary>
        /// The fully qualified class name of the test, built from the assembly name and the folder
        /// structure below the project folder. Example: MinimalApi.Test.Api.Persons.PersonEndpointsTests
        /// </summary>
        string ExtractFullyQualifiedClassName(string callerFilePath,
                                              Assembly callingAssembly);
    }

    /// <summary>
    /// Extracts test context information.
    /// </summary>
    internal sealed class TestContextHelper : ITestContextHelper
    {
        /// <summary>
        /// Extracts the fully qualified class name from the test context.
        /// Combines assembly name with the relative path structure to build the namespace.
        /// Example: MinimalApi.Test.Api.Persons.PersonEndpointsTests
        /// </summary>
        /// <param name="callerFilePath">Full file path of the test class</param>
        /// <param name="callingAssembly">Assembly containing the test</param>
        /// <returns>Fully qualified class name (namespace + class name)</returns>
        public string ExtractFullyQualifiedClassName(string callerFilePath,
                                                            Assembly callingAssembly)
        {
            var assemblyName = callingAssembly.GetName().Name ?? "Unknown";
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);

            // Extract relative path structure to build namespace
            // Example: D:\Repo\MinimalApi.Test\Api\Persons\PersonEndpointsTests.cs
            //          → Api\Persons\PersonEndpointsTests.cs
            //          → MinimalApi.Test.Api.Persons.PersonEndpointsTests

            // Normalize path separators to backslash for Windows
            var normalizedPath = callerFilePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var pathParts = normalizedPath.Split(Path.DirectorySeparatorChar);

            // Find where the assembly name appears in the path
            var assemblyIndex = -1;

            for (var i = pathParts.Length - 1; i >= 0; i--)
            {
                if (pathParts[i].Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                {
                    assemblyIndex = i;

                    break;
                }
            }

            // If assembly name found in path, extract everything after it (excluding the filename)
            if (assemblyIndex >= 0 && assemblyIndex < pathParts.Length - 1)
            {
                var namespaceParts = new List<string> { assemblyName };

                // Add path segments between assembly folder and file (these become namespace parts)
                for (var i = assemblyIndex + 1; i < pathParts.Length - 1; i++)
                {
                    var part = pathParts[i];

                    // Skip common non-namespace folders
                    if (part.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                        part.Equals("obj", StringComparison.OrdinalIgnoreCase) ||
                        part.Equals("Debug", StringComparison.OrdinalIgnoreCase) ||
                        part.Equals("Release", StringComparison.OrdinalIgnoreCase) ||
                        part.StartsWith("net", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    namespaceParts.Add(part);
                }

                namespaceParts.Add(fileName);

                return string.Join(".", namespaceParts);
            }

            // Fallback: just return filename if we can't extract namespace
            return fileName;
        }

    }
}