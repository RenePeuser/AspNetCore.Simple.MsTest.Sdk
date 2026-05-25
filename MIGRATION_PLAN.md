# Test Structure Migration Plan

## Goal
Reorganize tests by HTTP operation (GET, POST, PUT, PATCH, DELETE) and add Fluent API test variations alongside existing native extension tests.

---

## Current Structure

```
src/MinimalApi.Test/
└── Api/
    └── Persons/
        ├── PersonEndpointsTests.cs          (217 lines, all operations mixed)
        ├── GetPersonResponse.json
        ├── GetPersonFilteredResponse.json
        ├── NewPerson.json
        ├── NewPersonParameter.json
        ├── SonGoku.json
        ├── SonGokuNewResponse.json
        ├── Requests/
        │   └── SonGoku.json
        ├── Responses/
        │   ├── SonGoku.json
        │   └── InvalidResponseType.txt
        └── AnyFolder/
            ├── P/
            │   └── NewPersonParameter.json
            └── R/
                └── NewPersonParameter.json
```

---

## Target Structure

```
src/MinimalApi.Test/
└── Api/
    └── Persons/
        └── V1/
            ├── Get/
            │   ├── Requests/               (empty - GET has no body)
            │   ├── Responses/
            │   │   ├── PersonListResponse.json
            │   │   ├── PersonFilteredResponse.json
            │   │   └── InvalidResponseType.txt
            │   └── PersonGetTests.cs       (Native + Fluent + FluentEndpoint)
            │
            ├── Create/
            │   ├── Requests/
            │   │   ├── CreatePerson.json
            │   │   ├── CreatePersonParameterized.json
            │   │   └── CreatePersonSonGoku.json
            │   ├── Responses/
            │   │   ├── CreatePerson.json              ← Same name as request!
            │   │   ├── CreatePersonParameterized.json ← Same name as request!
            │   │   └── CreatePersonSonGoku.json       ← Same name as request!
            │   └── PersonCreateTests.cs    (Native + Fluent + FluentEndpoint)
            │
            ├── Update/
            │   ├── Requests/
            │   │   ├── UpdatePersonPut.json
            │   │   └── UpdatePersonPatch.json
            │   ├── Responses/
            │   │   ├── UpdatePersonPut.json   ← Same name as request!
            │   │   └── UpdatePersonPatch.json ← Same name as request!
            │   └── PersonUpdateTests.cs       (Native + Fluent + FluentEndpoint)
            │
            └── _Shared/
                └── TestHelpers.cs          (Shared filter/difference functions)
```

---

## Test Categorization

### GET Tests (7 tests → PersonGetTests.cs)
1. ✅ `Should_Return_Expected_Result_For_Given_Payload_Ignore_Id()` - GET with difference filtering
2. ✅ `Should_Return_Expected_Result_For_Given_Payload_With_Post_Sort()` - GET with filtering
3. ⚠️ `Invalid_Response_Type_Json_Exception()` - GET with wrong type (Ignored in CI)
4. ✅ `Should_Return_Expected_Result_For_Given_Payload()` - GET with inline JSON
5. ✅ `Should_Return_Expected_Result_For_Given_Payload_By_Embedded_File()` - GET basic
6. ✅ `Should_Return_Expected_Result_For_Given_Payload_By_Embedded_File_With_Expected_Status_Code()` - GET with status code
7. ✅ `Should_Filter_Persons_By_Name_Query_Parameter()` - GET with query params
8. ✅ `Should_Throw_Exception_If_Json_Is_Invalid()` - GET with invalid JSON (DataRow)

**JSON Files:**
- `GetPersonResponse.json` → `Responses/PersonListResponse.json`
- `GetPersonFilteredResponse.json` → `Responses/PersonFilteredResponse.json`
- `Responses/InvalidResponseType.txt` → `Responses/InvalidResponseType.txt`

### POST/Create Tests (7 tests → PersonCreateTests.cs)
1. ✅ `Should_Be_Able_To_Post_A_Person_Object()` - POST with object
2. ✅ `Should_Be_Able_To_Post_A_Person_Parameterized()` - POST with parameters
3. ✅ `Should_Be_Able_To_Post_A_Person_Parameterized_With_Absolute_Embedded_Filepath()` - POST with absolute path
4. ⚠️ `Should_Be_Able_To_Post_A_Person_By_Json()` - POST as error (Ignored)
5. ✅ `Should_Be_Able_To_Post_A_Person_By_Json_1()` - POST with embedded file
6. ✅ `Should_Be_Able_To_Post_A_Person_By_Json_With_Expected_Status_Code()` - POST with status code
7. ⚠️ `Should_Be_Able_Return_Validation_Infos_Of_Invalid_Payload()` - POST validation (Ignored)

