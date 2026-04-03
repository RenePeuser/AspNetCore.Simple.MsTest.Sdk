# Claude Code Guide

## About This Project

AspNetCore.Simple.MsTest.Sdk - A simple SDK to write easy and fast tests for your Web APIs.

## Skills & Patterns

This project uses specific patterns and conventions. Each skill is documented in detail:

### Architecture & Design Patterns

- **[Service Registration Pattern](.claude/skills/service-registration-pattern.md)** - Feature-based DI registration with AddXXX extensions per class

## Quick Reference

### Service Registration
- ✅ One extension per class in the same file
- ✅ Feature-based tree structure (not flat)
- ✅ Dependencies via their AddXXX extensions
- ✅ Always use `AddSingletonIfNotExists`

See [Service Registration Pattern](.claude/skills/service-registration-pattern.md) for full details.

---

**Note:** This file serves as an index. Detailed documentation is in individual skill files under `.claude/skills/`.
