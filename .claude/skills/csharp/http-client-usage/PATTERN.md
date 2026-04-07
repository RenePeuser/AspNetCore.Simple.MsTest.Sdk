# HttpClient Usage

Use this skill when introducing, changing, or reviewing outbound HTTP calls.

## Objective

Avoid socket exhaustion and stale connection issues by using `IHttpClientFactory` for application HTTP access.

Do not create and dispose `HttpClient` instances per request.

Instead, register HTTP clients through dependency injection and obtain configured clients from the factory.

## Apply These Rules

- Prefer `IHttpClientFactory` for application code that calls external HTTP services.
- Do not use `new HttpClient()` inside request handlers, services, loops, or frequently-called methods.
- Do not wrap per-request `HttpClient` instances in `using` blocks.
- Centralize HTTP client configuration with `AddHttpClient()` registration.
- Use **named clients** when several call sites share the same configuration.
- Use **typed clients** when an external API deserves a dedicated class and clear abstraction.
- Put `BaseAddress`, timeouts, and default headers in registration instead of repeating them at call sites.
- Keep request-specific data such as paths, query strings, and payloads at the call site.
- Continue to dispose `HttpRequestMessage` and `HttpResponseMessage` appropriately when needed, but not the factory-managed client lifetime itself.
- For resilience, prefer policies or handler configuration attached during registration.

## Prefer

```csharp
builder.Services.AddHttpClient("MyClient", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp");
});
```

```csharp
internal sealed class MyService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MyService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> FetchDataAsync(string endpoint, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("MyClient");
        using var response = await client.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

```csharp
builder.Services.AddHttpClient<MyApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

```csharp
internal sealed class MyApiClient
{
    private readonly HttpClient _httpClient;

    public MyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetResourceAsync(string endpoint, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

## Avoid

```csharp
public async Task FetchDataAsync(string url)
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();
}
```

```csharp
public async Task FetchManyAsync(IEnumerable<string> urls)
{
    foreach (var url in urls)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
```

## Review Checklist

- Is `HttpClient` created through `IHttpClientFactory` instead of `new HttpClient()`?
- Are per-call `using var client = new HttpClient();` patterns avoided?
- Is shared configuration centralized in `AddHttpClient()`?
- Does the code use a named or typed client when configuration is reusable?
- Are `BaseAddress`, timeout, and headers configured once instead of repeated?
- Are request and response lifetimes handled correctly without fighting factory-managed pooling?
- If resilience is needed, is it attached through handlers or policies instead of ad-hoc retry loops?

## Notes

The main anti-pattern is creating a fresh `HttpClient` for each call. Even after disposal, underlying connections remain in `TIME_WAIT`, which can exhaust available sockets under load.

`IHttpClientFactory` solves this by managing handler lifetimes and connection reuse. It also makes configuration discoverable and enables named or typed client patterns.

For most application code in this repository, prefer factory-managed clients over manually-managed `HttpClient` instances.
