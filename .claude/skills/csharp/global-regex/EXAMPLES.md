# Global Regex Examples

## Prefer: Centralized GlobalRegex

```csharp
using System.Text.RegularExpressions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static partial class GlobalRegex
    {
        [GeneratedRegex(@"\[(\d+)\](?=\.)")]
        internal static partial Regex IndexReplacement();

        [GeneratedRegex(@"(?:^|/)v(\d+(?:\.\d+)?)(?:/|$|\?)", RegexOptions.IgnoreCase)]
        internal static partial Regex UrlVersionPattern();

        [GeneratedRegex(@"^\d{3}-\d{2}-\d{4}$")]
        internal static partial Regex SocialSecurityNumber();
    }
}
```

Consumer:

```csharp
internal sealed class ApiVersionResolver : IApiVersionResolver
{
    public string? Resolve(string url)
    {
        var match = GlobalRegex.UrlVersionPattern().Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }
}
```

Consumer:

```csharp
internal sealed class IndexNormalizer
{
    public string Normalize(string path)
    {
        return GlobalRegex.IndexReplacement().Replace(path, "[].");
    }
}
```

## Avoid: Scattered Regex in Implementation Classes

```csharp
internal sealed partial class ApiVersionResolver : IApiVersionResolver
{
    [GeneratedRegex(@"(?:^|/)v(\d+(?:\.\d+)?)(?:/|$|\?)", RegexOptions.IgnoreCase)]
    private static partial Regex UrlVersionPattern();

    public string? Resolve(string url)
    {
        var match = UrlVersionPattern().Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }
}
```

```csharp
internal sealed partial class IndexNormalizer
{
    [GeneratedRegex(@"\[(\d+)\](?=\.)")]
    private static partial Regex IndexReplacement();

    public string Normalize(string path)
    {
        return IndexReplacement().Replace(path, "[].");
    }
}
```

## Avoid: Multiple Partial Classes for Regex Hosting

```csharp
internal sealed partial class ValidationHelpers
{
    [GeneratedRegex(@"^\d{3}-\d{2}-\d{4}$")]
    private static partial Regex SocialSecurityNumber();
}

internal sealed partial class ValidationHelpers
{
    public bool IsValidSsn(string value)
    {
        return SocialSecurityNumber().IsMatch(value);
    }
}
```

## Key Differences

**Prefer**: 
- All regex patterns centralized in `GlobalRegex`
- Clean implementation classes without partial keyword noise
- Easy to discover all regex patterns in one place
- Reusable across multiple consumers

**Avoid**: 
- Regex scattered across many implementation classes
- Partial class pollution just for hosting regex
- Hard to discover what patterns exist
- Duplicate patterns when needed in multiple places
