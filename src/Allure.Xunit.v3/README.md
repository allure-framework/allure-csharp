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

## Migrating from Allure.Xunit?

`Allure.Xunit` targets xUnit.net v2 and uses an older Allure API. See the
[migration guide](https://github.com/allure-framework/allure-csharp/blob/main/src/Allure.Xunit.v3/MIGRATION.md)
for the package, attribute, runtime API, configuration, and model changes
required when moving to `Allure.Xunit.v3`.

## Requirements

Install the `Allure.Xunit.v3` package in your xUnit.net v3 MTP test project.
If you don't have one, consult
[this article](https://xunit.net/docs/getting-started/v3/getting-started) on
how to create it.

Currently, `Allure.Xunit.v3` only supports the `xunit.v3.mtp-v2` xUnit.net package,
so make sure you've got this exact package installed.

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

## Limitations

`Allure.Xunit.v3` currently has the following limitations:

- It does not support the native xUnit.net runner. The project must enable
  the MTP runner.
- Unlike `Allure.Xunit`, it cannot delegate to or run alongside a second xUnit
  runner reporter.
- It cannot be used in the same test process with another integration based on
  `Allure.TestingPlatform`.
- Model read and update operations through `AllureInProcessApi` are not
  supported yet.
- Instrumentation attributes such as `AllureStep`, `AllureSetUp`,
  `AllureTearDown`, and attachment attributes use AspectInjector. On Apple
  silicon Macs, using these attributes may require Rosetta 2.
- In server mode, the configuration is not re-read until the process is
  restarted.
- Only one MTP request can be active at a time; parallel requests are not
  supported.

## Quick Start

After the packages and MSBuild property are added, run the tests with `dotnet run`:

```bash
dotnet run --project ./MyTests.csproj
```

If `dotnet test` is [configured to use Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test#mtp-mode-of-dotnet-test) for your project, you can run:

```bash
dotnet test
```

By default, `Allure.Xunit.v3` writes result files to an `allure-results`
directory inside the Microsoft Testing Platform results directory. Microsoft
Testing Platform creates that directory as `TestResults` in the build output
directory unless you pass a different location with `--results-directory`.

A typical default path is:

```
./bin/Debug/net10.0/TestResults/allure-results
```

Use attributes and the `Allure.AllureApi` facade from the `Allure` namespace
to add metadata to tests:

```csharp
using Allure;
using Xunit;

[AllureFeature("Checkout")]
public class CheckoutTests
{
    [Fact]
    [AllureStory("Apply promo code")]
    [AllureDescription("Checks that a valid promo code changes the order total.")]
    public void AppliesPromoCode()
    {
        AllureApi.Step("Create order", () => { /* ... */ });
        // ...
    }
}
```

Note, that model read and update operations from `Allure.AllureInProcessApi`
are not supported yet.

## Configuration

Create `allure.config.json` with the required configuration and make sure the
file is copied to the output directory:

```xml
<ItemGroup>
  <None Update="allure.config.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

The old `allureConfig.json` name is also supported.

Example configuration:

```json
{
  "$schema": "https://allure-framework.github.io/allure-csharp/schemas/allure.xunit.v3/allure.config.schema.json",
  "resultsDirectory": "allure-results",
  "hostname": "build-agent",
  "linkTemplates": {
    "issue": {
      "urlTemplate": "https://github.com/allure-framework/allure-csharp/issues/{0}",
      "nameTemplate": "Issue {0}"
    }
  },
  "globalLabels": {
    "layer": "unit",
    "project": "Allure.Xunit.v3"
  }
}
```

All properties are optional. JSON property names are case-sensitive.

| Property | Default | Description |
| --- | --- | --- |
| `hostname` | Current machine name | Host name recorded in generated test results. |
| `resultsDirectory` | `<MTP results directory>/allure-results` | Directory where Allure result files are written. Relative paths are resolved against the test process working directory. |
| `linkTemplates` | `{}` | Map of link types to template objects. Each object contains a `urlTemplate` and may contain a `nameTemplate`; both use `{0}` for the original link value. Values that are already valid URLs are not affected. |
| `failExceptions` | `[]` | Exception type names that should produce a `failed` status instead of `broken`. |
| `indentOutput` | `false` | Whether generated JSON files are indented. |
| `globalLabels` | `{}` | Map of label names to values applied to every test result. |
| `runtimeRegistrationHook` | Not set | Assembly-qualified name of an [`IAllureXunitRegistrationHook`](#registration-hook) implementation that is invoked during Allure registration. |
| `isEnabled` | `true` | Whether the Allure integration is enabled. |
| `isProcessWatchdogEnabled` | `true` | Whether an Allure global error is written when the test host process exits unexpectedly. |

### CLI arguments

The `--allure`, `--allure-watchdog`, and `--allure-results-directory`
command-line options override their corresponding configuration properties for
the current run:

```bash
dotnet test -- --allure off
dotnet test -- --allure-watchdog off
dotnet test -- --allure-results-directory ./artifacts/allure-results
```

### Configuration resolution

The default registration checks the following configuration sources
in order from highest to lowest precedence:

1. explicit configuration passed during custom registration;
2. the file path from the `ALLURE_CONFIG` environment variable;
3. `allureConfig.json` from the application base directory;
4. `allure.config.json` from the application base directory;
5. default configuration values.

The first source that provides a configuration wins, and resolution stops.
Configuration sources are not merged: values omitted by the selected source
are not filled from sources later in the list.

## Selective run

Allure tooling may instruct integrations to run only a subset of tests by
providing a test plan. Normally, the test plan is generated and passed to the
test process automatically. If no test plan is supplied, all tests run.

`Allure.Xunit.v3` can enforce the selection in two modes.

### Generated entry point

This is the default mode. The Allure-generated entry point translates the
selection into xUnit.net pre-execution filter arguments before starting the test
run.

Tests excluded by the selection are not started by xUnit.net. This avoids
constructing their test class instances and running their per-test lifecycle
code. If all tests in a test collection are excluded, its collection fixtures
and other collection-level lifecycle code do not run either.

### Native xUnit.net entry point

Set `Allure_GenerateXunitEntryPoint` to `false` to use xUnit.net's native entry
point:

```xml
<PropertyGroup>
  <Allure_GenerateXunitEntryPoint>false</Allure_GenerateXunitEntryPoint>
</PropertyGroup>
```

Without the generated entry point, Allure cannot add pre-execution filters.
Instead, it applies the selection at runtime before invoking each test method.
Excluded test methods do not run and do not produce Allure test results.

However, xUnit.net may still construct test classes and run fixtures or other
lifecycle code associated with excluded tests. Use the generated entry point
when avoiding those side effects is important.

A custom entry point can retain pre-execution filtering by delegating to
`AllureXunitEntryPoint.RunAsync` or `AllureXunitRunner.RunAsync`. See
[Custom entry point](#custom-entry-point).

## Customize Allure.Xunit.v3

### Registration hook

A registration hook provides a reusable way to customize an `Allure.Xunit.v3`
registration.

Define a class that implements `IAllureXunitRegistrationHook`:

```csharp
using System;
using System.Globalization;
using Allure.Sdk.Registration;
using Allure.Xunit.Registration;

namespace MyTests;

public sealed class ProjectAllureRegistration : IAllureXunitRegistrationHook
{
    public void SetUp(IAllureXunitRegistrationContext context)
    {
        context.ConfigureSerialization(
            rules => rules.AddDelegateRule(
                (DateTime value) => value.ToString("o", CultureInfo.InvariantCulture)
            )
        );
    }
}
```

The hook above instructs `Allure.Xunit.v3` to use a custom rule when
serializing `DateTime` arguments of tests, steps, and fixtures.

To register a hook programmatically, assign an instance to
`AllureXunitRegistrationHook.Current` before Allure registration begins:

```csharp
using System.Runtime.CompilerServices;
using Allure.Xunit.Registration;

namespace MyTests;

internal static class AllureSetup
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        AllureXunitRegistrationHook.Current = new ProjectAllureRegistration();
    }
}
```

Alternatively, select the hook through the `runtimeRegistrationHook` property in
`allure.config.json`. Specify the hook type followed by the name of its assembly:

```json
{
  "$schema": "https://allure-framework.github.io/allure-csharp/schemas/allure.xunit.v3/allure.config.schema.json",
  "runtimeRegistrationHook":
    "MyTests.ProjectAllureRegistration, MyTests"
}
```

Only the simple assembly name is required. Assembly version, culture, and
public key token may be omitted. If the hook type is not declared in the
test assembly, the test project must reference the project or package that
contains it.

Alternatively, set the `ALLURE_XUNIT_REGISTRATION_HOOK` environment variable:

```bash
export ALLURE_XUNIT_REGISTRATION_HOOK="MyTests.ProjectAllureRegistration, MyTests"
```

Or, on Windows:

```powershell
$Env:ALLURE_XUNIT_REGISTRATION_HOOK = "MyTests.ProjectAllureRegistration, MyTests"
```

Hooks selected through configuration or an environment variable must have a
public parameterless constructor.

If multiple hooks are provided, they all run in sequence with
the environment hook running first, followed by the configuration hook,
followed by `AllureXunitRegistrationHook.Current`.

### Parameter serialization

Allure uses ordered serialization rules to convert CLR parameter values to
text. Configure these rules during runtime registration with
`ConfigureSerialization`; rules cannot be added after the Allure runtime has
been registered.

Use `AddDelegateRule<T>` for simple conversions. For reusable rules, implement
`IParameterSerializationRule` or derive from
`TypedParameterSerializationRule<T>`. Typed rules accept subclasses and
interface implementations. If multiple rules accept a value, the last added
matching rule has priority.

Allure automatically applies the configured rules to:

- test method arguments;
- values passed to `AddTestParameterFromObject` and
  `AddTestParameterFromObjectAsync`;
- values passed to `AddParameterFromObject` and
  `AddParameterFromObjectAsync` on step and fixture contexts;
- arguments of methods marked with `[AllureStep]`, `[AllureSetUp]`, or
  `[AllureTearDown]`.

The serialized values are also used for argument placeholders in instrumented
step and fixture names.

`AddTestParameter`, `AddTestParameterAsync`, `AddParameter`, and
`AddParameterAsync` accept already-formatted strings and bypass the parameter
serializer.

When migrating from `Allure.Net.Commons`, see
[Migrate type formatters](https://github.com/allure-framework/allure-csharp/blob/main/src/Allure.Xunit.v3/MIGRATION.md#migrate-type-formatters).

### Manual Allure registration

Register `Allure.Xunit.v3` manually when the project already configures
Microsoft Testing Platform through an `ITestApplicationBuilder` hook. This
allows the Allure registration to use builder arguments, share objects with
other extensions, and participate in the same extension-registration workflow.

To register Allure manually, disable the default Allure registration:

```xml
<PropertyGroup>
  <Allure_XunitEnableSelfRegistration>false</Allure_XunitEnableSelfRegistration>
</PropertyGroup>
```

Then define a Microsoft Testing Platform builder hook if you don't have one.
Call `AddAllureXunit` from its `AddExtensions` method:

```csharp
using Allure.Sdk.Registration;
using Allure.Xunit;
using Allure.Xunit.Configuration;
using Microsoft.Testing.Platform.Builder;

namespace MyTests;

public static class MyAllureBuilderHook
{
    public static void AddExtensions(ITestApplicationBuilder builder, string[] args)
    {
        builder.AddAllureXunit(allure =>
        {
            allure.UseConfiguration(new AllureXunitConfiguration
            {
                ResultsDirectory = "artifacts/allure-results",
                Hostname = "My xUnit.net v3 tests",
            });
        });
    }
}
```

Finally, register the builder hook with Microsoft Testing Platform:

```xml
<ItemGroup>
  <TestingPlatformBuilderHook Include="{UUID}">
    <DisplayName>My Allure registration</DisplayName>
    <TypeFullName>MyTests.MyAllureBuilderHook</TypeFullName>
  </TestingPlatformBuilderHook>
</ItemGroup>
```

Use a stable UUID value for `{UUID}`.

The callback passed to `AddAllureXunit` receives the same
`IAllureXunitRegistrationContext` available to Allure registration hooks.
Manual registration changes where registration is initiated; it does not
provide a different Allure configuration API.

### Custom entry point

Use a custom entry point when the test application needs startup logic or when
Allure registration must be configured from code that runs before xUnit.net.

If all you need is to provide custom configuration to Allure, delegate to
the generated `AllureXunitEntryPoint.RunAsync` overload:

```csharp
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.Xunit.Configuration;
using Allure.Xunit.Generated;
using Allure.Xunit.Registration;

namespace MyTests;

public static class Program
{
    public static async Task<int> Main(string[] args) =>
        await AllureXunitEntryPoint.RunAsync(allure =>
        {
            allure.UseConfiguration(new AllureXunitConfiguration
            {
                ResultsDirectory = "artifacts/allure-results",
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
using Allure.Sdk.Registration;
using Allure.TestingPlatform;
using Allure.Xunit;
using Allure.Xunit.Configuration;
using Allure.Xunit.Generated;

namespace MyTests;

public static class Program
{
    public static async Task<int> Main(string[] args) =>
        await AllureXunitRunner.RunAsync(
            (builder, argsWithTestPlanFilters) =>
            {
                builder.AddAllureXunit(allure =>
                {
                    allure.UseConfiguration(new AllureXunitConfiguration
                    {
                        ResultsDirectory = "artifacts/allure-results",
                    });
                });
                // other registrations
            },
            args
        );
}
```

Point `StartupObject` to the entry point class. You may also disable Allure entry point
generation because you don't need it in this setup:

```xml
<PropertyGroup>
  <Allure_GenerateXunitEntryPoint>false</Allure_GenerateXunitEntryPoint>
  <StartupObject>MyTests.Program</StartupObject>
</PropertyGroup>
```

`StartupObject` is still required because xUnit.net still generates its
own entry point. Omitting it will result in either the `Program has more than one entry
point defined` error or the wrong entry point being selected.

## MSBuild Properties

| Property | Default | Description |
| --- | --- | --- |
| `UseMicrosoftTestingPlatformRunner` | user-defined | Must be set to `true` to run xUnit.net v3 through Microsoft Testing Platform. |
| `Allure_XunitEnableSelfRegistration` | `true` when MTP is enabled | Registers `Allure.Xunit.v3` automatically with Microsoft Testing Platform. |
| `Allure_GenerateXunitEntryPoint` | `$(GenerateSelfRegisteredExtensions)` when MTP is enabled | Generates the Allure-aware xUnit.net entry point. |
| `Allure_ApplyXunitAttribute` | `true` when MTP is enabled | Applies `AllureXunitAttribute` to the test assembly automatically. |
| `Allure_WarnIfMtpDisabled` | `true` | Emits warning `ALLURE001` when the package is referenced without the MTP runner enabled. |
| `StartupObject` | `Allure.Xunit.Generated.AllureXunitEntryPoint` when an entry point is generated and `StartupObject` is empty | Selects the application entry point. |

## Troubleshooting

### No Allure results are generated

First, check `resultsDirectory` in the selected configuration and the
`--allure-results-directory` option. If neither is set, Allure writes to
`<MTP results directory>/allure-results`. With the default Microsoft Testing
Platform location, a typical path is something like
`./bin/Debug/net10.0/TestResults/allure-results`. This differs from `Allure.Xunit`,
which used `<build-output>/allure-results`.

If the path is correct but the directory is still empty, check that:

- the project references both `xunit.v3.mtp-v2` and `Allure.Xunit.v3`;
- `UseMicrosoftTestingPlatformRunner` is set to `true`;
- the Allure runner reporter is active: automatic reporter activation is enabled
  and no other reporter is explicitly selected;
- `Allure_XunitEnableSelfRegistration` is not set to `false` unless you provide
  a custom registration;
- the `--allure off` command-line option is not used;
- the configuration file does not set `isEnabled` to `false`.

### Warning ALLURE001 is shown

`Allure.Xunit.v3` was referenced by a project that does not enable the
Microsoft Testing Platform runner. Set `UseMicrosoftTestingPlatformRunner` to
`true`, or set `Allure_WarnIfMtpDisabled` to `false` if the warning is expected.

### Allure runner reporter is disabled

`Allure.Xunit.v3` relies on `AllureRunnerReporter` to correlate xUnit test
lifecycle messages with Allure API calls. xUnit.net can activate only one runner
reporter at a time, so activating another reporter prevents
`AllureRunnerReporter` from being activated.

The Allure runtime can still be registered when `AllureRunnerReporter` is
inactive. This partial state is not supported: results may be missing or
incomplete, and API calls that require test correlation may throw
`Could not correlate Allure messages: AllureRunnerReporter was disabled.`

To use Allure, leave `--auto-reporters` unset or set it to `on`, remove reporter
switches such as `--report-junit`, and ensure no other environmentally enabled
reporter is registered.

To use another reporter, disable Allure with `--allure off` or set `isEnabled`
to `false`. If you use default self-registration, you can instead set the
`Allure_XunitEnableSelfRegistration` MSBuild property to false. The same value
can be supplied as an environment variable when building the project.

### A custom entry point does not produce Allure results

Make sure the entry point delegates to `AllureXunitEntryPoint.RunAsync` or
`AllureXunitRunner.RunAsync`, and make sure Allure is registered either through
self-registration or by calling `AddAllureXunit`.

### Test plan correctly deselects the tests but class constructors and fixtures are still executed

Make sure `Allure_GenerateXunitEntryPoint` is not set to `false` and that
`StartupObject`, when set, points to an entry point that delegates to the
Allure-generated runner helpers.
