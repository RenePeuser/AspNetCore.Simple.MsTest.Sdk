# JSON File Naming Guide

## Naming Convention Rule

**Request and Response files MUST have identical names** - only the folder differs.

```
Operation/
├── Requests/
│   └── OperationName.json
└── Responses/
    └── OperationName.json    ← SAME NAME!
```

This makes it **crystal clear** which response belongs to which request.

---

## Complete File Mapping

### MinimalApi.Test

#### GET Operations (No Request Body)
| Old Location | New Location | Notes |
|-------------|--------------|-------|
| `GetPersonResponse.json` | `Get/Responses/GetAllPersons.json` | Renamed for clarity |
| `GetPersonFilteredResponse.json` | `Get/Responses/GetPersonByQuery.json` | Renamed for clarity |
| `Responses/InvalidResponseType.txt` | `Get/Responses/InvalidResponseType.txt` | Error case |

#### POST/Create Operations
| Old Location | New Request | New Response | Notes |
|-------------|-------------|--------------|-------|
| `NewPerson.json` (response) | - | `Create/Responses/CreatePerson.json` | Response only (object in code) |
| `NewPersonParameter.json` (both) | `Create/Requests/CreatePersonParameterized.json` | `Create/Responses/CreatePersonParameterized.json` | **Same name!** |
| `AnyFolder/P/NewPersonParameter.json` | `Create/Requests/CreatePersonParameterized.json` | - | Merged into one |
| `AnyFolder/R/NewPersonParameter.json` | - | `Create/Responses/CreatePersonParameterized.json` | Merged into one |
| `Requests/SonGoku.json` | `Create/Requests/CreatePersonSonGoku.json` | - | Prefixed with operation |
| `Responses/SonGoku.json` | - | `Create/Responses/CreatePersonSonGoku.json` | **Same name!** |

#### PUT/PATCH Operations
| Old Location | New Request | New Response | Notes |
|-------------|-------------|--------------|-------|
| `SonGoku.json` (request) | `Update/Requests/UpdatePersonPut.json` | - | Used for PUT |
| `SonGokuNewResponse.json` | - | `Update/Responses/UpdatePersonPut.json` | **Same name!** |
| `Persons.Requests.SonGoku.json` | `Update/Requests/UpdatePersonPatch.json` | - | Used for PATCH |
| `Persons.Responses.SonGoku.json` | - | `Update/Responses/UpdatePersonPatch.json` | **Same name!** |

---

### Controllers.Test

#### GET Operations
| Old Location | New Location | Notes |
|-------------|--------------|-------|
| `Responses/GetPersonResponse.json` | `Get/Responses/GetAllPersons.json` | Renamed for clarity |

#### POST/Create Operations
| Old Location | New Request | New Response | Notes |
|-------------|-------------|--------------|-------|
| `Requests/CreatePersonRequest.json` | `Create/Requests/CreatePerson.json` | `Create/Responses/CreatePerson.json` | **Same name!** |
| `Responses/CreatePersonResponse.json` | - | `Create/Responses/CreatePerson.json` | - |

#### PUT/PATCH Operations
| Old Location | New Request | New Response | Notes |
|-------------|-------------|--------------|-------|
| `Requests/UpdatePersonRequest.json` | `Update/Requests/UpdatePersonPut.json` | `Update/Responses/UpdatePersonPut.json` | **Same name!** |
| `Requests/PatchPersonRequest.json` | `Update/Requests/UpdatePersonPatch.json` | `Update/Responses/UpdatePersonPatch.json` | **Same name!** |

---

## Naming Pattern Examples

### Good ✅

```csharp
// GET - Response only (no request body)
"Responses/GetAllPersons.json"
"Responses/GetPersonByQuery.json"
"Responses/GetPersonById.json"

// POST - Same name in both folders
"Requests/CreatePerson.json"
"Responses/CreatePerson.json"

"Requests/CreatePersonWithEmails.json"
"Responses/CreatePersonWithEmails.json"

// PUT - Same name pattern
"Requests/UpdatePersonPut.json"
"Responses/UpdatePersonPut.json"

// PATCH - Same name pattern
"Requests/UpdatePersonPatch.json"
"Responses/UpdatePersonPatch.json"

// DELETE - Response only (usually 204 NoContent)
"Responses/DeletePersonSuccess.json"  // If there's a response body
```

### Bad ❌

```csharp
// Different names - confusing!
"Requests/PersonRequest.json"
"Responses/CreatedPerson.json"          // ❌ Which request does this belong to?

// Unclear operation
"Requests/Person.json"                  // ❌ Is this Create? Update?
"Responses/Person.json"

// Inconsistent naming
"Requests/create-person.json"           // ❌ Inconsistent casing
"Responses/CreatePerson.json"
```

---

## Template for New Operations

```
{Operation}/
├── Requests/
│   ├── {Operation}{Descriptor}.json
│   ├── {Operation}{Descriptor}Parameterized.json
│   └── {Operation}{Descriptor}Invalid.json
└── Responses/
    ├── {Operation}{Descriptor}.json              ← SAME NAME!
    ├── {Operation}{Descriptor}Parameterized.json ← SAME NAME!
    └── {Operation}{Descriptor}Invalid.json       ← SAME NAME!
```

