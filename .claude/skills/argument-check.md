---
name: argument-check
description: Use when adding or reviewing guard clauses with the Argument.Check package in this .NET codebase.
---

# Argument Check

Use this skill when adding or reviewing guard clauses with `Argument.Check`.

## Objective

Use `Throw.*` helpers for clear and consistent argument checks where defensive validation is actually needed.

## Apply These Rules

- Use `Argument.Check` for guard clauses at clear boundaries such as constructors, public methods, and critical inputs.
- Add checks only where invalid input would otherwise break behavior or make the code unsafe.
- Prefer the most specific `Throw.*` helper available.
- Prefer concise throw-expression style for required dependencies.
- Do not add checks mechanically to every variable or every internal call.
- Do not use `Argument.Check` as a replacement for domain validation or business rules.

## Prefer

```csharp
_dependency = Throw.IfNull(dependency);
Throw.IfNullOrWhiteSpace(name);
Throw.IfOutOfRange(retryCount, 0, 10);
```

## Avoid

- Adding guard clauses everywhere without a clear reason
- Re-checking values that are already guaranteed by design
- Using custom predicates when a dedicated helper already exists
- Mixing guard clauses with domain or workflow validation

## Review Checklist

- Is the check placed at a meaningful boundary?
- Is the check actually needed?
- Is the most specific `Throw.*` helper used?
- Is the code clearer because of the check?
- Is this still guard-clause validation and not business validation?

## Notes

Use `Argument.Check` where it improves safety and clarity.
Do not force it into places where the code already has clear and reliable guarantees.
