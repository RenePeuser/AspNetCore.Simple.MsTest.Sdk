# Namespace Provider Examples

## ReSharper / Rider Setting Pattern

Use namespace provider settings to skip technical folders from namespace generation.

Example configuration in `[ProjectName].csproj.DotSettings`:

```xml
<wpf:ResourceDictionary xml:space="preserve" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:s="clr-namespace:System;assembly=mscorlib" xmlns:ss="urn:shemas-jetbrains-com:settings-storage-xaml" xmlns:wpf="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Crequests/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cresponses/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cvalidations/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Ccommands/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cqueries/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cendpoints/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cmappers/@EntryIndexedValue">True</s:Boolean>
	<s:Boolean x:Key="/Default/CodeInspection/NamespaceProvider/NamespaceFoldersToSkip/=api_005Cformsconfigurations_005Cv1_005Cextensions/@EntryIndexedValue">True</s:Boolean>
</wpf:ResourceDictionary>
```

## Common Technical Folders to Skip

Typical folders that should be skipped from namespace generation:

- `Requests`
- `Responses`
- `Commands`
- `Queries`
- `Endpoints`
- `Mappers`
- `Extensions`
- `Validations`
- `Handlers`
- `Models` (sometimes)

## Result

**Before** (without namespace provider settings):
```csharp
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1.Requests;
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1.Commands;
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1.Endpoints;
```

**After** (with namespace provider settings):
```csharp
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1;
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1;
namespace Pulse.FieldingTool.Api.FormsConfigurations.V1;
```

All technical folders hidden, namespace stays domain-focused.