### Examples:

#### Create Operation
```
Create/
├── Requests/
│   ├── CreatePerson.json
│   ├── CreatePersonWithEmails.json
│   └── CreatePersonInvalid.json
└── Responses/
    ├── CreatePerson.json
    ├── CreatePersonWithEmails.json
    └── CreatePersonInvalid.json
```

#### Get Operation (No Request Body)
```
Get/
├── Requests/
│   └── (empty - GET has no body)
└── Responses/
    ├── GetAllPersons.json
    ├── GetPersonById.json
    ├── GetPersonByQuery.json
    └── GetPersonNotFound.json
```

#### Update Operation (Both PUT and PATCH)
```
Update/
├── Requests/
│   ├── UpdatePersonPut.json
│   ├── UpdatePersonPatch.json
│   └── UpdatePersonInvalid.json
└── Responses/
    ├── UpdatePersonPut.json
    ├── UpdatePersonPatch.json
    └── UpdatePersonInvalid.json
```

---

## Test Code Pattern

### With Same-Name Files (Recommended ✅)

```csharp
[TestMethod]
public Task Should_Create_Person()
{
    // Same name makes it obvious!
    return Client.AssertPost("api/v1/persons")
        .WithBody("Requests/CreatePerson.json")
        .WithResponse<Person>("Responses/CreatePerson.json")
        .ExpectSuccess();
}

[TestMethod]
public Task Fluent_Should_Create_Person_With_Emails()
{
    // Same name, different operation variant
    return Client.AssertPost("api/v1/persons")
        .Accepts("Requests/CreatePersonWithEmails.json")
        .Produces<Person>("Responses/CreatePersonWithEmails.json")
        .ExpectSuccess();
}
```

### With Different Names (Confusing ❌)

```csharp
[TestMethod]
public Task Should_Create_Person()
{
    // Which response belongs to NewPersonRequest.json? 🤔
    return Client.AssertPost("api/v1/persons")
        .WithBody("Requests/NewPersonRequest.json")
        .WithResponse<Person>("Responses/CreatedPersonResult.json")  // ❌ Unclear!
        .ExpectSuccess();
}
```

---

## Benefits of Same-Name Convention

1. ✅ **Instant Recognition**: File name tells you exactly what it tests
2. ✅ **Easy Navigation**: Ctrl+P and type name, both files show up
3. ✅ **Maintenance**: Rename operation? Both files are obvious
4. ✅ **Code Reviews**: Reviewer immediately knows what's tested
5. ✅ **Refactoring**: Search/Replace works cleanly
6. ✅ **Documentation**: Self-documenting structure

---

## File Content Template

### Request File: `Create/Requests/CreatePerson.json`
```json
{
  "name": "Son",
  "firstName": "Goku",
  "age": 42,
  "emails": []
}
```

### Response File: `Create/Responses/CreatePerson.json`
```json
{
  "content": {
    "headers": [
      {
        "key": "Content-Type",
        "value": ["application/json; charset=utf-8"]
      }
    ],
    "value": {
      "id": 1,
      "name": "Son",
      "firstName": "Goku",
      "age": 42,
      "emails": []
    }
  },
  "statusCode": "Created",
  "headers": [],
  "trailingHeaders": [],
  "isSuccessStatusCode": true
}
```

---

## Migration Checklist

When migrating old files:

- [ ] Identify the HTTP operation (GET, POST, PUT, PATCH, DELETE)
- [ ] Choose a descriptive operation name (CreatePerson, UpdatePersonPut, etc.)
- [ ] Ensure request and response have **identical names**
- [ ] Place request in `{Operation}/Requests/`
- [ ] Place response in `{Operation}/Responses/`
- [ ] Update test code to reference new paths
- [ ] Update `.csproj` embedded resources
- [ ] Run tests to verify
- [ ] Delete old file

---

## Questions?

**Q: What if one request has multiple possible responses?**  
A: Add a suffix to distinguish them:
```
Requests/CreatePerson.json
Responses/CreatePerson.json         (success)
Responses/CreatePersonConflict.json (409 error)
Responses/CreatePersonInvalid.json  (400 error)
```

**Q: What if the same request is used in multiple tests?**  
A: That's fine! Multiple tests can reference the same file:
```csharp
// Test 1
.WithBody("Requests/CreatePerson.json")

// Test 2 - same request, different assertion
.WithBody("Requests/CreatePerson.json")
.FilterResponse(...)
```

**Q: Should I include the HTTP method in the filename?**  
A: No! The folder structure already indicates the operation:
- ✅ `Create/Requests/CreatePerson.json` (folder shows it's POST)
- ❌ `Create/Requests/PostCreatePerson.json` (redundant)

**Q: What about query parameters?**  
A: Include them in the description:
- ✅ `Get/Responses/GetPersonByQuery.json`
- ✅ `Get/Responses/GetPersonsByAge.json`
- ✅ `Get/Responses/GetPersonsPaginated.json`
