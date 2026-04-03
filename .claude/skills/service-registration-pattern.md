---
name: service-registration
description: Use when creating, refactoring, or reviewing dependency injection registration in this .NET codebase.
---

# Service Registration

Use this skill when working on dependency injection registration code.

## Objective

Keep service registration readable, hierarchical, and feature-oriented.

Prefer a structure where top-level feature registration calls delegate dependency registration to the owning service or feature.

## Apply These Rules

- Prefer feature-based registration over flat technical registration blocks.
- A consumer should register a feature, not manually register all of its internal dependencies.
- Prefer `AddXxx()` extension methods to encapsulate registration logic.
- Place the `AddXxx()` extension method in the same file as the service class (not in separate extension files).
- Register dependencies before the service itself.
- Register configuration/settings before the service itself when required.
- Register the primary service last.
- Prefer idempotent registration methods such as `AddSingletonIfNotExists` where the project uses that pattern.

## Avoid

- Large flat registration blocks in startup code
- Direct registration of implementation details from outside the owning feature
- Placing `AddXxx()` extension methods in separate files from the service class
- Scattering registration logic across unrelated files
- Making consumers responsible for knowing internal dependency trees

## Preferred Shape

```csharp
public override void ConfigureServices(IServiceCollection services)
{
    services.AddOwnership();
    services.AddOpportunities();
    services.AddSyncStore();
}
```

## Preferred Extension Structure

```csharp
public static class AddMyServiceExtension
{
    public static void AddMyService(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Dependencies
        services.AddDependency1();

        // 2. Settings
        services.AddSomeSettings(configuration);

        // 3. Service itself
        services.AddSingletonIfNotExists<MyService, MyService>();
    }
}
```

## Review Checklist

When reviewing service registration code, verify:

- Is the registration feature-oriented?
- Does the service own its dependency registration?
- Are dependencies registered through their own `AddXxx()` extensions where appropriate?
- Are settings/configuration registered before the service?
- Is the service registered last?
- Is the registration safe to call multiple times if the project expects idempotency?
- Is the structure easy to navigate and understand?

## Notes

This skill defines the preferred registration pattern for this repository.
If more detail or examples are needed, consult the project rule or example files for service registration.
