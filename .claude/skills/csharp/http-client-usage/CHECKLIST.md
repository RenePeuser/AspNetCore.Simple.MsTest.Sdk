# HttpClient Usage Checklist

Use this checklist when reviewing outbound HTTP code.

## Lifetime and Construction

- `HttpClient` is not created with `new HttpClient()` in normal application flow
- No `using var client = new HttpClient();` per request
- Client instances come from `IHttpClientFactory` or a typed client registration

## Configuration

- Shared configuration is registered with `AddHttpClient()`
- `BaseAddress` is not repeated unnecessarily at every call site
- Timeout and default headers are configured centrally when reused
- Named client is used when multiple consumers share one configuration
- Typed client is used when an external API has a dedicated wrapper

## Call-Site Quality

- Request-specific paths and payloads remain at the call site
- `EnsureSuccessStatusCode()` is used when failure should throw
- `CancellationToken` is accepted and forwarded where appropriate
- Responses or request messages are disposed when needed

## Reliability

- Resilience is attached through handlers or built-in resilience configuration when appropriate
- No ad-hoc retry loops that hide repeated failures
- The code avoids patterns that can cause socket exhaustion under load

## Smells

Watch for these red flags:

- `new HttpClient()` inside methods
- `using` around `HttpClient` for every request
- Setting the same `BaseAddress` repeatedly
- Many call sites duplicating header setup
- HTTP logic mixed directly into unrelated orchestration classes
