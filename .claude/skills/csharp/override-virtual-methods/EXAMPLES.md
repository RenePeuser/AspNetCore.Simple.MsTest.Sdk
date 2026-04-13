# Overriding Virtual Methods Examples

## Good

```csharp
protected override void OnConfigurationChanged()
{
    base.OnConfigurationChanged();

    cache.Invalidate();
    logger.LogInformation("Configuration changed");
}
```

Why this is good:
- Extends base behavior instead of silently replacing it
- Keeps framework hooks intact

---

## Intentional Exception

```csharp
// Base validation is not applicable for schema v2.
protected override void Validate()
{
    schemaV2Validator.Validate(this);
}
```

Why this is acceptable:
- Base behavior is intentionally replaced
- The reason is explicit in the code
