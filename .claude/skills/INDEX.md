# Skills Index

This index lists all coding patterns and skills available in this repository.

Each skill can be invoked using the `/category/skill-name` command format.

## How to Use Skills

Skills help you apply repository patterns consistently. Each skill:
- Has a `SKILL.md` with the skill prompt and approach
- Has a `PATTERN.md` with detailed rules and guidance
- May have `EXAMPLES.md` with code examples for larger or more implementation-heavy skills
- May have `CHECKLIST.md` with review criteria for review-heavy skills

### Invocation Format

```
/api/minimal-api-structure
/csharp/constructor-overloads
/architecture/strategy-pattern
/async/avoid-sync-over-async
```

---

## API Patterns

### `/api/minimal-api-structure`
**Create or review Minimal API endpoint structure and organization**

Apply service-only registration, self-mapping endpoints, and horizontal action slicing patterns.

[Documentation](api/minimal-api-structure/PATTERN.md) | [Examples](api/minimal-api-structure/EXAMPLES.md) | [Checklist](api/minimal-api-structure/CHECKLIST.md)

### `/api/namespace-provider`
**Configure folder structure and namespace conventions**

Set up ReSharper/Rider namespace provider settings to keep namespaces domain-focused.

[Documentation](api/namespace-provider/PATTERN.md) | [Examples](api/namespace-provider/EXAMPLES.md) | [Checklist](api/namespace-provider/CHECKLIST.md)

### `/api/create-endpoint`
**Create architecture-compliant endpoint or endpoints**

Generate endpoint scaffolding with contracts, commands or queries, endpoints, documentation files, and explicit startup registration.

[Documentation](api/create-endpoint/PATTERN.md) | [Examples](api/create-endpoint/EXAMPLES.md) | [Checklist](api/create-endpoint/CHECKLIST.md)

### `/api/update-openapi-spec`
**Update OpenAPI specification files when API schema changes**

Refresh OpenAPI response fixtures through the targeted test flow and summarize the resulting schema changes.

[Documentation](api/update-openapi-spec/PATTERN.md) | [Examples](api/update-openapi-spec/EXAMPLES.md) | [Checklist](api/update-openapi-spec/CHECKLIST.md)

### `/api/throwing-exceptions`
**Use ProblemDetailsException for proper HTTP error handling (RFC 7807)**

Throw HTTP-specific exceptions with constant titles, detailed messages, and structured extensions for dashboards and log analytics.

[Documentation](api/throwing-exceptions/PATTERN.md) | [Examples](api/throwing-exceptions/EXAMPLES.md) | [Checklist](api/throwing-exceptions/CHECKLIST.md)

### `/api/request-validation`
**Design request DTO validation with attributes and request validators**

Use validation attributes for baseline request checks and add sync, async, or patch request validators only for richer rules that require cross-property or async validation.

[Documentation](api/request-validation/PATTERN.md) | [Examples](api/request-validation/EXAMPLES.md) | [Checklist](api/request-validation/CHECKLIST.md)

---

## API Testing Patterns

### `/api-testing/test-endpoint`
**Generate unit tests for Minimal API endpoints**

Create endpoint tests that mirror the API structure, use repository assertion helpers, and generate response fixtures through the supported test flow.

[Documentation](api-testing/test-endpoint/PATTERN.md) | [Examples](api-testing/test-endpoint/EXAMPLES.md) | [Checklist](api-testing/test-endpoint/CHECKLIST.md)

---

## C# Language Patterns

### `/csharp/constructor-overloads`
**Design or review overloaded constructors with proper chaining**

Follow the "one maximum constructor" pattern with clear delegation chains.

[Documentation](csharp/constructor-overloads/PATTERN.md) | [Examples](csharp/constructor-overloads/EXAMPLES.md) | [Checklist](csharp/constructor-overloads/CHECKLIST.md)

### `/csharp/method-overloads`
**Design or review overloaded methods with proper delegation**

Follow the "one maximum overload" pattern with clear forwarding chains.

