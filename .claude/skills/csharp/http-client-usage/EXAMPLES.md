# HttpClient Usage Examples

## Prefer: Named Client for Shared Configuration

```csharp
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "AspNetCore.Simple.MsTest.Sdk");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

```csharp
internal sealed class GitHubService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetRateLimitAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("GitHub");
        using var response = await client.GetAsync("rate_limit", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

## Prefer: Typed Client for Dedicated API Access

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

    public async Task<string> GetOrdersAsync(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync("orders", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

## Prefer: Resilience Attached During Registration

```csharp
builder.Services.AddHttpClient("ResilientClient", client =>
    {
        client.BaseAddress = new Uri("https://api.example.com/");
    })
    .AddStandardResilienceHandler();
```

## Avoid: New Client Per Call

```csharp
internal sealed class ApiService
{
    public async Task<string> GetAsync(string url)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
```

## Avoid: New Client Inside a Loop

```csharp
foreach (var url in urls)
{
    using var client = new HttpClient();
    using var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();
}
```

## Key Differences

**Prefer**:
- Client lifetime managed by `IHttpClientFactory`
- Configuration defined once in DI
- Named or typed clients for reuse and clarity
- Better connection reuse and easier resilience setup

**Avoid**:
- Manual per-call construction and disposal
- Repeating base address and headers everywhere
- Socket exhaustion risk under load
- Hidden outbound HTTP configuration spread across the codebase
