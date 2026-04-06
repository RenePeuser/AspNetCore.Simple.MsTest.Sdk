# Global Regex

Use this skill when introducing, changing, or reviewing regular expressions.

## Objective

Keep generated regular expressions centralized per project.

Do not spread `[GeneratedRegex]` partial methods across normal services, handlers, resolvers, or other implementation classes.

Instead, place them in one project-local `internal static partial class GlobalRegex`.

## Apply These Rules

- Store generated regular expressions in a single `GlobalRegex` type per project.
- Use `internal static partial class GlobalRegex`.
- Keep regex definitions out of normal implementation classes such as services, resolvers, handlers, endpoints, or providers.
- Do not introduce additional partial classes only to host `[GeneratedRegex]` methods.
- Keep regex sharing local to the project. Do not create cross-project regex sharing by default.
- Expose regex factory methods from `GlobalRegex` with clear names that describe the pattern purpose.
- Let consuming classes call `GlobalRegex.SomePattern()` instead of defining their own generated regex methods.
- Keep the class `internal` so the regex catalog stays project-scoped.

## Prefer

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
    }
}
```

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

## Avoid

```csharp
internal sealed partial class ApiVersionResolver : IApiVersionResolver
{
    [GeneratedRegex(@"(?:^|/)v(\d+(?:\.\d+)?)(?:/|$|\?)", RegexOptions.IgnoreCase)]
    private static partial Regex UrlVersionPattern();
}
```

## Review Checklist

- Is `[GeneratedRegex]` used?
- If yes, is the regex placed in the project-local `GlobalRegex` type?
- Is the consuming implementation free of regex-hosting partial class noise?
- Is the regex name descriptive and reusable inside the project?
- Is the regex kept project-local with `internal` visibility?
- Has cross-project sharing been avoided unless explicitly required?

## Notes

The default repository rule is:

- one `GlobalRegex` per project
- `internal static partial class GlobalRegex`
- no scattered regex-hosting partial implementation classes

This keeps regex definitions centralized, reduces structural noise, and avoids unnecessary partial classes throughout the codebase.
