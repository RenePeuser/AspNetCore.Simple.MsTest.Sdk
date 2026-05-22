using System.Reflection;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    /// <summary>
    /// Helper methods for converting source locations (type names) to clickable file URIs.
    /// </summary>
    internal static class SourceLocationHelper
    {
        /// <summary>
        /// Tries to convert a source location string (type name or method name) to a clickable file:/// URI.
        /// If successful, returns the URI. Otherwise, returns the original source location.
        /// </summary>
        /// <param name="sourceLocation">Source location like "MinimalApi.Api.Persons.V1.CreatePersonEndpoint" or "MinimalApi.Api.Persons.V1.CreatePersonEndpoint.Handle"</param>
        /// <param name="callingAssembly">Assembly to search for the type</param>
        /// <returns>Clickable file:/// URI if found, otherwise the original source location</returns>
        public static string ToClickableUri(string? sourceLocation,
                                            Assembly callingAssembly)
        {
            if (sourceLocation.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // Special cases that are not types
            if (sourceLocation.Contains("Program.cs") ||
                sourceLocation.Contains("Lambda") ||
                sourceLocation.Contains("(Inline"))
            {
                return sourceLocation; // Already a readable location
            }

            // Try to extract type name (remove method name if present)
            var typeName = sourceLocation;
            var lastDotIndex = sourceLocation.LastIndexOf('.');

            Type? type = null;

            if (lastDotIndex > 0)
            {
                // Try with full name first (might be full type name without method)
                type = TryGetType(sourceLocation, callingAssembly);

                if (type == null)
                {
                    // Try without the last segment (might be a method name)
                    typeName = sourceLocation.Substring(0, lastDotIndex);
                    type = TryGetType(typeName, callingAssembly);
                }
            }

            if (type != null)
            {
                string? filePath = null;
                string? errorMessage = null;

                try
                {
                    filePath = TryResolveFilePathFromType(type, callingAssembly);
                }
#pragma warning disable CA1031
                catch (Exception ex)
#pragma warning restore CA1031
                {
                    errorMessage = ex.Message;
                }

                if (filePath.IsNotNullOrWhiteSpace())
                {
                    var uri = $"file:///{filePath.Replace('\\', '/')}";

                    return uri;
                }

                // Debug: show what we found with error details
                if (errorMessage.IsNotNullOrWhiteSpace())
                {
                    return $"{sourceLocation} (Type found, Error: {errorMessage})";
                }

                return $"{sourceLocation} (Type found: {type.FullName}, but no source file)";
            }

            // Fallback: return original source location (with debug info)
            return $"{sourceLocation} (Type not found)";
        }

        private static Type? TryGetType(string typeName,
                                        Assembly callingAssembly)
        {
            try
            {
                // Try to get type from the calling assembly first
                var type = callingAssembly.GetType(typeName);

                if (type != null)
                {
                    return type;
                }

                // Try from all loaded assemblies
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in loadedAssemblies)
                {
                    // Skip system assemblies for performance
                    if (assembly.FullName?.StartsWith("System", StringComparison.Ordinal) ?? false)
                    {
                        continue;
                    }

                    if (assembly.FullName?.StartsWith("Microsoft", StringComparison.Ordinal) ?? false)
                    {
                        continue;
                    }

                    type = assembly.GetType(typeName);

                    if (type != null)
                    {
                        return type;
                    }
                }

                return null;
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return null;
            }
        }

        private static string? TryResolveFilePathFromType(Type type,
                                                          Assembly callingAssembly)
        {
            try
            {
                // Get the assembly location (DLL path)
                var assemblyLocation = type.Assembly.Location;

                if (assemblyLocation.IsNullOrWhiteSpace())
                {
                    return null;
                }

                // Get the directory of the DLL (e.g., bin/Debug/net10.0)
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                if (assemblyDirectory.IsNullOrWhiteSpace())
                {
                    return null;
                }

                // Navigate up to find the solution root
                var solutionRoot = FindProjectRoot(assemblyDirectory);

                if (solutionRoot.IsNullOrWhiteSpace())
                {
                    return null;
                }

                // Get the assembly name to find the correct project folder
                var assemblyName = type.Assembly.GetName().Name ?? string.Empty;

                // Find the project directory that contains a .csproj with this assembly name
                var projectRoot = FindProjectDirectory(solutionRoot, assemblyName);

                if (projectRoot.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException($"Project directory not found. SolutionRoot: {solutionRoot}, AssemblyName: {assemblyName}");
                }

                // Convert namespace to relative path
                var fullNamespace = type.FullName ?? type.Name;

                // Remove assembly name from the beginning if present
                var relativePath = fullNamespace;

                if (relativePath.StartsWith(assemblyName + ".", StringComparison.Ordinal))
                {
                    relativePath = relativePath.Substring(assemblyName.Length + 1);
                }

                // Get the class name from the type
                var pathParts = relativePath.Split('.');
                var fileName = pathParts[^1] + ".cs"; // Last part is the class name

                // First try: search recursively for the file in the project
                // This handles cases where folder structure doesn't match namespace
                var foundFile = SearchForFile(projectRoot, fileName);

                if (foundFile.IsNotNullOrWhiteSpace())
                {
                    // Note: Line number extraction from PDB is complex and not implemented yet
                    // For now, we just return the file path
                    return foundFile;
                }

                // Fallback: build path from namespace (might not match folder structure)
                var directories = pathParts.Take(pathParts.Length - 1).ToArray();
                var expectedPath = Path.Combine(projectRoot, Path.Combine(directories), fileName);

                // Debug: include attempted path
                var attemptedPaths = $"Searched recursively for {fileName} in {projectRoot}, also tried namespace-based path: {expectedPath}";

                // Return debug info in exception
                throw new InvalidOperationException($"File not found. {attemptedPaths}");
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return null;
            }
        }

        private static string? FindProjectRoot(string startDirectory)
        {
            // First, search backwards to find the solution file
            var solutionRoot = FindSolutionRoot(startDirectory);

            if (solutionRoot.IsNullOrWhiteSpace())
            {
                // Fallback: just find the first .csproj going up
                return FindFirstCsprojDirectory(startDirectory);
            }

            return solutionRoot;
        }

        private static string? FindSolutionRoot(string startDirectory)
        {
            var directory = startDirectory;

            // Navigate up max 15 levels to find .sln file
            for (var i = 0; i < 15; i++)
            {
                if (directory.IsNullOrWhiteSpace())
                {
                    break;
                }

                // Check if .sln file exists in this directory
                var slnFiles = Directory.GetFiles(directory, "*.sln");

                if (slnFiles.Length > 0)
                {
                    return directory;
                }

                // Go up one level
                var parent = Directory.GetParent(directory);

                if (parent == null)
                {
                    break;
                }

                directory = parent.FullName;
            }

            return null;
        }

        private static string? FindFirstCsprojDirectory(string startDirectory)
        {
            var directory = startDirectory;

            // Navigate up max 10 levels to find .csproj file
            for (var i = 0; i < 10; i++)
            {
                if (directory.IsNullOrWhiteSpace())
                {
                    break;
                }

                // Check if .csproj file exists in this directory
                var csprojFiles = Directory.GetFiles(directory, "*.csproj");

                if (csprojFiles.Length > 0)
                {
                    return directory;
                }

                // Go up one level
                var parent = Directory.GetParent(directory);

                if (parent == null)
                {
                    break;
                }

                directory = parent.FullName;
            }

            return null;
        }

        private static string? FindProjectDirectory(string solutionRoot, string assemblyName)
        {
            try
            {
                // Search recursively for a .csproj file with matching name
                var csprojPattern = $"{assemblyName}.csproj";
                var csprojFiles = Directory.GetFiles(solutionRoot, csprojPattern, SearchOption.AllDirectories);

                if (csprojFiles.Length > 0)
                {
                    // Return the directory containing the .csproj
                    return Path.GetDirectoryName(csprojFiles[0]);
                }

                return null;
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return null;
            }
        }

        private static string? SearchForFile(string rootDirectory, string fileName)
        {
            try
            {
                // Search recursively for the file
                // Skip common folders that shouldn't contain source files
                var files = Directory.EnumerateFiles(rootDirectory, fileName, new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    MatchCasing = MatchCasing.CaseInsensitive,
                    IgnoreInaccessible = true
                });

                foreach (var file in files)
                {
                    // Skip bin, obj, and other build folders
                    if (file.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase) ||
                        file.Contains("\\obj\\", StringComparison.OrdinalIgnoreCase) ||
                        file.Contains("\\.vs\\", StringComparison.OrdinalIgnoreCase) ||
                        file.Contains("\\node_modules\\", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    return file;
                }

                return null;
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return null;
            }
        }

        private static int TryGetLineNumber(Type type)
        {
            // Getting the actual line number requires reading PDB (debug symbols) files
            // which is complex and requires additional libraries like System.Reflection.Metadata
            //
            // For now, we return 0 to omit the line number from the URI
            // The file:/// link will still work and open the file in the IDE
            //
            // Future enhancement: Use System.Reflection.Metadata to read PDB and get actual line numbers
            return 0;
        }
    }
}