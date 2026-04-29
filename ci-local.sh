#!/usr/bin/env bash
set -euo pipefail

export PATH="/home/user/.dotnet:$PATH"
export DOTNET_ROOT="/home/user/.dotnet"

# Must be passed as -p: since MSBuild doesn't read UseSharedCompilation as env var.
# Disables Roslyn compiler server to avoid OpenSSL 3.x "invalid digest" crash
# on Linux when building strong-named assemblies.
SHARED_COMP="-p:UseSharedCompilation=false"

SOLUTION_PATH="allure-csharp.slnx"
BUILD_CONFIGURATION="Release"
REPO_ROOT="/mnt/c/com/github/grootstebozewolf/allure-csharp"

cd "$REPO_ROOT"

echo "=============================="
echo "SDK version"
echo "=============================="
dotnet --version

echo ""
echo "=============================="
echo "Clearing stale build artifacts (Windows-path assets)"
echo "=============================="
# Delete stale project.assets.json so restore regenerates them with correct
# Linux paths instead of Windows paths from a Windows-side build.
# (rm -rf artifacts/obj fails on WSL2/NTFS when files are Windows-locked)
find "$REPO_ROOT/artifacts/obj" -name 'project.assets.json' -delete 2>/dev/null || true
find "$REPO_ROOT/artifacts/obj" -name '*.csproj.nuget.*' -delete 2>/dev/null || true

echo ""
echo "=============================="
echo "STEP 1: Restore packages"
echo "=============================="
dotnet restore "$SOLUTION_PATH" $SHARED_COMP

echo ""
echo "=============================="
echo "STEP 2: Build solution"
echo "=============================="
dotnet build "$SOLUTION_PATH" \
  --no-restore \
  --configuration "$BUILD_CONFIGURATION" \
  -p:Allure_TestTargetVersion=SNAPSHOT \
  -p:Allure_PreRunTestingFlow=true \
  $SHARED_COMP

echo ""
echo "=============================="
echo "STEP 3: Create NuGet packages"
echo "=============================="
dotnet pack "$SOLUTION_PATH" \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION" \
  $SHARED_COMP

echo ""
echo "=============================="
echo "STEP 4: Build test samples"
echo "=============================="
dotnet msbuild "$SOLUTION_PATH" \
  -t:Allure_BuildTestSamples \
  -p:Allure_SampleConfiguration="$BUILD_CONFIGURATION" \
  $SHARED_COMP

echo ""
echo "=============================="
echo "STEP 5: Run test samples"
echo "=============================="
dotnet msbuild "$SOLUTION_PATH" \
  -t:Allure_RunTestSamples \
  -p:Allure_SampleConfiguration="$BUILD_CONFIGURATION" \
  $SHARED_COMP

echo ""
echo "=============================="
echo "STEP 6: Run tests"
echo "=============================="

echo "--- Allure.Net.Commons.Tests ---"
dotnet test ./tests/Allure.Net.Commons.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo "--- Allure.NUnit.Tests ---"
dotnet run --project ./tests/Allure.NUnit.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo "--- Allure.Xunit.Tests ---"
dotnet run --project ./tests/Allure.Xunit.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo "--- Allure.Xunit.v3.Tests ---"
dotnet run --project ./tests/Allure.Xunit.v3.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo "--- Allure.SpecFlow.Tests ---"
dotnet test ./tests/Allure.SpecFlow.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo "--- Allure.Reqnroll.Tests ---"
dotnet test ./tests/Allure.Reqnroll.Tests \
  --no-restore \
  --no-build \
  --configuration "$BUILD_CONFIGURATION"

echo ""
echo "=============================="
echo "CI parity run COMPLETE"
echo "=============================="