**JSON Files:**
- `NewPerson.json` → `Requests/CreatePerson.json` + `Responses/CreatePerson.json`
- `NewPersonParameter.json` → `Requests/CreatePersonParameterized.json` + `Responses/CreatePersonParameterized.json`
- `AnyFolder/P/NewPersonParameter.json` → `Requests/CreatePersonParameterized.json`
- `AnyFolder/R/NewPersonParameter.json` → `Responses/CreatePersonParameterized.json`
- `Requests/SonGoku.json` → `Requests/CreatePersonSonGoku.json`
- `Responses/SonGoku.json` → `Responses/CreatePersonSonGoku.json`

**Naming Convention:** Request and Response files have **identical names** - only the folder differs (Requests/ vs Responses/). This makes it immediately obvious which response belongs to which request.

### PUT/PATCH Tests (6 tests → PersonUpdateTests.cs)
1. ✅ `Should_Be_Able_To_Put_A_Person_By_Json()` - PATCH test
2. ✅ `Should_Be_Able_To_Put_A_Person_By_Json_With_Expected_Status_Code()` - PATCH with status
3. ✅ `Should_Be_Able_To_Put_A_Patch_By_Json()` - PUT test
4. ✅ `Should_Be_Able_To_Put_A_Patch_By_Json_With_Expected_Status_Code()` - PUT with status
5. ✅ `Should_Be_Able_To_Put_A_Patch_By_Json_1()` - PUT with inline JSON
6. ✅ `Should_Be_Able_To_Put_A_Person_By_Json_1()` - PATCH with embedded file

**JSON Files:**
- `SonGoku.json` → `Requests/UpdatePersonPut.json`
- `SonGokuNewResponse.json` → `Responses/UpdatePersonPut.json`
- `Persons.Requests.SonGoku.json` → `Requests/UpdatePersonPatch.json`
- `Persons.Responses.SonGoku.json` → `Responses/UpdatePersonPatch.json`

**Naming Convention:** PUT and PATCH operations get distinct suffixes but still follow the same-name rule.

### DELETE Tests
None currently exist - we can add showcase tests later.

---

## Implementation Steps

### Phase 1: Create New Structure (Non-breaking)
1. Create new folder structure (`V1/Get/`, `V1/Create/`, `V1/Update/`)
2. Create `Requests/` and `Responses/` subfolders
3. Keep old structure intact during migration

### Phase 2: Migrate GET Tests
1. Create `Api/Persons/V1/Get/PersonGetTests.cs`
2. Copy GET tests from `PersonEndpointsTests.cs`
3. Add `[TestCategory("Native")]` to all native tests
4. Add Fluent API variations:
   - `[TestCategory("Fluent")]` + `[TestCategory("Fluent.Neutral")]`
   - `[TestCategory("Fluent")]` + `[TestCategory("Fluent.EndpointStyle")]`
5. Move JSON files to `Get/Responses/`
6. Update embedded resource paths

### Phase 3: Migrate POST Tests
1. Create `Api/Persons/V1/Create/PersonCreateTests.cs`
2. Copy POST tests from `PersonEndpointsTests.cs`
3. Add test categories
4. Add Fluent API variations
5. Move JSON files to `Create/Requests/` and `Create/Responses/`

### Phase 4: Migrate PUT/PATCH Tests
1. Create `Api/Persons/V1/Update/PersonUpdateTests.cs`
2. Copy PUT/PATCH tests from `PersonEndpointsTests.cs`
3. Add test categories
4. Add Fluent API variations
5. Move JSON files to `Update/Requests/` and `Update/Responses/`

### Phase 5: Shared Helpers
1. Create `Api/Persons/V1/_Shared/TestHelpers.cs`
2. Move `FilterFunc()` and `DifferenceFunc()` to shared class
3. Make them static and reusable

### Phase 6: Cleanup
1. Run all tests to verify migration
2. Delete old `PersonEndpointsTests.cs`
3. Delete old JSON files from root `Api/Persons/` folder
4. Update `.csproj` to remove old embedded resources

### Phase 7: Repeat for Controllers
Apply the same pattern to `Controllers.Test/Api/Persons/PersonController.cs`

---

## Test Method Naming Convention

### Native Tests
```csharp
[TestMethod]
[TestCategory("Native")]
public Task Should_Get_All_Persons()
```

### Fluent Neutral Tests
```csharp
[TestMethod]
[TestCategory("Fluent")]
[TestCategory("Fluent.Neutral")]
public Task Fluent_Should_Get_All_Persons()
```

### Fluent Endpoint Style Tests
```csharp
[TestMethod]
[TestCategory("Fluent")]
[TestCategory("Fluent.EndpointStyle")]
public Task FluentEndpoint_Should_Get_All_Persons()
```

