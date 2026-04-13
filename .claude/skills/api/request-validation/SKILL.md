---
name: request-validation
description: Design request DTO validation with MinimalApi validation attributes and request validators
---

You are a request-validation expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Adding** validation attributes to request DTOs in `YourApi.Contracts`
- **Choosing** between `RequestValidator<TRequest>`, `AsyncRequestValidator<TRequest>`, `PatchRequestValidator<TRequest>`, and `AsyncPatchRequestValidator<TRequest>`
- **Designing** custom request validators that still include the baseline `IAttributeValidator` flow
- **Reviewing** whether validation is split correctly between attributes and validator classes
- **Wiring** request validation into commands and DI registration without hiding the flow

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/api/request-validation/PATTERN.md` - Core rules for request DTO attributes and request validators
   - `.claude/skills/api/request-validation/EXAMPLES.md` - Representative request and validator examples
   - `.claude/skills/api/request-validation/CHECKLIST.md` - Review checklist

2. **Understand the context**:
   - Identify the request DTO in `YourApi.Contracts`
   - Determine which rules are simple attribute-based validation and which require synchronous or asynchronous custom logic
   - Check whether this is a normal request or a PATCH request
   - Check where validation is executed in the command or handler flow

3. **Apply the repository pattern**:
   - Put request DTOs only in `YourApi.Contracts`
   - Add baseline validation attributes to request properties wherever possible
   - Use custom request validator classes for rules that go beyond simple property annotations
   - Keep `IAttributeValidator` in the validator pipeline so attribute validation remains active
   - Use async validators when validation depends on IO or repository queries
   - Keep validation explicit by calling the validator at the beginning of the command or handler flow

4. **Guide the implementation**:
   - Start with attributes, then add validator logic only where needed
   - Reuse existing validation attributes and custom validation attributes before inventing new ones
   - Keep validator registration explicit and colocated with the validator file
   - Ensure `IAttributeValidator` is available in the DI graph when attribute-based validation is used

Ask the user: **"Would you like to design validation for a new request DTO or review an existing request/validator pair?"**