[Documentation](csharp/method-overloads/PATTERN.md) | [Examples](csharp/method-overloads/EXAMPLES.md) | [Checklist](csharp/method-overloads/CHECKLIST.md)

### `/csharp/argument-check`
**Add or review guard clauses with Argument.Check package**

Use `Throw.*` helpers for clear, consistent validation at boundaries.

[Documentation](csharp/argument-check/PATTERN.md) | [Examples](csharp/argument-check/EXAMPLES.md) | [Checklist](csharp/argument-check/CHECKLIST.md)

### `/csharp/equality-comparison`
**Add or review equality checks using Extensions.Pack helpers**

Use `EqualsTo` and `NotEqualsTo` for consistent value equality.

[Documentation](csharp/equality-comparison/PATTERN.md) | [Examples](csharp/equality-comparison/EXAMPLES.md) | [Checklist](csharp/equality-comparison/CHECKLIST.md)

### `/csharp/non-null-collections`
**Design and review collection properties that never return null**

Collections must always be non-null with empty defaults.

[Documentation](csharp/non-null-collections/PATTERN.md) | [Examples](csharp/non-null-collections/EXAMPLES.md) | [Checklist](csharp/non-null-collections/CHECKLIST.md)

### `/csharp/object-initializer-order`
**Design or review object initializers with consistent property ordering**

Order properties alphabetically for predictability, better diffs, and merge safety.

[Documentation](csharp/object-initializer-order/PATTERN.md) | [Examples](csharp/object-initializer-order/EXAMPLES.md) | [Checklist](csharp/object-initializer-order/CHECKLIST.md)

### `/csharp/override-virtual-methods`
**Add or review overrides of virtual or abstract members**

Call `base` by default unless intentionally replacing behavior.

[Documentation](csharp/override-virtual-methods/PATTERN.md) | [Examples](csharp/override-virtual-methods/EXAMPLES.md) | [Checklist](csharp/override-virtual-methods/CHECKLIST.md)

### `/csharp/global-regex`
**Create or review regular expressions with GeneratedRegex**

Centralize all regex patterns in one `GlobalRegex` class per project, avoiding scattered partial classes.

[Documentation](csharp/global-regex/PATTERN.md) | [Examples](csharp/global-regex/EXAMPLES.md) | [Checklist](csharp/global-regex/CHECKLIST.md)

### `/csharp/http-client-usage`
**Create or review HttpClient usage to avoid socket exhaustion**

Use `IHttpClientFactory` for HTTP integrations instead of manual `HttpClient` instantiation.

[Documentation](csharp/http-client-usage/PATTERN.md) | [Examples](csharp/http-client-usage/EXAMPLES.md) | [Checklist](csharp/http-client-usage/CHECKLIST.md)

### `/csharp/if-else-early-exit`
**Refactor nested branching into flat early-exit control flow**

Use early `return`, `throw`, `continue`, and `break` to avoid deep `if/else` cascades and keep the happy path easy to read.

[Documentation](csharp/if-else-early-exit/PATTERN.md) | [Examples](csharp/if-else-early-exit/EXAMPLES.md) | [Checklist](csharp/if-else-early-exit/CHECKLIST.md)

### `/csharp/loops-use-continue`
**Refactor nested loop logic into flat continue-first control flow**

Use early `continue` in `foreach` and `for` loops to keep skip logic at the top and the actual work easy to read.

[Documentation](csharp/loops-use-continue/PATTERN.md) | [Examples](csharp/loops-use-continue/EXAMPLES.md) | [Checklist](csharp/loops-use-continue/CHECKLIST.md)

---

## Architecture & Design Patterns

### `/architecture/pipeline-pattern`
**Apply or review the Pipeline Pattern**

Implement sequential processing with multiple steps. Simpler than Strategy Pattern - no `CanHandle` logic needed.

[Documentation](architecture/pipeline-pattern/PATTERN.md) | [Examples](architecture/pipeline-pattern/EXAMPLES.md) | [Checklist](architecture/pipeline-pattern/CHECKLIST.md)