---

## Example: PersonGetTests.cs Structure

```csharp
namespace MinimalApi.Test.Api.Persons.V1.Get
{
    using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
    using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;

    [TestClass]
    public class PersonGetTests : ApiTestBase
    {
        // ============================================================
        // NATIVE API TESTS
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        public Task Should_Get_All_Persons()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>(
                "api/v1/persons",
                "Responses/GetAllPersons.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        public Task Should_Get_Person_By_Query()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>(
                "api/v1/persons?name=Son",
                "Responses/GetPersonByQuery.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        public Task Should_Get_Persons_With_Filtering()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>(
                "api/v1/persons",
                "Responses/PersonListResponse.json",
                TestHelpers.OrderByIdFilter);
        }

        // ============================================================
        // FLUENT API TESTS - NEUTRAL STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        public Task Fluent_Should_Get_All_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                .WithResponse<IEnumerable<Person>>("Responses/GetAllPersons.json")
                .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        public Task Fluent_Should_Create_Person()
        {
            // Notice: Same filename in both Requests/ and Responses/!
            return Client.AssertPost("api/v1/persons")
                .WithBody("Requests/CreatePerson.json")
                .WithResponse<Person>("Responses/CreatePerson.json")
                .IgnoreProperty<Person>(p => p.Id)
                .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        public Task Fluent_Should_Get_Persons_With_Filtering()
        {
            return Client.AssertGet("api/v1/persons")
                .WithResponse<IEnumerable<Person>>("Responses/PersonListResponse.json")
                .FilterResponse(TestHelpers.OrderByIdFilter)
                .IgnoreProperty<Person>(p => p.Id)
                .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - ENDPOINT STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        public Task FluentEndpoint_Should_Get_All_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                .Produces<IEnumerable<Person>>("Responses/GetAllPersons.json")
                .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        public Task FluentEndpoint_Should_Create_Person()
        {
            // Notice: Same filename! Crystal clear which response belongs to which request
            return Client.AssertPost("api/v1/persons")
                .Accepts("Requests/CreatePerson.json")
                .Produces<Person>("Responses/CreatePerson.json")
                .IgnoreProperty<Person>(p => p.Id)
                .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        public Task FluentEndpoint_Should_Get_Persons_With_Filtering()
        {
            return Client.AssertGet("api/v1/persons")
                .Produces<IEnumerable<Person>>("Responses/PersonListResponse.json")
                .FilterResponse(TestHelpers.OrderByIdFilter)
                .IgnoreProperty<Person>(p => p.Id)
                .ExpectSuccess();
        }
    }
}
```

---

## Benefits of New Structure

1. ✅ **Clear Organization**: Each HTTP operation in own folder
2. ✅ **Side-by-side Comparison**: All 3 test styles in same file
3. ✅ **Shared Resources**: One set of JSON files for all styles
4. ✅ **Easy to Find**: `V1/Create/` → obviously creation tests
5. ✅ **Scalable**: Easy to add V2, V3 in future
6. ✅ **CI Flexible**: Can run only Native, only Fluent, or all
7. ✅ **Discoverable**: New developers immediately understand structure

---

## CI Test Execution Strategy

```bash
# Default: Run all tests
dotnet test

# Run only Native tests (existing API)
dotnet test --filter TestCategory=Native

# Run only Fluent tests
dotnet test --filter TestCategory=Fluent

# Run specific Fluent style
dotnet test --filter "TestCategory=Fluent&TestCategory=Fluent.Neutral"
dotnet test --filter "TestCategory=Fluent&TestCategory=Fluent.EndpointStyle"

# Run tests for specific operation
dotnet test --filter "FullyQualifiedName~.Get."
dotnet test --filter "FullyQualifiedName~.Create."
```

---

## Rollback Plan

If migration causes issues:
1. Old test files are not deleted until Phase 6
2. Can revert folder creation (no code changes yet)
3. Can run old tests in parallel during migration
4. Only delete old files after 100% test pass rate

---

## Timeline Estimate

- Phase 1 (Structure): 15 minutes
- Phase 2 (GET): 30 minutes  
- Phase 3 (POST): 30 minutes
- Phase 4 (PUT/PATCH): 30 minutes
- Phase 5 (Helpers): 15 minutes
- Phase 6 (Cleanup): 15 minutes
- Phase 7 (Controllers): 60 minutes

**Total: ~3 hours**

---

## Next Steps

1. Review and approve this plan
2. Start with Phase 1 (non-breaking structure creation)
3. Migrate GET tests first (smallest, easiest)
4. Validate, then continue with POST, PUT/PATCH
5. Repeat for Controllers.Test
