# Request Validation Examples

## Example 1: Attribute-First Request DTO

```csharp
public sealed record CreateNodeRequest
{
    [IsNotNull]
    [NoTrailingWhitespaces]
    [NoLeadingWhitespaces]
    [IsNotEmpty]
    [IsNotWhitespace]
    public required string Id { get; init; }

    [NoTrailingWhitespaces]
    [NoLeadingWhitespaces]
    [IsNotEmpty]
    [IsNotWhitespace]
    public string? Label { get; init; }

    [MaxLength(1000)]
    public string? Note { get; init; }

    [IsNotNull]
    [EnumIsDefined]
    public required CapabilityTypeEnum CapabilityType { get; init; }

    [GuidIsNotEmpty]
    public Guid? ReferenceId { get; init; }
}
```

Why this is good:
- Baseline validation is directly visible on the request DTO
- Property-level rules do not need to be duplicated in imperative validator code
- The request type remains in `YourApi.Contracts`

---

## Example 2: Async Request Validator for External Lookups

```csharp
internal static class AddCreateNodeRequestValidatorExtension
{
    internal static void AddCreateNodeRequestValidator(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<CreateNodeRequestValidator>();
    }
}

internal sealed class CreateNodeRequestValidator(
    IAttributeValidator attributeValidator,
    GetCapabilityByIdOrDefaultQuery getCapabilityByIdOrDefaultQuery)
    : AsyncRequestValidator<CreateNodeRequest>(attributeValidator)
{
    protected override async IAsyncEnumerable<PropertyValidationResult> GetValidationErrorsAsync(
        CreateNodeRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.ReferenceId is null || request.ReferenceId == Guid.Empty)
        {
            yield break;
        }

        var capability = await getCapabilityByIdOrDefaultQuery.ExecuteAsync(
            request.ReferenceId.Value,
            DeploymentConstants.DefaultDeploymentId,
            cancellationToken).ConfigureAwait(false);

        if (capability is null)
        {
            yield return new PropertyValidationResult(
                nameof(request.ReferenceId),
                new ValidationErrorDetails
                {
                    CurrentValue = request.ReferenceId,
                    Errors = [$"ReferenceId '{request.ReferenceId}' does not reference an existing capability."],
                    Samples = ["3fa85f64-5717-4562-b3fc-2c963f66afa6"]
                });
        }
    }
}
```

Why this is good:
- Attribute validation handles baseline rules
- Async validator adds the lookup-based rule
- The validator does not block on IO

---

## Example 3: Command Validates Before Processing

```csharp
internal sealed class CreateNodeCommand(CreateNodeRequestValidator requestValidator,
                                        CreateNodeRequestMapper requestMapper)
{
    internal async Task<Node> ExecuteAsync(CreateNodeRequest request,
                                           CancellationToken cancellationToken)
    {
        await requestValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        var entity = requestMapper.MapFrom(request);
        return await SaveAsync(entity, cancellationToken).ConfigureAwait(false);
    }
}
```

Why this is good:
- Validation is explicit
- Validation happens before mapping and persistence
- The command flow is easy to follow

---

## Example 4: Sync Validator for Pure In-Memory Rules

```csharp
internal sealed class CreateDeploymentRequestValidator(IAttributeValidator attributeValidator)
    : RequestValidator<CreateDeploymentRequest>(attributeValidator)
{
    protected override IEnumerable<PropertyValidationResult> GetValidationErrors(CreateDeploymentRequest request)
    {
        if (request.Stage == Stage.Workbench)
        {
            yield return new PropertyValidationResult(
                nameof(request.Stage),
                new ValidationErrorDetails
                {
                    CurrentValue = request.Stage,
                    Errors = [$"You cannot deploy to '{nameof(Stage.Workbench)}'."],
                    Samples = [Stage.Development]
                });
        }
    }
}
```

Why this is good:
- No async work is needed
- Pure rule validation stays simple
- `RequestValidator<TRequest>` is sufficient

---

## Example 5: Custom Async Validator That Composes Attribute Validation

```csharp
internal static class AddCreateEdgeRequestValidatorExtension
{
    internal static void AddCreateEdgeRequestValidator(this IServiceCollection services,
                                                       IConfiguration configuration)
    {
        services.AddAmazonDynamoDbClientFactory(configuration);
        services.AddGetNodeByIdOrDefaultQuery(configuration);
        services.AddGetCapabilityByIdOrDefaultQuery(configuration);
        services.AddSingletonIfNotExists<CreateEdgeRequestValidator>();
    }
}

internal sealed class CreateEdgeRequestValidator(
    GetAllCapabilityTypeInfosQuery getAllCapabilityTypeInfos,
    GetNodeByIdOrDefaultQuery getNodeByIdQuery,
    GetCapabilityByIdOrDefaultQuery getCapabilityByIdQuery,
    IAttributeValidator attributeValidator)
    : AsyncRequestValidator<CreateEdgeRequest>(attributeValidator)
{
    protected override async IAsyncEnumerable<PropertyValidationResult> GetValidationErrorsAsync(
        CreateEdgeRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.SourceNodeId.EqualsTo(request.TargetNodeId))
        {
            yield return new PropertyValidationResult(
                nameof(request.SourceNodeId),
                new ValidationErrorDetails
                {
                    CurrentValue = request.SourceNodeId,
                    Errors = [$"{nameof(request.SourceNodeId)} must not be the same as {nameof(request.TargetNodeId)}."],
                    Samples = ["b0a2a74d-c1f0-403b-bc16-f46768baaef3"]
                });
        }

        if (request.SourceNodeId.IsNullOrWhiteSpace() || request.TargetNodeId.IsNullOrWhiteSpace())
        {
            yield break;
        }

        // Additional async graph and relation validation here...
    }
}
```

Why this is good:
- The custom validator keeps the baseline attribute validation through `IAttributeValidator`
- It adds richer async rules that cannot live in request attributes
- The command can stay focused on processing because graph-aware validation lives in the validator

---

## Example 6: What to Avoid

```csharp
public sealed record CreateSomethingRequest
{
    public required string Name { get; init; }
}
```

```csharp
internal sealed class CreateSomethingCommand
{
    internal Task ExecuteAsync(CreateSomethingRequest request)
    {
        // No validation before processing
        return Task.CompletedTask;
    }
}
```

Why this is weak:
- No baseline validation attributes on the request DTO
- Validation intent is hidden or missing
- The processing flow gives no explicit validation step
