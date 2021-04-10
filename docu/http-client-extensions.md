# HttpClient extensions

## Use json string to send data
Important to know when you try to send large data you get performance issues. To avoid them send large objects
as json string.

```csharp
public async Task Post_Data_As_Json_String()
{
    var postResponse = await Client.PostAsJsonStringAsync("myApi/data", "Your json string here").ConfigureAwait(false);

    // Also possible with generic return type
    var myResult = await Client.PostAsJsonStringAsync<MyResult>("myApi/data", "Your json string here").ConfigureAwait(false);
}

public async Task Put_Data_As_Json_String()
{
    var putResponse = await Client.PutAsJsonStringAsync("myApi/data", "Your json string here").ConfigureAwait(false);

    // rest of your test code here    
    var myResult = await Client.PutAsJsonStringAsync<MyResult>("myApi/data", "Your json string here").ConfigureAwait(false);
}
```