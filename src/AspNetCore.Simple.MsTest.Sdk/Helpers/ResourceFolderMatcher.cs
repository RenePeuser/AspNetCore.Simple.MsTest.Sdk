using System;
using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Tells whether a manifest resource name sits in one of a set of folders.
    ///
    /// Resource names are dotted, so a folder is a segment and nothing else: ".Responses." matches,
    /// "MyResponses." does not. The resolution uses this to scope a lookup to the folders that belong
    /// to the role being looked up, and the not-found error uses the same rule to keep its suggestions
    /// inside that role.
    /// </summary>
    internal static class ResourceFolderMatcher
    {
        public static bool ContainsFolderSegment(string resourceName,
                                                 IImmutableSet<string> folders)
        {
            foreach (var folder in folders)
            {
                if (resourceName.Contains("." + folder + ".",
                                          StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
