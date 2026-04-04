# Strategy Pattern Examples

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
