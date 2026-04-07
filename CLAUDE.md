# Claude Code Guide

## About This Project

AspNetCore.Simple.MsTest.Sdk - A simple SDK to write easy and fast tests for your Web APIs.

## Skills

This project has comprehensive coding patterns available as executable skills.

**See the full catalog**: [Skills Index](.claude/skills/INDEX.md)

### Invoke Skills

Use the command format: `/category/skill-name`

Examples:
- `/architecture/service-registration` - DI registration patterns
- `/architecture/pipeline-pattern` - Sequential processing without CanHandle
- `/architecture/strategy-pattern` - Extensible branching logic
- `/csharp/constructor-overloads` - Constructor chaining patterns
- `/csharp/http-client-usage` - Safe outbound HTTP patterns with `IHttpClientFactory`
- `/api/minimal-api-structure` - Minimal API organization

### Categories

- **API Patterns** - Minimal API structure, namespace conventions
- **C# Language** - Constructors, methods, equality, collections, validation
- **Architecture** - Pipeline Pattern, Strategy Pattern, DI registration, data modeling
- **Async** - Async/await patterns, avoiding blocking operations

### Quick Reference

**Key Patterns:**
- Service Registration: Feature-based DI with `AddXxx()` extensions per class
- Pipeline Pattern: Sequential processing, no CanHandle needed, simpler extension
- Strategy Pattern: Strict resolution (exactly one match), explicit failures
- Constructor Overloads: One maximum constructor with clear delegation chain
- HttpClient Usage: Avoid per-call `new HttpClient()` and prefer `IHttpClientFactory`
- Immutable Data: Records for data, classes for services

**See** [Skills Index](.claude/skills/INDEX.md) for complete list and documentation links.

---

**Note:** Each skill has detailed documentation (PATTERN.md), code examples (EXAMPLES.md), and review checklists (CHECKLIST.md).
