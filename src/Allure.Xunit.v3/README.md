# Allure.Xunit.v3

[![Nuget release](https://img.shields.io/nuget/v/Allure.Xunit.v3?style=flat)](https://www.nuget.org/packages/Allure.Xunit.v3)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.Xunit.v3?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Xunit.v3)

> An Allure adapter for [xUnit.net v3](https://xunit.net/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

## Overview

`Allure.Xunit.v3` integrates [Allure Report](https://allurereport.org/) with
[xUnit.net v3](https://xunit.net/) projects that run on
[Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro).

The package builds on top of `Allure.TestingPlatform` and adds the xUnit.net v3
adapter layer. It reports xUnit test results, Allure metadata attributes,
runtime API calls, parameters, steps, fixtures, and attachments to Allure result
files.

`Allure.Xunit.v3` supports only xUnit.net v3 projects that run through
Microsoft Testing Platform.

## Requirements

Install these packages in the test project:

- `xunit.v3.mtp-v2`
- `Allure.Xunit.v3`

Enable the Microsoft Testing Platform runner:

```xml
<PropertyGroup>
  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="xunit.v3.mtp-v2" Version="..." />
  <PackageReference Include="Allure.Xunit.v3" Version="..." />
</ItemGroup>
```

If `UseMicrosoftTestingPlatformRunner` is not set to `true`, the adapter is not
registered and the project will not produce Allure results through this package.

## Quick Start

After the packages and MSBuild property are added, run the tests with `dotnet run`:

```bash
dotnet run --project ./MyTests.csproj
```

If `dotnet test` is [configured to use Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test#mtp-mode-of-dotnet-test) for your project, you can run:

```bash
dotnet test
```

By default, Allure.Xunit.v3 writes result files to an `allure-results`
directory inside the Microsoft Testing Platform results directory. Microsoft
Testing Platform creates that directory as `TestResults` in the build output
directory unless you pass a different location with `--results-directory`.

A typical default path is:

```
./bin/Debug/net10.0/TestResults/allure-results
```

Use attributes from `Allure.Net.Commons.Attributes` to add metadata to tests:

```csharp
using Allure.Net.Commons.Attributes;
using Xunit;

[AllureFeature("Checkout")]
public class CheckoutTests
{
    [Fact]
    [AllureStory("Apply promo code")]
    [AllureDescription("Checks that a valid promo code changes the order total.")]
    public void AppliesPromoCode()
    {
        // ...
    }
}
```

You can also use `Allure.Net.Commons.AllureApi` to add labels, links,
parameters, steps, and attachments at runtime.

## Configuration

The default registration uses the same configuration discovery as
`Allure.TestingPlatform`:

1. explicit configuration passed during custom registration;
2. the file path from the `ALLURE_CONFIG` environment variable;
3. `allureConfig.json` from the application base directory;
4. default configuration values.

An `allureConfig.json` file can be copied to the output directory:

```xml
<ItemGroup>
  <None Update="allureConfig.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

Example configuration:

```json
{
  "directory": "allure-results",
  "title": "My xUnit.net v3 tests"
}
```

The result directory can also be set for a single run:

```bash
dotnet test -- --allure-results-directory ./artifacts/allure-results
```

## Test Plans

To support test plans, Allure.Xunit.v3 generates an Allure-aware entry point.
The entry point adds xUnit.net pre-execution filter arguments for tests
selected by the current Allure test plan and calls xUnit.net.

If `Allure_GenerateXunitEntryPoint` is set to `false`, the xUnit.net entry point
will be used. Pre-execution filtering is not generated. In that mode,
Allure still applies the test plan at runtime, but xUnit.net may construct
test class instances and run fixtures for tests that are not selected by the plan.

## Advanced Usage

### Custom Allure registration

Disable the default Allure registration when you need to customize
`Allure.TestingPlatform`, for example to use a custom configuration object,
results writer, or enablement rule:

```xml
<PropertyGroup>
  <Allure_XunitEnableSelfRegistration>false</Allure_XunitEnableSelfRegistration>
</PropertyGroup>
```

Then define your own Microsoft Testing Platform builder hook:

```csharp
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform;
using Allure.Xunit;
using Microsoft.Testing.Platform.Builder;

namespace MyTests;

public static class MyAllureBuilderHook
{
    public static void AddExtensions(ITestApplicationBuilder builder, string[] args)
    {
        builder.AddAllureXunit(allure =>
        {
            allure.UseConfiguration(new AllureConfiguration
            {
                Directory = "artifacts/allure-results",
                Title = "My xUnit.net v3 tests",
            });
        });
    }
}
```

And register the hook for self-registration:

```xml
<ItemGroup>
  <TestingPlatformBuilderHook Include="{UUID}">
    <DisplayName>My Allure registration</DisplayName>
    <TypeFullName>MyTests.MyAllureBuilderHook</TypeFullName>
  </TestingPlatformBuilderHook>
</ItemGroup>
```

Use a stable UUID value for `{UUID}`.

### Custom entry point

Use a custom entry point when the test application needs startup logic or when
Allure registration must be configured from code that runs before xUnit.net.

If all you need is to provide custom configuration to Allure, delegate to
the generated `AllureXunitEntryPoint.RunAsync` overload:

```csharp
using System.Threading.Tasks;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform;
using Allure.Xunit;

namespace MyTests;

public static class Program
{
    public static async Task<int> Main(string[] args) =>
        await AllureXunitEntryPoint.RunAsync(allure =>
        {
            allure.UseConfiguration(new AllureConfiguration
            {
                Directory = "artifacts/allure-results",
            });
        }, args);
}
```

Then set `StartupObject` to your entry point and disable the default Allure
registration:

```xml
<PropertyGroup>
  <Allure_XunitEnableSelfRegistration>false</Allure_XunitEnableSelfRegistration>
  <StartupObject>MyTests.Program</StartupObject>
</PropertyGroup>
```

It's important to set `Allure_XunitEnableSelfRegistration` to `false`. Otherwise,
Allure will be registered twice, which results in an exception.

For better control over extension registration, call `AllureXunitRunner.RunAsync`
instead and pass a registration function to it:

```csharp
using System.Threading.Tasks;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform;
using Allure.Xunit;

namespace MyTests;

public static class Program
{
    public static async Task<int> Main(string[] args) =>
        await AllureXunitRunner.RunAsync(
            (builder, argsWithTestPlanFilters) =>
            {
                builder.AddAllureXunit(allure =>
                {
                    allure.UseConfiguration(new AllureConfiguration
                    {
                        Directory = "artifacts/allure-results",
                    });
                });
                // other registrations
            },
            args
        );
}
```

Point `StartupObject` to the entry point class. You may also disable Allure entry point
generation:

```xml
<PropertyGroup>
  <Allure_GenerateXunitEntryPoint>false</Allure_GenerateXunitEntryPoint>
  <StartupObject>MyTests.Program</StartupObject>
</PropertyGroup>
```

`StartupObject` is required in this setup because xUnit.net still generates its
own entry point. Omitting it will result in the `Program has more than one entry
point defined` error or your entry point not being selected.

## MSBuild Properties

| Property | Default | Description |
| --- | --- | --- |
| `UseMicrosoftTestingPlatformRunner` | user-defined | Must be set to `true` to run xUnit.net v3 through Microsoft Testing Platform. |
| `Allure_XunitEnableSelfRegistration` | `true` when MTP is enabled | Registers Allure.Xunit.v3 automatically with Microsoft Testing Platform. |
| `Allure_GenerateXunitEntryPoint` | `$(GenerateSelfRegisteredExtensions)` when MTP is enabled | Generates the Allure-aware xUnit.net entry point. |
| `Allure_ApplyXunitAttribute` | `true` when MTP is enabled | Applies `AllureXunitAttribute` to the test assembly automatically. |
| `Allure_WarnIfMtpDisabled` | `true` | Emits warning `ALLURE001` when the package is referenced without the MTP runner enabled. |
| `StartupObject` | `Allure.Xunit.AllureXunitEntryPoint` when an entry point is generated and `StartupObject` is empty | Selects the application entry point. |

## Troubleshooting

### No Allure results are generated

Check that:

- the project references both `xunit.v3.mtp-v2` and `Allure.Xunit.v3`;
- `UseMicrosoftTestingPlatformRunner` is set to `true`;
- `Allure_XunitEnableSelfRegistration` is not set to `false` unless you provide
  a custom registration;
- the `--allure off` command-line option is not used.

### Warning ALLURE001 is shown

`Allure.Xunit.v3` was referenced by a project that does not enable the
Microsoft Testing Platform runner. Set `UseMicrosoftTestingPlatformRunner` to
`true`, or set `Allure_WarnIfMtpDisabled` to `false` if the warning is expected.

### A custom entry point does not produce Allure results

Make sure the entry point delegates to `AllureXunitEntryPoint.RunAsync` or
`AllureXunitRunner.RunAsync`, and make sure Allure is registered either through
self-registration or by calling `AddAllureXunit`.

### Test plan correctly deselects the tests but class constructors and fixtures are still executed

Make sure `Allure_GenerateXunitEntryPoint` is not set to `false` and that
`StartupObject`, when set, points to an entry point that delegates to the
Allure-generated runner helpers.
