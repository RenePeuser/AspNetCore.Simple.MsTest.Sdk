---
name: namespace-provider
description: Use when creating, reviewing, or restructuring folder and namespace conventions in this .NET codebase.
---

# Namespace Provider

Use this skill when working with folder structure, namespaces, and ReSharper or Rider namespace provider settings.

## Objective

Keep namespaces domain-focused and easy to read.

Use folder structure for physical organization, but do not let technical subfolders create noisy or overly detailed namespaces.

## Apply These Rules

- Keep namespaces focused on domain and version.
- Skip technical folders in namespace generation when they only describe implementation structure.
- Prefer namespaces such as `Pulse.FieldingTool.Api.FormsConfigurations.V1`.
- Do not expose technical folder names like `Requests`, `Responses`, `Commands`, `Queries`, `Endpoints`, `Mappers`, `Extensions`, or `Validations` in normal namespace design when namespace provider settings can hide them.
- Keep domain folders and version folders visible in namespaces.
- Treat physical folder structure and logical namespace design as separate concerns.
- Keep namespace conventions consistent across production and test code.
- Share namespace provider settings across the team so generated namespaces remain uniform.
- Store namespace provider configuration in the relevant `*.csproj.DotSettings` file.
- Do not place these namespace provider settings into `*.sln.DotSettings`.

## Settings File Convention

Store namespace provider settings in the corresponding project settings file:

- `[ProjectName].csproj.DotSettings`

Do not place these namespace provider settings into:

- `[SolutionName].sln.DotSettings`

Reason:

These namespace provider rules are project-specific and must stay close to the project they belong to.

## Prefer

```csharp
using Pulse.FieldingTool.Api.FormsConfigurations.V1;
```

## Avoid

```csharp
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Commands;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Extensions;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Validations;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Endpoints;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Mappers;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Requests;
using Pulse.FieldingTool.Api.FormsConfigurations.V1.Responses;
```

## ReSharper / Rider Setting Pattern

Use namespace provider settings to skip technical folders from namespace generation.

Example:

```xml
<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Crequests/@EntryIndexedValue">True</s:Boolean>
<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cvalidations/@EntryIndexedValue">True</s:Boolean>
<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Ccommands/@EntryIndexedValue">True</s:Boolean>
```

## Review Checklist

- Is the namespace domain-focused?
- Are domain and version still visible?
- Are technical folders hidden from the namespace where appropriate?
- Are production and test code using the same namespace convention?
- Are ReSharper or Rider settings aligned with the intended structure?
- Is the namespace provider configuration stored in the correct `*.csproj.DotSettings` file?
- Would renaming a technical folder avoid unnecessary namespace churn?

## Notes

The folder structure may still contain technical subfolders for organization.
That is acceptable and expected.

The namespace should primarily communicate domain and API version, not internal implementation details.
