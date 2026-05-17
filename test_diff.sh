#!/bin/bash
# Test script for character-level diff highlighting

echo "Building in Release mode (with ANSI colors)..."
dotnet build --configuration Release -v quiet

echo ""
echo "Running test with differences to see colored output..."
dotnet test src/MinimalApi.Test/MinimalApi.Test.csproj \
  --filter "FullyQualifiedName~Should_Be_Able_To_Post_A_Person_Object" \
  --configuration Release \
  --no-build \
  --verbosity normal
