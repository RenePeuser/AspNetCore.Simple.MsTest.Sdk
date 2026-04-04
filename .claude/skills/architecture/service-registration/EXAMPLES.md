# Service Registration Examples

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
