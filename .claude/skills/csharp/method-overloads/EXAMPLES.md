# Method Overloads Examples

## Prefer: Proper Overload Forwarding

```csharp
public static Task ProcessAsync(string value)
{
    return ProcessAsync(value, option: false);
}

public static Task ProcessAsync(string value, bool option)
{
    return ProcessAsync(value, option, CancellationToken.None);
}

public static Task ProcessAsync(
    string value,
    bool option,
    CancellationToken cancellationToken)
{
    var request = new ProcessRequest
    {
        Value = value,
        Option = option
    };

    return ExecuteAsync(request, cancellationToken);
}
```

## Avoid: Duplicate Logic

```csharp
public static Task ProcessAsync(string value)
{
    var request = new ProcessRequest { Value = value, Option = false };
    return ExecuteAsync(request, CancellationToken.None);
}

public static Task ProcessAsync(string value, bool option)
{
    var request = new ProcessRequest { Value = value, Option = option };
    return ExecuteAsync(request, CancellationToken.None);
}

public static Task ProcessAsync(
    string value,
    bool option,
    CancellationToken cancellationToken)
{
    var request = new ProcessRequest { Value = value, Option = option };
    return ExecuteAsync(request, cancellationToken);
}
```

## Key Difference

**Prefer**: Core logic centralized in ONE maximum overload
**Avoid**: Request creation and logic duplicated across ALL overloads
