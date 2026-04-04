# Constructor Overloads Examples

## Prefer: Proper Constructor Chaining

```csharp
public sealed class ExampleService
{
    private readonly string _name;
    private readonly bool _enabled;
    private readonly int _retryCount;

    public ExampleService(string name)
        : this(name, enabled: true)
    {
    }

    public ExampleService(string name, bool enabled)
        : this(name, enabled, retryCount: 3)
    {
    }

    public ExampleService(string name, bool enabled, int retryCount)
    {
        _name = name;
        _enabled = enabled;
        _retryCount = retryCount;
    }
}
```

## Avoid: Duplicate Initialization

```csharp
public sealed class ExampleService
{
    private readonly string _name;
    private readonly bool _enabled;
    private readonly int _retryCount;

    public ExampleService(string name)
    {
        _name = name;
        _enabled = true;
        _retryCount = 3;
    }

    public ExampleService(string name, bool enabled)
    {
        _name = name;
        _enabled = enabled;
        _retryCount = 3;
    }

    public ExampleService(string name, bool enabled, int retryCount)
    {
        _name = name;
        _enabled = enabled;
        _retryCount = retryCount;
    }
}
```

## Key Difference

**Prefer**: State initialization happens in ONE place (maximum constructor)
**Avoid**: State initialization duplicated across ALL constructors
