---
name: strategy-pattern
description: Use when designing or reviewing extensible branching logic in this .NET codebase.
---

# Strategy Pattern

Use this skill when code contains growing branching logic such as `if/else`, `switch`, or mode/type-based selection.

## Objective

Use the Strategy Pattern when behavior varies by case and the system is expected to grow.

By default, strategy resolution must be strict:

- exactly one strategy must match
- zero matches must fail explicitly
- multiple matches must fail explicitly

The default repository model is not silent fallback behavior.

## Apply These Rules

- Consider the Strategy Pattern when there are already more than two meaningful cases.
- Consider it earlier when future extension is likely.
- Prefer strategies over growing `if/else` or `switch` logic when behavior should be open for extension.
- Keep orchestration separate from case-specific behavior.
- Use a clear contract such as `CanHandle(...)` and `Execute(...)` or a domain-specific equivalent.
- Register strategies via DI and resolve them centrally.
- By default, strategy resolution must be strict: exactly one strategy must match.
- If no strategy matches, fail explicitly.
- If more than one strategy matches, fail explicitly.
- Do not introduce silent fallback behavior unless the use case explicitly requires it.
- Strategies should defensively validate that they are actually allowed to handle the given input, even if the orchestrator already selected them.
- Introduce a specific abstract base class when many strategies share repeated mechanics such as type checks, casting, guards, or shared helpers.

## Preferred Contracts

A small orchestrator plus many specific strategies is the preferred default.

```csharp
public interface ISpecificResponseTypeHandler
{
    bool CanHandle<TResult>(HttpResponseMessage responseMessage);

    Task<TResult> HandleAsync<TResult>(
        HttpResponseMessage responseMessage,
        HttpMethod httpMethod,
        HttpClient httpClient,
        string url);
}
```

## Preferred Orchestrator Shape

The orchestrator owns selection and enforces strict resolution.

Use a detailed repository exception type instead of generic exceptions when strict strategy resolution fails.

```csharp
public sealed class ResponseTypeHandleStrategy(IEnumerable<ISpecificResponseTypeHandler> handlers)
{
    public Task<TResult> HandleAsync<TResult>(
        HttpResponseMessage responseMessage,
        HttpMethod httpMethod,
        HttpClient httpClient,
        string url)
    {
        var responseHandlers = handlers
            .Where(handler => handler.CanHandle<TResult>(responseMessage))
            .ToImmutableList();

        if (responseHandlers.IsEmpty)
        {
            throw new InternalServerErrorDetailsException(
                "No response handler was found to handle expected response",
                $"No response handler was found to handle expected response type: '{typeof(TResult).Name}'",
                ("ResponseType", typeof(TResult).Name),
                ("HttpStatusCode", responseMessage.StatusCode.ToString()),
                ("IsSuccessStatusCode", responseMessage.IsSuccessStatusCode.ToString()));
        }

        if (responseHandlers.Count > 1)
        {
            throw new InternalServerErrorDetailsException(
                "More than one response handler was found for expected response type",
                $"For response type: '{typeof(TResult).Name}' {responseHandlers.Count} response handlers were found",
                ("ResponseType", typeof(TResult).Name),
                ("HandlerCount", responseHandlers.Count.ToString()),
                ("ResponseHandlers", responseHandlers.Select(handler => handler.GetType().Name).ToImmutableList()));
        }

        return responseHandlers[0].HandleAsync<TResult>(responseMessage, httpMethod, httpClient, url);
    }
}
```

## Preferred Strategy Shape

A specific strategy should still protect itself even if the orchestrator normally guarantees correct selection.

Use a detailed repository exception type that explains the failure clearly.

```csharp
internal sealed class ByteArrayResponseTypeHandler : ISpecificResponseTypeHandler
{
    public bool CanHandle<TResult>(HttpResponseMessage responseMessage)
    {
        return responseMessage.IsSuccessStatusCode &&
               typeof(TResult).EqualsTo(typeof(byte[]));
    }

    public async Task<TResult> HandleAsync<TResult>(
        HttpResponseMessage responseMessage,
        HttpMethod httpMethod,
        HttpClient httpClient,
        string url)
    {
        // Safety first: this method can be called without checking CanHandle.
        if (CanHandle<TResult>(responseMessage).IsFalse())
        {
            throw new InternalServerErrorDetailsException(
                "ByteArrayResponseTypeHandler was called without checking CanHandle.",
                $"The response type handler for type: '{typeof(TResult).Name}' is not supported by {nameof(ByteArrayResponseTypeHandler)}.",
                ("Handler", nameof(ByteArrayResponseTypeHandler)),
                ("RequestedType", typeof(TResult).Name),
                ("SupportedTypes", "byte[]"),
                ("HttpStatusCode", responseMessage.StatusCode.ToString()),
                ("IsSuccessStatusCode", responseMessage.IsSuccessStatusCode.ToString()));
        }

        var result = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

        return (TResult)result.Cast<object>();
    }
}
```

## Exception Guidance

Prefer a specific repository exception type for invalid strategy resolution or invalid strategy execution.

Examples:

- `InternalServerErrorDetailsException`
- another repository-specific exception that can carry title, detail, and structured metadata

Avoid throwing plain `InvalidOperationException` when the repository already has a richer internal error type.

### Exception Rules

- The exception type should communicate that this is an internal strategy-resolution or strategy-execution failure.
- The exception should include both a short title and a meaningful detail message.
- The exception should include structured metadata whenever possible.
- The detail message should explain what was expected and what was actually found.
- Metadata should help debugging without forcing the reader to inspect the code first.

### Good Exception Content

Include details such as:

- requested type
- strategy type
- matching strategy count
- matched strategy names
- supported types
- relevant status code or mode
- other key selection inputs

## When a Specific Base Class Makes Sense

Introduce a specific abstract base class when many strategies repeat the same mechanics.

Typical examples:

- type checks
- casting
- guard clauses
- shared validation
- shared helper methods
- repeated exception-building logic

The base class should reduce duplication without hiding the strategy’s actual decision logic.

## Avoid

- Growing `if/else` chains for case-specific behavior
- Large `switch` statements that must be edited for every new case
- Central services that know every implementation detail
- Silent fallback behavior in the default strategy model
- Returning without a result when exactly one strategy is expected
- Silent ambiguity when multiple strategies match unexpectedly
- Generic exceptions when a richer repository exception type exists
- Exceptions that only contain a title but no useful detail or metadata

## Review Checklist

- Are there already more than two meaningful cases?
- Is future growth of cases likely?
- Would a strategy remove branching from the orchestration layer?
- Is each strategy focused on one responsibility?
- Is selection based on a clear `CanHandle(...)` rule or equivalent?
- Does the orchestrator enforce exactly one match?
- Does the orchestrator fail explicitly for zero matches?
- Does the orchestrator fail explicitly for multiple matches?
- Do strategies defend themselves against invalid direct execution?
- Do exceptions include a useful type, a title, a meaningful detail, and structured metadata?
- Would a shared abstract base class reduce duplication across many strategies?
- Is the design more open for extension and less dependent on central modification?

## Notes

The default strategy model in this repository is:

- one orchestrator
- many specific strategies
- one clear selection rule
- exactly one match
- explicit failure for zero or multiple matches
- defensive strategy execution checks
- detailed repository exceptions
- optional abstract base class for repeated mechanics

Use the Strategy Pattern to support the Open/Closed Principle:
new behavior should usually be added by introducing a new strategy, not by modifying a growing central `if/else` or `switch`.
