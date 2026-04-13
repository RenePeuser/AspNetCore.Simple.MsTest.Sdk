# Request Validation

## Objective

Every web API request object should have a minimal baseline of attribute-driven validation.

Use request validators on top only when business, cross-property, historical, or IO-based checks are needed.

## Core Rule

Start with validation attributes on the request DTO.

Then add one of the request validator base classes only for rules that cannot be expressed clearly by attributes alone.

## Request DTO Placement

- Request DTOs should live in a separate contracts project (e.g., `YourApi.Contracts`)
- Do not define request DTOs in the main API project
- Keep request validation metadata close to the request properties themselves

## Attribute-First Rule

Prefer attributes for property-level validation such as:
- required or nullability checks
- empty or whitespace checks
- leading or trailing whitespace checks
- enum validity
- GUID emptiness
- length or max length

Examples used in this repository include:
- `[IsNotNull]`
- `[IsNotEmpty]`
- `[IsNotWhitespace]`
- `[NoLeadingWhitespaces]`
- `[NoTrailingWhitespaces]`
- `[EnumIsDefined]`
- `[GuidIsNotEmpty]`
- `[MaxLength(...)]`
- `[Length(min, max)]`

## When to Use a Request Validator

Use a request validator when validation needs more than static property annotations.

Typical cases:
- cross-property rules
- request-shape rules that depend on multiple values together
- validation against existing data
- validation that requires repository or API access
- special PATCH semantics
- rules that need richer error construction than a simple attribute provides

## Custom Request Validator Rule

Custom request validators are a first-class pattern in this repository.

That means:
- start with request DTO attributes for the baseline rules
- then add a custom validator class for richer logic
- keep the `IAttributeValidator` in the validator constructor or base-class pipeline so attribute validation still runs

This is the preferred layering when a request needs both declarative property validation and imperative domain-aware validation.

## Choose the Right Base Type

### `RequestValidator<TRequest>`
Use for synchronous request validation.

### `AsyncRequestValidator<TRequest>`
Use when validation needs async work such as database lookups or service calls.

### `PatchRequestValidator<TRequest>`
Use for PATCH requests with patch-specific validation behavior.

### `AsyncPatchRequestValidator<TRequest>`
Use for PATCH requests that also require async validation.

## Validator Construction Rule

- Put the validator in the API layer near the operation, usually under `Validations/` or `Validators/`
- Keep the DI extension in the same file as the validator
- Inject `IAttributeValidator` when attribute-based validation should be included
- Ensure `IAttributeValidator` is registered in the DI graph for that area when needed
- If a custom validator adds lookup or graph consistency logic, keep that logic in the validator instead of pushing it down into the command

## Validation Layering

Recommended order:

1. Request DTO attributes for baseline property validation
2. Custom request validator for cross-property, lookup-based, or graph-aware rules
3. Command execution only after validation succeeds

Typical command flow:

1. Validate request
2. Map request
3. Execute main business or persistence logic

## Async Validation Rule

If validation needs to load data, use `AsyncRequestValidator<TRequest>` or `AsyncPatchRequestValidator<TRequest>` instead of blocking.

Do not turn IO-bound validation into synchronous code.

This includes validators that compose attribute validation with additional repository, query, or relation checks.

## Error Construction Rule

Return `PropertyValidationResult` values with clear `ValidationErrorDetails` or equivalent details.

Keep error messages specific and tied to the failing property or rule.

## Custom Attribute Rule

Before adding imperative validator logic, check whether an existing validation attribute or custom validation attribute already fits.

If a new reusable validation rule is needed:
- prefer a custom validation attribute plus validator pair
- keep it reusable across request DTOs
- keep its registration explicit

## Sample Value Guidance

Request validation and sample values should remain aligned.

If a request uses validation attributes and OpenAPI sample generation in the same area:
- keep request sample values valid by default
- place sample value providers in an appropriate location within your API structure
- keep the provider and its DI extension in the same file

## Good Signs

- The request DTO already communicates the basic rules through attributes
- The custom validator contains only the logic that could not live in attributes
- Attribute validation is still executed through `IAttributeValidator`
- Async validators are used for async checks
- Commands validate first and process second
- The validation flow is explicit and easy to trace

## Review Checklist

- Does the request DTO have baseline validation attributes?
- Is the chosen validator base type correct?
- Is async validation used when external data is involved?
- Is validator registration explicit?
- Is validation executed before mapping and main processing?
- Are request DTOs located in the contracts project?

## Notes

This pattern is not about pushing every rule into attributes.

The goal is:
- baseline rules in attributes
- richer rules in request validators
- explicit validation flow in the API layer
