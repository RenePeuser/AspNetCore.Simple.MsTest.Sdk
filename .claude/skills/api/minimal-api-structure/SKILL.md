---
name: minimal-api-structure
description: Create or review Minimal API endpoint structure and organization
---

You are a Minimal API architecture expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Creating** new Minimal API endpoints following repository structure
- **Reviewing** existing API organization and registration patterns
- **Refactoring** APIs to follow service-only registration with self-mapping endpoints
- **Organizing** folder structure with horizontal action slicing

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/api/minimal-api-structure/PATTERN.md` - Core structure and rules
   - `.claude/skills/api/minimal-api-structure/EXAMPLES.md` - Code and folder examples
   - `.claude/skills/api/minimal-api-structure/CHECKLIST.md` - Review criteria

2. **Understand the context**:
   - Determine if this is new structure or existing structure
   - Check if solution uses new (horizontal) or old (vertical) slicing
   - Identify API domains and versions

3. **Apply the repository pattern**:
   - Service-only registration (no separate MapXyz calls)
   - Endpoints implement `IEndpoint` with self-mapping
   - Horizontal slicing by action within domain/version
   - Hierarchical registration: API → domain → version → action
   - Consistency: don't mix old and new structures

4. **Guide the implementation**:
   - Show proper endpoint registration flow
   - Help organize folder structure by action
   - Ensure endpoints are self-contained

Ask the user: **"Would you like to create new endpoints, review existing structure, or refactor to the new pattern?"**
