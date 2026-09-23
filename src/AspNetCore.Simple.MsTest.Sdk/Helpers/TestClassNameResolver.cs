using System;
using System.IO;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class AddTestClassNameResolverExtension
    {
        public static void AddTestClassNameResolver(this IServiceCollection services)
        {
            services.AddSourceLocationHelper();
            services.AddSingletonIfNotExists<ITestClassNameResolver, TestClassNameResolver>();
        }
    }

    public interface ITestClassNameResolver
    {
        string Resolve(string callerFilePath,
                       string projectName);
    }

    /// <summary>
    /// Turns a caller file path into the fully qualified test class name shown in error output.
    ///
    /// This used to locate the project by searching the path for a folder named like the assembly.
    /// A legacy project whose folder name differs from its assembly name has no such folder, so the
    /// search failed and the output degraded to the bare file name - the same defect that was fixed
    /// in <see cref="EmbeddedFileLocalizer"/>. The nearest folder holding a csproj is the project root
    /// by definition, so that is what is used first.
    /// </summary>
    public sealed class TestClassNameResolver(ISourceLocationHelper sourceLocationHelper) : ITestClassNameResolver
    {
        public string Resolve(string callerFilePath,
                              string projectName)
        {
            try
            {
                if (callerFilePath.IsNullOrWhiteSpace())
                {
                    return projectName;
                }

                var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
                var directory = Path.GetDirectoryName(callerFilePath);

                var byCsproj = ResolveByProjectFolder(directory, fileName, projectName);

                if (byCsproj.IsNotNullOrWhiteSpace())
                {
                    return byCsproj;
                }

                // The sources are not on this machine (a snapshot recorded elsewhere, a ci artifact).
                // Matching the assembly name against the path is all that is left.
                return ResolveByAssemblyName(callerFilePath, fileName, projectName);
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                // Fallback to full caller file path on any error
                return callerFilePath;
            }
        }

        private string? ResolveByProjectFolder(string? directory,
                                               string fileName,
                                               string projectName)
        {
            if (directory.IsNullOrWhiteSpace())
            {
                return null;
            }

            var projectFolder = sourceLocationHelper.FindFirstCsprojDirectory(directory);

            if (projectFolder.IsNullOrWhiteSpace())
            {
                return null;
            }

            var relativePath = Path.GetRelativePath(projectFolder, directory);

            // The file sits outside the project folder - that is not a namespace we can name.
            if (relativePath.StartsWith("..", StringComparison.Ordinal))
            {
                return null;
            }

            var namespacePath = relativePath
                                .Replace(Path.DirectorySeparatorChar, '.')
                                .Replace(Path.AltDirectorySeparatorChar, '.')
                                .Trim('.');

            // The test sits in the project root - there is no folder part to append.
            return namespacePath.IsNullOrWhiteSpace()
                       ? $"{projectName}.{fileName}"
                       : $"{projectName}.{namespacePath}.{fileName}";
        }

        private static string ResolveByAssemblyName(string callerFilePath,
                                                    string fileName,
                                                    string projectName)
        {
            var pathSegments = callerFilePath.Replace("\\", "/").Split('/');
            var projectIndex = Array.FindIndex(pathSegments, segment => segment.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (projectIndex < 0 || projectIndex >= pathSegments.Length - 1)
            {
                return fileName;
            }

            var namespaceParts = pathSegments.Skip(projectIndex + 1)
                                             .Take(pathSegments.Length - projectIndex - 2)
                                             .ToList();

            if (namespaceParts.Count.EqualsTo(0))
            {
                // File is directly in project root
                return $"{projectName}.{fileName}";
            }

            var namespaceString = string.Join(".", namespaceParts.Select(segment => segment.Replace(" ", string.Empty)));

            return $"{projectName}.{namespaceString}.{fileName}";
        }
    }
}