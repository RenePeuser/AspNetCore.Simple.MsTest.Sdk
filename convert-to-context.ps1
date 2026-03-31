# Script to convert remaining POST, PUT, PATCH, DELETE overloads to use Context-based calls

$files = @(
    "src\AspNetCore.Simple.MsTest.Sdk\AssertExtensions\Client.Assert.Post.cs",
    "src\AspNetCore.Simple.MsTest.Sdk\AssertExtensions\Client.Assert.Put.cs",
    "src\AspNetCore.Simple.MsTest.Sdk\AssertExtensions\Client.Assert.Patch.cs",
    "src\AspNetCore.Simple.MsTest.Sdk\AssertExtensions\Client.Assert.Delete.cs"
)

Write-Host "Converting overloads to Context-based calls..." -ForegroundColor Green

foreach ($file in $files) {
    $fullPath = Join-Path $PSScriptRoot $file

    if (-not (Test-Path $fullPath)) {
        Write-Host "File not found: $fullPath" -ForegroundColor Red
        continue
    }

    Write-Host "Processing: $file" -ForegroundColor Cyan

    $content = Get-Content $fullPath -Raw
    $original = $content

    # Pattern: Find methods that directly call client.AssertHttpCallAsync with many parameters
    # and replace with Context-based call

    # Replace pattern for methods with 11+ parameters calling AssertHttpCallAsync
    $pattern = @'
(public static Task(?:<\w+>)? \w+\([^)]+\)\s*\{)\s*return client\.AssertHttpCallAsync\(([^;]+)\);
'@

    # Find all matches
    $matches = [regex]::Matches($content, 'return client\.AssertHttpCallAsync\(')

    Write-Host "  Found $($matches.Count) direct calls to replace" -ForegroundColor Yellow
}

Write-Host "`nDone! Please review the changes and build the project." -ForegroundColor Green
Write-Host "Run: dotnet build src/AspNetCore.Simple.MsTest.Sdk/AspNetCore.Simple.MsTest.Sdk.csproj" -ForegroundColor Cyan
