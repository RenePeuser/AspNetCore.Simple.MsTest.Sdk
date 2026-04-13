# Test Endpoint Examples

## Example 1: Happy Path GET Test

```csharp
namespace YourApi.Test.Api.Projects.V1.GetCurrent.Status_200_Ok;

[TestClass]
[TestCategory("MinimalApi")]
[TestCategory("MVP1")]
[TestCategory("Projects")]
[TestCategory("Projects V1 GetCurrent Status 200 OK")]
public class GetCurrentStatus200OkTest : ApiTestBase
{
    [TestMethod]
    public async Task Should_Be_Able_To_Get_Current_Project()
    {
        await Client.AssertGetAsync<GetCurrentProjectResponse>(
            "api/console/v1/projects/current",
            "GetCurrentProject.json").ConfigureAwait(false);
    }
}
```

Why this is good:
- Test path mirrors the endpoint operation
- Includes `[TestCategory("MinimalApi")]`
- Uses the repository assertion helper
- Keeps the test focused on one status code

---

## Example 2: POST Test with Captured Response Fixture

```csharp
namespace YourApi.Test.Api.Capabilities.V1.Create.Status_200_Ok;

[TestClass]
[TestCategory("MinimalApi")]
[TestCategory("MVP1")]
[TestCategory("Capabilities")]
[TestCategory("Capabilities V1 Create Status 200 OK")]
public class CreateStatus200OkTest : ApiTestBase
{
    private static readonly string UniqueName = GetUniqueRunnerName();

    [TestMethod]
    public async Task Should_Be_Able_To_Create_A_Capability()
    {
        await Client.AssertPostAsync<CreateCapabilityResponse>(
            "api/console/v1/capabilities",
            "CreateCapability.json",
            "CreateCapability.json",
            parameters: [("$UniqueName$", UniqueName)],
            writeResponse: true).ConfigureAwait(false);
    }
}
```

After generating the fixture, remove `writeResponse: true` and re-run the same test.

---

## Example 3: Unprocessable Content with DataRow

```csharp
[TestClass]
[TestCategory("MinimalApi")]
[TestCategory("Capabilities")]
public class CreateStatus422UnprocessableContentTest : ApiTestBase
{
    [TestMethod]
    [DataRow("InvalidName.json", "InvalidName.json")]
    [DataRow("InvalidOwner.json", "InvalidOwner.json")]
    public Task Should_Return_Unprocessable_Content_If_Request_Violates_Rules(string request, string response)
    {
        return Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(
            "api/console/v1/capabilities",
            request,
            response);
    }
}
```

Why this is good:
- Uses one focused status-code class
- Shares similar scenarios through `[DataRow]`
- Uses the correct problem details shape for the area

---

## Example 4: Cleanup for Data-Creating Tests

```csharp
protected override async Task TestCleanupAsync()
{
    await TestClient.Capabilities.V1.DeleteAllAsync(GetUniqueRunnerName()).ConfigureAwait(false);
    await base.TestCleanupAsync().ConfigureAwait(false);
}
```

Why this is useful:
- Explicit cleanup keeps tests repeatable
- Cleanup stays inside the repository's test infrastructure model
