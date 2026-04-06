# Global Regex Review Checklist

Use this checklist when reviewing regular expression usage in the codebase.

## Structure

- [ ] Is `[GeneratedRegex]` used for regex patterns?
- [ ] Does the project have a `GlobalRegex` class?
- [ ] Is `GlobalRegex` defined as `internal static partial class`?
- [ ] Are all regex patterns centralized in `GlobalRegex`?
- [ ] Is the `GlobalRegex` class located in the project root namespace?

## Implementation Classes

- [ ] Are implementation classes (services, handlers, resolvers) free of `[GeneratedRegex]` attributes?
- [ ] Do consumers call `GlobalRegex.PatternName()` instead of defining their own regex?
- [ ] Have partial class declarations been removed from implementation classes that only hosted regex?
- [ ] Are implementation classes focused on their primary responsibility without regex hosting noise?

## Pattern Quality

- [ ] Do regex factory method names clearly describe the pattern purpose?
- [ ] Are regex patterns reusable within the project scope?
- [ ] Are regex options (e.g., `RegexOptions.IgnoreCase`) specified when appropriate?
- [ ] Are regex patterns well-tested and validated?

## Scope and Visibility

- [ ] Is `GlobalRegex` marked as `internal` to keep it project-scoped?
- [ ] Has cross-project regex sharing been avoided unless explicitly required?
- [ ] Are regex factory methods marked as `internal static partial`?

## Migration from Scattered Regex

If refactoring existing code:

- [ ] Have all `[GeneratedRegex]` attributes been moved to `GlobalRegex`?
- [ ] Have all consuming classes been updated to call `GlobalRegex` methods?
- [ ] Have partial class keywords been removed from implementation classes when no longer needed?
- [ ] Have duplicate regex patterns been consolidated into a single definition?

## Anti-Patterns to Avoid

- [ ] No `[GeneratedRegex]` in services, handlers, resolvers, or endpoints
- [ ] No partial implementation classes solely for hosting regex methods
- [ ] No scattered regex definitions across multiple classes
- [ ] No duplicate regex patterns defined in multiple places
- [ ] No cross-project regex dependencies unless explicitly designed

## Documentation

- [ ] Are complex regex patterns commented to explain their purpose?
- [ ] Is the regex pattern format documented when non-obvious?
- [ ] Are edge cases or special matching rules documented?
