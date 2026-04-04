# Overriding Virtual Methods

## Objective

Call `base` by default when overriding a virtual method.

Treat an override as an extension of base behavior unless the base implementation is being intentionally and fully replaced.

## Apply These Rules

- Call `base` by default when overriding a virtual member.
- Skip the `base` call only when replacing the base behavior is intentional and understood.
- Document the reason when an override intentionally does not call `base`.
- Be especially careful with framework, SDK, and third-party base classes.
- Treat missing `base` calls as a review red flag.

## Prefer

```csharp
protected override void OnConfigurationChanged()
{
    base.OnConfigurationChanged();

    _cache.Invalidate();
    LogConfigChange();
}
```

## Acceptable Exception

```csharp
// Replacing base validation intentionally.
// Base validation is not applicable for schema v2.
protected override void Validate()
{
    _schemaV2Validator.Validate(this);
}
```

## Avoid

```csharp
protected override void Initialize()
{
    _customState = new State();
}
```

## Review Checklist

- Does the override call `base`?
- If not, is the replacement intentional?
- Is the reason documented?
- Could base behavior contain important initialization, validation, or hooks?
- Is the override especially risky because the base type comes from a framework or SDK?

## Notes

In this repository, the default assumption is: overriding means extending, not silently replacing.

A missing `base` call is acceptable only when the base behavior is intentionally replaced and that decision is clear in the code.