### `/architecture/strategy-pattern`
**Apply or review the Strategy Pattern**

Implement extensible branching logic with strict resolution where exactly one strategy must match.

[Documentation](architecture/strategy-pattern/PATTERN.md) | [Examples](architecture/strategy-pattern/EXAMPLES.md) | [Checklist](architecture/strategy-pattern/CHECKLIST.md)

### `/architecture/service-registration`
**Create or review dependency injection registration patterns**

Use feature-based hierarchical registration with `AddXxx()` extensions.

[Documentation](architecture/service-registration/PATTERN.md) | [Examples](architecture/service-registration/EXAMPLES.md) | [Checklist](architecture/service-registration/CHECKLIST.md)

### `/architecture/immutable-data-and-services`
**Design data models and services with proper separation**

Use immutable records for data, classes for services, and immutable collections for stability.

[Documentation](architecture/immutable-data-and-services/PATTERN.md) | [Examples](architecture/immutable-data-and-services/EXAMPLES.md) | [Checklist](architecture/immutable-data-and-services/CHECKLIST.md)

---

## Async Patterns

### `/async/avoid-sync-over-async`
**Review and fix sync-over-async anti-patterns**

Prefer `await` over `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.

[Documentation](async/avoid-sync-over-async/PATTERN.md) | [Examples](async/avoid-sync-over-async/EXAMPLES.md) | [Checklist](async/avoid-sync-over-async/CHECKLIST.md)

### `/async/avoid-thread-sleep`
**Review and fix Thread.Sleep usage**

Prefer `await Task.Delay()` over blocking `Thread.Sleep()`.

[Documentation](async/avoid-thread-sleep/PATTERN.md) | [Examples](async/avoid-thread-sleep/EXAMPLES.md) | [Checklist](async/avoid-thread-sleep/CHECKLIST.md)

---

## Quick Reference by Use Case

**Creating new code:**
- Creating endpoints → `/api/create-endpoint`
- Building APIs → `/api/minimal-api-structure`
- Testing endpoints → `/api-testing/test-endpoint`
- Throwing exceptions → `/api/throwing-exceptions`
- Request validation → `/api/request-validation`
- HTTP client integrations → `/csharp/http-client-usage`
- Flattening if/else cascades → `/csharp/if-else-early-exit`
- Flattening loop logic → `/csharp/loops-use-continue`
- Registering services → `/architecture/service-registration`
- Designing data models → `/architecture/immutable-data-and-services`
- Adding overloads → `/csharp/constructor-overloads`, `/csharp/method-overloads`
- Sequential processing → `/architecture/pipeline-pattern`
- Conditional branching → `/architecture/strategy-pattern`
- Object initializers → `/csharp/object-initializer-order`
- Regular expressions → `/csharp/global-regex`

**Reviewing existing code:**
- OpenAPI specs → `/api/update-openapi-spec`
- Exception handling → `/api/throwing-exceptions`
- Request validation → `/api/request-validation`
- HttpClient usage → `/csharp/http-client-usage`
- Pipeline implementations → `/architecture/pipeline-pattern`
- Strategy implementations → `/architecture/strategy-pattern`
- DI registration → `/architecture/service-registration`
- Async patterns → `/async/avoid-sync-over-async`
- Guard clauses → `/csharp/argument-check`
- Branch flattening → `/csharp/if-else-early-exit`
- Loop branch flattening → `/csharp/loops-use-continue`
- Object initializers → `/csharp/object-initializer-order`
- Regular expressions → `/csharp/global-regex`

**Refactoring:**
- Sequential processing → `/architecture/pipeline-pattern`
- Branching logic → `/architecture/strategy-pattern`
- Constructor duplication → `/csharp/constructor-overloads`
- Blocking delays → `/async/avoid-thread-sleep`
- HttpClient instantiation → `/csharp/http-client-usage`
- Nested branching → `/csharp/if-else-early-exit`
- Nested loop branching → `/csharp/loops-use-continue`
- Namespace clutter → `/api/namespace-provider`
- Scattered regex → `/csharp/global-regex`
