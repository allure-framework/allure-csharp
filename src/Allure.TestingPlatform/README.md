# Allure adapter for Microsoft Testing Platform (MTP) frameworks

[![NuGet release](https://img.shields.io/nuget/v/Allure.TestingPlatform?style=flat)](https://www.nuget.org/packages/Allure.TestingPlatform)
[![NuGet downloads](https://img.shields.io/nuget/dt/Allure.TestingPlatform?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.TestingPlatform)

> An Allure adapter for [Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro) frameworks.

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) – stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

## Overview

`Allure.TestingPlatform` integrates [Allure Report](https://allurereport.org/) with test frameworks based on
[Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro).

The package can be used in two ways:

- as a standalone adapter in an MTP test project;
- as an embedded runtime for framework-specific Allure adapters built on top of MTP.

Test projects should use standalone mode. Embedded mode is intended for adapter authors who need to produce
Allure results from their own MTP extensions.

## Installation

Install the package in your test project:

```xml
<PackageReference Include="Allure.TestingPlatform" Version="..." />
```

The package uses Microsoft Testing Platform self-registration by default, so no additional code is required for the
default standalone setup.

## Standalone Mode

Standalone mode registers `Allure.TestingPlatform` as the Allure adapter for the test project. This is the default mode
used by package self-registration and by `builder.AddAllure()`.

### Automatic registration

`Allure.TestingPlatform` ships a Microsoft.Testing.Platform.MSBuild self-registration hook:

- `Allure.TestingPlatform.AllureTestingPlatformBuilderHook`
- registered through `TestingPlatformBuilderHook` metadata in `buildTransitive`
- enabled by default with `Allure_TestingPlatformEnableSelfRegistration=true`

When the hook is enabled, Microsoft.Testing.Platform.MSBuild calls `AllureTestingPlatformBuilderHook.AddExtensions`.
The method registers `Allure.TestingPlatform` in standalone mode with default options.

### Disable automatic registration

Set this property in the test project to disable the self-registration hook:

```xml
<PropertyGroup>
  <Allure_TestingPlatformEnableSelfRegistration>false</Allure_TestingPlatformEnableSelfRegistration>
</PropertyGroup>
```

You can then register the adapter explicitly from your Microsoft Testing Platform builder setup:

```csharp
builder.AddAllure();
```

or configure it:

```csharp
builder.AddAllure(allure =>
{
    allure.UseConfigurationFile("allureConfig.json");
});
```

To disable the self-registration for a single test run, pass the same MSBuild property to `dotnet run` or `dotnet test`:

```bash
dotnet run --project "./<test-project-path>" -p:Allure_TestingPlatformEnableSelfRegistration=false
```

## Standalone Registration

Standalone registration is performed through `IStandaloneAllureRegistrationContext`.

```csharp
builder.AddAllure(allure =>
{
    allure.UseConfiguration(new AllureConfiguration
    {
        Directory = "allure-results",
        Title = "My test report"
    });
});
```

Available standalone registration methods:

| API | Description |
| --- | --- |
| `UseConfiguration(...)` | Sets the factory used to create `AllureConfiguration`. |
| `UseConfigurationFile(...)` | Reads `AllureConfiguration` from the specified JSON file. |
| `SetIsEnabled(...)` | Decides whether the Allure runtime is enabled. |
| `UseWriter(...)` | Sets the `IAllureResultsWriter` factory. |
| `UseTypeFormatters(...)` | Sets type formatters used to serialize parameter values. |
| `DisableHostProcessWatchdog()` | Disables the crash watchdog for this registration. |

### Configuration discovery

The built-in registration resolves the configuration in the following order:

1. a configuration object or factory passed to `UseConfiguration(...)`, or an explicit path passed to `UseConfigurationFile(...)`;
2. the path from the `ALLURE_CONFIG` environment variable;
3. `allureConfig.json` from the application base directory;
4. default configuration values.

Configuration may be stored directly at the root of the JSON file:

```json
{
  "directory": "allure-results",
  "title": "My test report",
  "links": [
    "https://example.org/{issue}"
  ],
  "indentOutput": true
}
```

For backward compatibility, the configuration can also be placed under the `"allure"` property:

```json
{
  "allure": {
    "directory": "allure-results",
    "title": "My test report",
    "links": [
      "https://example.org/{issue}"
    ],
    "indentOutput": true
  }
}
```

If Microsoft Testing Platform provides a result directory and the Allure configuration does not set one, the default
Allure output directory is placed under the MTP result directory as `allure-results`.

### Custom configuration types

Use `ConfigurationFunctions.ReadConfiguration<TConfiguration>(...)` when an adapter needs a subclass of
`AllureConfiguration` with additional properties:

```csharp
builder.AddAllure(allure =>
{
    allure.UseConfiguration(serviceProvider =>
        ConfigurationFunctions.ReadConfiguration<MyAllureConfiguration>(
            serviceProvider,
            "allureConfig.json"
        )
    );
});
```

## CLI Options

`Allure.TestingPlatform` registers these Microsoft Testing Platform command-line options:

| Option | Values | Default | Description |
| --- | --- | --- | --- |
| `--allure` | `on`, `off` | `on` | Enables or disables the Allure runtime. |
| `--allure-watchdog` | `on`, `off` | `on` | Enables or disables the crash watchdog. |
| `--allure-results-directory` | path | automatic | Sets the directory where Allure result files are written. |

Examples:

```bash
dotnet test -- --allure off
dotnet test -- --allure-watchdog off
dotnet test -- --allure-results-directory ./artifacts/allure-results
```

`--allure-results-directory` is honored by the built-in configuration reader, including configurations loaded via `UseConfigurationFile(...)`. If you replace the configuration factory, the custom factory is responsible for honoring this CLI option.

## Embedded Mode

Embedded mode is for framework-specific adapters that want to reuse the Allure.TestingPlatform runtime, lifecycle,
writer, configuration, and message-processing pipeline.

```csharp
var runtimeReferences = builder.AddEmbeddedAllure(allure =>
{
    allure.UseConfigurationFile<MyAllureConfiguration>("allureConfig.json");
    allure.UseMtpSessionCorrelation();
});
```

Embedded mode uses `IEmbeddedAllureRegistrationContext`. It includes the standalone configuration hooks and adds
SDK-focused hooks:

| API | Description |
| --- | --- |
| `UseCorrelation(...)` | Sets the strategy used to correlate SDK messages with MTP sessions. |
| `UseLogger(...)` | Sets the Allure logger factory. |
| `UseLifecycle(...)` | Sets the `AllureLifecycle` factory. |
| `SetSdkEventHandlers(...)` | Registers handlers for runtime lifecycle events. |
| `UseConfigurationFile<TConfiguration>(...)` | Reads a custom `AllureConfiguration` subtype from JSON. |
| `UseConfiguration<TConfiguration>(...)` | Sets the configuration instance factory. |
| `UseMtpSessionCorrelation()` | Uses the MTP session UID as the correlation identifier. |
| `UseTestNodeMetadataCorrelation()` | Reads the correlation identifier from test node metadata. |

`AddEmbeddedAllure(...)` returns `IAllureTestingPlatformRuntimeReferenceRegistry`. Use it to resolve the runtime
associated with an MTP test application by `IServiceProvider`.

## Messages And Properties

Embedded adapters communicate with the Allure runtime by publishing Microsoft Testing Platform data messages. These
messages describe Allure lifecycle operations without requiring the adapter to mutate the Allure lifecycle directly.

Available message groups:

- scopes: `AllureScopeStartMessage`, `AllureScopeStopMessage`, `AllureScopeTestsMessage`;
- fixtures: `AllureBeforeFixtureStartMessage`, `AllureAfterFixtureStartMessage`,
  `AllureFixtureUpdateMessage`, `AllureFixtureStopMessage`;
- tests: `AllureTestUpdateMessage`;
- base operation messages for creating, updating, and removing Allure model contexts.

Messages carry a `CorrelationUid` and context identifiers such as `ScopeContextUid`, `FixtureContextUid`, and
`TestContextUid`. The data consumer uses these identifiers to buffer, route, and apply operations to the correct
Allure lifecycle context.

Messages can be customized with Allure properties. The SDK includes properties for:

- names, full names, descriptions, and HTML descriptions;
- status, status details, and exception-derived status;
- start, stop, and duration updates;
- labels, links, parameters, title path, and default suite labels;
- byte-array and file attachments;
- reflected test method metadata and argument serialization.

## Identifiers

Allure.TestingPlatform does not manage the model objects directly. Instead, it stores their associated contexts and uses the context identifiers provided with the messages:

  - `ScopeContextUid` for scopes (i.e., containers),
  - `FixtureContextUid` for fixtures,
  - `TestContextUid` for tests,
  - `StepContextUid` for steps.

Prefer issuing a unique identifier for each context. If that is hard to achieve, make sure no two contexts of the same type share the same ID at the same time.

Unlike other identifiers, test context UIDs are created from the `TestNode.Uid` property, which is provided by the test framework via `TestNodeUpdateMessage`. You must create `TestContextUid` from exactly the same value when producing test-context-bound messages like `AllureTestUpdateMessage`. MTP requires `TestNode.Uid` values to be stable between runs, so frameworks usually assign stable test identifiers that are available via the public API. Refer to the documentation and the source code of the framework you're integrating with for more details.

## Correlation

Not all testing frameworks provide access to the current `SessionUid` value through the public API, which makes associating Allure messages with MTP messages like `TestNodeUpdateMessage` not a trivial task. `CorrelationUid` is an alternative session identifier that is expected to have a one-to-one relationship with `SessionUid` and be accessible to integration code.

It's up to the adapter how to construct `CorrelationUid`, as long as the value can be associated with an MTP message, or the message producer. The rules for achieving this are represented by the `ICorrelationStrategy` interface:


```csharp
public interface ICorrelationStrategy
{
    Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken);
}
```

Built-in strategies:

- `TestingPlatformSessionUidCorrelationStrategy` uses the MTP `SessionUid`. Suitable for test frameworks that publish the `SessionUid` via their public API.
- `TestNodeMetadataCorrelationStrategy` reads a value from `TestMetadataProperty` with key `Allure.TestingPlatform.CorrelationUid`. Suitable for adapters that can include custom metadata in a `TestNodeUpdateMessage` sent by the test framework.

## Watchdog

`AllureTestingPlatformHostProcessWatchdog` records a global Allure error if the test host process crashes.

The watchdog is enabled by default.

In nested MTP runs, the watchdog may cause the application to hang. If that happens, disable the watchdog via the registration API:

```csharp
builder.AddAllure(allure =>
{
    allure.DisableHostProcessWatchdog();
});
```

or from the command line:

```bash
dotnet run --project "./<path-to-project>" -- --allure-watchdog off
```

## Runtime Extension Base Classes

Integration authors can build MTP extensions on top of the Allure runtime base classes.

`AllureTestingPlatformExtension` implements the common MTP `IExtension` members and exposes protected runtime services:

- `Logger`
- `Configuration`
- `Writer`
- `TypeFormatters`
- `Lifecycle`
- `CorrelationStrategy`
- `ConfiguredRuntime`
- `LiveRuntime`

Use this base class for extensions that consume an already configured or live runtime.

`AllureTestingPlatformRuntimeControllerExtension` is for extensions that configure or start the runtime. It wraps an
`IAllureTestingPlatformRuntimeController`, configures the runtime from `IsEnabledAsync`, and exposes
`EnsureRuntimeStarted()` for process-lifetime handlers that may need to write results during late events such as a host
process crash.

Exactly one extension inheriting from `AllureTestingPlatformRuntimeControllerExtension` is expected to exist in each process that accesses the Allure.TestingPlatform runtime. In the test host process, this is always `AllureTestingPlatformInProcessRuntimeController`, which makes the runtime available for all in-process extensions. If an out-of-process extension requires the runtime, it must either inherit from `AllureTestingPlatformRuntimeControllerExtension` or be accompanied by such an extension.

## Troubleshooting

### No Allure result files are written

If no Allure results are written, check the following in order:

  - The `--allure off` option was not provided.
  - If `SetIsEnabled` is used, the callback returned `true`.
  - If self-registration is used, the build hook is called from `SelfRegisteredExtensions.cs` (auto-generated in the `obj` directory by Microsoft.Testing.Platform.MSBuild):
    - For default Allure.TestingPlatform registration, the `SelfRegisteredExtensions` class must call `global::Allure.TestingPlatform.AllureTestingPlatformBuilderHook.AddExtensions`. If no such call is present, check if the `Allure_TestingPlatformEnableSelfRegistration` MSBuild property is `true`.
    - If you suppress the default registration and provide your own, ensure you pass it to MSBuild correctly. Consult [this article](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-architecture-extensions#auto-register-your-extension-with-testingplatformbuilderhook) for more details.
  - If you don't use self-registration, make sure `AddAllure` is called from the entry point of the test application.

### Self-registration runs unexpectedly

Disable the self-registration hook:

```xml
<PropertyGroup>
  <Allure_TestingPlatformEnableSelfRegistration>false</Allure_TestingPlatformEnableSelfRegistration>
</PropertyGroup>
```

Or pass the MSBuild property on the command line:

```shell
dotnet run --project "./<path-to-project>" -p:Allure_TestingPlatformEnableSelfRegistration=false
```

### `--allure-results-directory` is ignored

The option is applied by the built-in configuration reader. If you provide a custom configuration factory with
`UseConfiguration(...)`, make sure that factory reads the CLI option if you want to support it:

```csharp
builder.AddAllure((allure) =>
{
    allure.UseConfiguration((serviceProvider) =>
    {
        var commandLineOptions = serviceProvider.GetCommandLineOptions();
        var resultsDir =
            // namespace: Allure.TestingPlatform.Sdk.TestingPlatformExtensions
            AllureCliOptionsProvider.GetResultsDirectoryValue(commandLineOptions)
                ?? fallbackLocation;

        return new AllureConfiguration
        {
            Directory = resultsDir,
        };
    });
});
```

### Embedded messages are not applied

Check that the adapter configured a suitable correlation strategy and that SDK messages use the expected `CorrelationUid`.
Messages that cannot be correlated are buffered until correlation becomes available. If correlation cannot be established from a `TestNodeUpdateMessage` or its producer, no Allure messages will be processed.
