# Allure.TestingPlatform

Framework-agnostic Allure adapter for [Microsoft.Testing.Platform][mtp]. It
consumes MTP messages (`TestNodeUpdateMessage`) and drives `AllureLifecycle`
from `Allure.Net.Commons`, producing standard Allure result files.

This package is the foundation for framework-specific adapters (xUnit.v3,
NUnit-on-MTP, etc.); those layers add framework-specific enrichment (traits →
labels, parameter formatting, fixture mapping) on top of the lifecycle handled
here.

## Status

**Minimal viable scaffold.** Currently maps the test happy path
(`InProgress` / `Passed` / `Failed` / `Skipped`) to Allure test results.
Steps, fixtures, attachments, parameters and labels will follow in subsequent
PRs.

## Usage

In your MTP-based test project's `Program.cs`:

```csharp
using Allure.TestingPlatform;
using Microsoft.Testing.Platform.Builder;

var builder = await TestApplication.CreateBuilderAsync(args);
// register your test framework's MTP integration first, e.g. builder.AddXunit()
builder.AddAllure();
using var app = await builder.BuildAsync();
return await app.RunAsync();
```

[mtp]: https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro
