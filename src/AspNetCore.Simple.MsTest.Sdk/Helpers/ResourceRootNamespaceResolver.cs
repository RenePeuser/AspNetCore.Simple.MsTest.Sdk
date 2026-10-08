using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddResourceRootNamespaceResolverExtension
    {
        public static void AddResourceRootNamespaceResolver(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IResourceRootNamespaceResolver, ResourceRootNamespaceResolver>();
        }
    }

    /// <summary>
    /// Answers with which namespace the manifest resources of an assembly start.
    /// By default that is the assembly name, but MSBuild uses the (overridable) RootNamespace and
    /// honours per item LogicalName overrides. Legacy test projects make use of this, e.g.
    /// AssemblyName "Pulse.DataManagement.Test.API.MySql" with RootNamespace "Pulse.DataManagement.Test",
    /// so the resource for "API\V1\Results\Foo.json" is named
    /// "Pulse.DataManagement.Test.API.V1.Results.Foo.json".
    /// Assuming the assembly name would map such a resource to no physical file at all.
    /// </summary>
    internal interface IResourceRootNamespaceResolver
    {
        /// <summary>
        /// The root namespace shared by the majority of the assembly's manifest resources.
        /// Used to build a resource name for a file which does not exist yet.
        /// </summary>
        string ResolveForAssembly(Assembly assembly,
                                  DirectoryInfo? projectFolder);

        /// <summary>
        /// The root namespace of one concrete manifest resource name, or null when the name cannot
        /// be attributed to this assembly at all.
        /// </summary>
        string? ResolveForResource(string resourceName,
                                   Assembly assembly,
                                   DirectoryInfo? projectFolder);
    }

    internal sealed class ResourceRootNamespaceResolver : IResourceRootNamespaceResolver
    {
        /// <summary>
        /// Probing every resource of a large test project would cost thousands of File.Exists calls.
        /// A handful of resolvable resources is enough to agree on the root namespace.
        /// </summary>
        private const int MaxProbedResources = 50;

        private static readonly ConcurrentDictionary<(string Assembly, string ProjectFolder), string> AssemblyRoots = new();

        public string ResolveForAssembly(Assembly assembly,
                                         DirectoryInfo? projectFolder)
        {
            var assemblyName = assembly.GetName().Name ?? string.Empty;

            if (projectFolder.IsNull())
            {
                return assemblyName;
            }

            return AssemblyRoots.GetOrAdd((assembly.FullName ?? assemblyName, projectFolder.FullName),
                                          _ => VoteForRootNamespace(assembly, projectFolder, assemblyName));
        }

        public string? ResolveForResource(string resourceName,
                                          Assembly assembly,
                                          DirectoryInfo? projectFolder)
        {
            if (resourceName.IsNullOrWhiteSpace())
            {
                return null;
            }

            // 1. The file is already on disk - that is the only exact answer we can get.
            if (projectFolder.IsNotNull())
            {
                var probed = ProbeRootNamespace(resourceName, projectFolder);

                if (probed.IsNotNull())
                {
                    return probed;
                }
            }

            // 2. Nothing on disk yet (snapshot mode writes a new file). Fall back to whatever the
            //    rest of the assembly agreed on.
            var assemblyRoot = ResolveForAssembly(assembly, projectFolder);

            if (StartsWithRoot(resourceName, assemblyRoot))
            {
                return assemblyRoot;
            }

            // 3. Last resort - the plain assembly name, which is the default RootNamespace.
            var assemblyName = assembly.GetName().Name ?? string.Empty;

            return StartsWithRoot(resourceName, assemblyName) ? assemblyName : null;
        }

        private static bool StartsWithRoot(string resourceName,
                                           string root)
        {
            return root.IsNotNullOrWhiteSpace() &&
                   resourceName.StartsWith(root + ".", StringComparison.OrdinalIgnoreCase);
        }

        private static string VoteForRootNamespace(Assembly assembly,
                                                   DirectoryInfo projectFolder,
                                                   string assemblyName)
        {
            var votes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var resourceName in assembly.GetManifestResourceNames().Take(MaxProbedResources))
            {
                var root = ProbeRootNamespace(resourceName, projectFolder);

                if (root.IsNull())
                {
                    continue;
                }

                votes[root] = votes.TryGetValue(root, out var count) ? count + 1 : 1;
            }

            if (votes.Count.EqualsTo(0))
            {
                return assemblyName;
            }

            // A tie means the assembly mixes roots (LogicalName overrides). The shorter root strips
            // fewer segments and therefore keeps the longer - more specific - folder path.
            return votes.OrderByDescending(vote => vote.Value)
                        .ThenBy(vote => vote.Key.Length)
                        .First()
                        .Key;
        }

        /// <summary>
        /// Splits "Root.Ns.Folder.Sub.File.json" at every possible point and returns the head of the
        /// first split whose tail is an existing file below the project folder.
        /// </summary>
        private static string? ProbeRootNamespace(string resourceName,
                                                  DirectoryInfo projectFolder)
        {
            var parts = resourceName.Split('.');

            // At least one root segment plus "File" plus "json".
            if (parts.Length < 3)
            {
                return null;
            }

            var fileName = $"{parts[^2]}.{parts[^1]}";
            var namespaceSegments = parts[..^2];

            // A root namespace always has at least one segment, so skip starts at 1. Ascending order
            // prefers the longest folder path, which is the least likely accidental match.
            for (var skip = 1; skip <= namespaceSegments.Length; skip++)
            {
                var folderSegments = namespaceSegments[skip..];

                var candidate = folderSegments.Length > 0
                                    ? Path.Combine(projectFolder.FullName, Path.Combine(folderSegments), fileName)
                                    : Path.Combine(projectFolder.FullName, fileName);

                if (File.Exists(candidate))
                {
                    return string.Join(".", namespaceSegments[..skip]);
                }
            }

            return null;
        }
    }
}