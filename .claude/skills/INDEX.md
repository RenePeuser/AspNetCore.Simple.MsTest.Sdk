# Skills Index

This index lists all coding patterns and skills available in this repository.

Each skill can be invoked using the `/category/skill-name` command format.

## How to Use Skills

Skills help you apply repository patterns consistently. Each skill:
- Has a `SKILL.md` with the skill prompt and approach
- Has a `PATTERN.md` with detailed rules and guidance
- May have `EXAMPLES.md` with code examples
- May have `CHECKLIST.md` with review criteria

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

[Documentation](api/namespace-provider/PATTERN.md) | [Examples](api/namespace-provider/EXAMPLES.md)

---

## C# Language Patterns

### `/csharp/constructor-overloads`
**Design or review overloaded constructors with proper chaining**

Follow the "one maximum constructor" pattern with clear delegation chains.

[Documentation](csharp/constructor-overloads/PATTERN.md) | [Examples](csharp/constructor-overloads/EXAMPLES.md)

### `/csharp/method-overloads`
**Design or review overloaded methods with proper delegation**

Follow the "one maximum overload" pattern with clear forwarding chains.

[Documentation](csharp/method-overloads/PATTERN.md) | [Examples](csharp/method-overloads/EXAMPLES.md)

### `/csharp/argument-check`
**Add or review guard clauses with Argument.Check package**

Use `Throw.*` helpers for clear, consistent validation at boundaries.

[Documentation](csharp/argument-check/PATTERN.md)

### `/csharp/equality-comparison`
**Add or review equality checks using Extensions.Pack helpers**

Use `EqualsTo` and `NotEqualsTo` for consistent value equality.

[Documentation](csharp/equality-comparison/PATTERN.md)

### `/csharp/non-null-collections`
**Design and review collection properties that never return null**

Collections must always be non-null with empty defaults.

[Documentation](csharp/non-null-collections/PATTERN.md)

### `/csharp/object-initializer-order`
**Design or review object initializers with consistent property ordering**

Order properties alphabetically for predictability, better diffs, and merge safety.

[Documentation](csharp/object-initializer-order/PATTERN.md) | [Examples](csharp/object-initializer-order/EXAMPLES.md)

### `/csharp/override-virtual-methods`
**Add or review overrides of virtual/abstract members**

Call `base` by default unless intentionally replacing behavior.

[Documentation](csharp/override-virtual-methods/PATTERN.md)

### `/csharp/global-regex`
**Create or review regular expressions with GeneratedRegex**

Centralize all regex patterns in one `GlobalRegex` class per project, avoiding scattered partial classes.

[Documentation](csharp/global-regex/PATTERN.md) | [Examples](csharp/global-regex/EXAMPLES.md) | [Checklist](csharp/global-regex/CHECKLIST.md)

---

## Architecture & Design Patterns

### `/architecture/pipeline-pattern`
**Apply or review the Pipeline Pattern**

Implement sequential processing with multiple steps. Simpler than Strategy Pattern - no `CanHandle` logic needed.

[Documentation](architecture/pipeline-pattern/PATTERN.md) | [Examples](architecture/pipeline-pattern/EXAMPLES.md) | [Checklist](architecture/pipeline-pattern/CHECKLIST.md)

### `/architecture/strategy-pattern`
**Apply or review the Strategy Pattern**

Implement extensible branching logic with strict resolution (exactly one strategy must match).

[Documentation](architecture/strategy-pattern/PATTERN.md) | [Examples](architecture/strategy-pattern/EXAMPLES.md) | [Checklist](architecture/strategy-pattern/CHECKLIST.md)

### `/architecture/service-registration`
**Create or review dependency injection registration patterns**

Use feature-based hierarchical registration with `AddXxx()` extensions.

[Documentation](architecture/service-registration/PATTERN.md) | [Examples](architecture/service-registration/EXAMPLES.md) | [Checklist](architecture/service-registration/CHECKLIST.md)

### `/architecture/immutable-data-and-services`
**Design data models and services with proper separation**

Use immutable records for data, classes for services, immutable collections for stability.

[Documentation](architecture/immutable-data-and-services/PATTERN.md) | [Examples](architecture/immutable-data-and-services/EXAMPLES.md)

---

## Async Patterns

### `/async/avoid-sync-over-async`
**Review and fix sync-over-async anti-patterns**

Prefer `await` over `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.

[Documentation](async/avoid-sync-over-async/PATTERN.md)

### `/async/avoid-thread-sleep`
**Review and fix Thread.Sleep usage**

Prefer `await Task.Delay()` over blocking `Thread.Sleep()`.

[Documentation](async/avoid-thread-sleep/PATTERN.md)

---

## Quick Reference by Use Case

**Creating new code:**
- Building APIs → `/api/minimal-api-structure`
- Registering services → `/architecture/service-registration`
- Designing data models → `/architecture/immutable-data-and-services`
- Adding overloads → `/csharp/constructor-overloads`, `/csharp/method-overloads`
- Sequential processing → `/architecture/pipeline-pattern`
- Conditional branching → `/architecture/strategy-pattern`
- Object initializers → `/csharp/object-initializer-order`
- Regular expressions → `/csharp/global-regex`

**Reviewing existing code:**
- Pipeline implementations → `/architecture/pipeline-pattern`
- Strategy implementations → `/architecture/strategy-pattern`
- DI registration → `/architecture/service-registration`
- Async patterns → `/async/avoid-sync-over-async`
- Guard clauses → `/csharp/argument-check`
- Object initializers → `/csharp/object-initializer-order`
- Regular expressions → `/csharp/global-regex`

**Refactoring:**
- Sequential processing → `/architecture/pipeline-pattern`
- Branching logic → `/architecture/strategy-pattern`
- Constructor duplication → `/csharp/constructor-overloads`
- Blocking delays → `/async/avoid-thread-sleep`
- Namespace clutter → `/api/namespace-provider`
- Scattered regex → `/csharp/global-regex`
