# Allure adapter for Microsoft Testing Platform (MTP) frameworks

[![NuGet release](https://img.shields.io/nuget/v/Allure.TestingPlatform?style=flat)](https://www.nuget.org/packages/Allure.TestingPlatform)
[![NuGet downloads](https://img.shields.io/nuget/dt/Allure.TestingPlatform?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.TestingPlatform)

> Allure integration for test frameworks based on [Microsoft Testing Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) – stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) – explore a live example of Allure Report in action

---

## Choose the right integration

Prefer an Allure adapter built specifically for your test framework whenever
one is available. A framework-specific adapter can understand the framework's
test methods, parameters, fixtures, metadata, and execution context, and can
support the Allure runtime and attribute APIs.

Use `Allure.TestingPlatform` directly in a test project only as a last-resort fallback when no framework-specific adapter exists.

| Use case | Recommendation |
| --- | --- |
| Run tests with a supported framework | Install that framework's Allure adapter. |
| Run tests with an MTP-based framework that has no Allure adapter | Use `Allure.TestingPlatform` in standalone mode as a limited fallback. |
| Build an Allure adapter for an MTP-based framework | Build the adapter on top of the `Allure.TestingPlatform` SDK. |

Standalone mode observes the generic events exposed by Microsoft Testing Platform. It does **not** provide:

- Allure runtime API support, including calls made through `AllureApi` or `AllureInProcessApi`;
- Allure attribute API support;
- framework-specific metadata such as reflected test methods, declared parameters, framework fixture semantics, or framework-specific labels and hierarchy.

A specialized framework adapter can provide these capabilities by connecting the framework lifecycle and execution context to `Allure.TestingPlatform`.

## Use standalone mode

### Install the package

Add `Allure.TestingPlatform` to the test project:

```xml
<PackageReference Include="Allure.TestingPlatform" Version="..." />
```

The package registers itself with Microsoft Testing Platform by default. No application code is required for the default standalone setup.

### Configure registration

To replace automatic registration with explicit registration, disable the package's MSBuild hook:

```xml
<PropertyGroup>
  <Allure_TestingPlatformEnableSelfRegistration>false</Allure_TestingPlatformEnableSelfRegistration>
</PropertyGroup>
```

Then register Allure from the Microsoft Testing Platform builder setup:

```csharp
using Allure.Sdk.Registration;
using Allure.TestingPlatform;
using Allure.TestingPlatform.Configuration;

builder.AddAllure(allure =>
{
    allure.UseConfiguration(new AllureTestingPlatformConfiguration
    {
        ResultsDirectory = "allure-results",
    });
});
```

The following registration methods are available in standalone mode:

- `UseConfiguration`, `UseConfigurationSource`, `UseConfigurationFile`, and `UseConfigurationPathEnvironmentVariable` select configuration sources;
- `TransformConfiguration` adjusts the resolved configuration;
- `ConfigureSerialization` customizes parameter serialization rules;
- `UseParameterSerializer` replaces parameter serialization;
- `UseDestination` replaces the results destination;
- `Disable` disables the runtime;
- `DisableHostProcessWatchdog` disables the test-host crash watchdog.

Do not call `AddAllure` while automatic registration is enabled, because that would register the standalone integration twice.

To disable automatic registration for a single invocation, pass the MSBuild property separately from MTP options:

```bash
dotnet test -p:Allure_TestingPlatformEnableSelfRegistration=false
```

### Configure Allure

Without explicit registration, configuration is loaded from the first available source:

1. the file identified by the `ALLURE_CONFIG` environment variable;
2. `allureConfig.json` in the application base directory;
3. default values.

An explicit configuration source registered through `AddAllure` replaces that default source list.

The canonical JSON format is:

```json
{
  "resultsDirectory": "allure-results",
  "hostname": "test-host",
  "linkTemplates": {
    "issue": {
      "urlTemplate": "https://issues.example.org/{0}",
      "nameTemplate": "Issue {0}"
    }
  },
  "failExceptions": [
    "MyAssertions.AssertionException"
  ],
  "indentOutput": true,
  "globalLabels": {
    "owner": "quality-team"
  },
  "isEnabled": true,
  "isProcessWatchdogEnabled": true
}
```

For compatibility with older Allure .NET configuration files, the reader also accepts configuration under a top-level `"allure"` property and the legacy `directory`, `title`, and `links` properties. New configuration should use the canonical properties above.

If `resultsDirectory` is unset, Allure writes to an `allure-results`
subdirectory of the MTP results directory. MTP defaults that directory to
`TestResults`, resolved relative to the current working directory, so the usual result path there is
`TestResults/allure-results`. When the
test project is run through `dotnet run` or `dotnet test`, the working directory
is the build output directory.

### Command-line options

`Allure.TestingPlatform` adds these Microsoft Testing Platform options:

| Option | Values | Default | Description |
| --- | --- | --- | --- |
| `--allure` | `on`, `off` | `on` | Enables or disables Allure. |
| `--allure-watchdog` | `on`, `off` | `on` | Enables or disables the test-host crash watchdog. |
| `--allure-results-directory` | path | automatic | Overrides the Allure results directory. |

For example:

```bash
dotnet test -- --allure off
dotnet test -- --allure-watchdog off
dotnet test -- --allure-results-directory ./artifacts/allure-results
```

Command-line values override values loaded from configuration sources.

### Test-host crash watchdog

The test-host crash watchdog is enabled by default. It observes the test-host
process and writes an Allure global error when that process exits unexpectedly.

In nested Microsoft Testing Platform runs, the watchdog may cause the
application to hang while process lifetimes are being observed. Disable it for
nested runs if this occurs. Set `isProcessWatchdogEnabled` to `false`, call
`DisableHostProcessWatchdog` during explicit registration, or add `--allure-watchdog off` to the arguments.

## Build a framework adapter

`Allure.TestingPlatform` provides the Microsoft Testing Platform registration,
message, correlation, and execution-state bridge needed by a framework adapter.
The adapter remains responsible for translating its framework's lifecycle and
metadata into Allure messages.

Reference `Allure.TestingPlatform` from the adapter project. Test projects
should install the completed framework-specific adapter.

### Register an embedded runtime

Call `AddEmbeddedAllure` while configuring the framework's Microsoft Testing
Platform test application:

```csharp
using Allure.TestingPlatform.Sdk;

var allure = builder.AddEmbeddedAllure(
    "My Framework",
    (context, _) =>
    {
        context.UseCorrelationContext(
            _ => new MyFrameworkCorrelationContext()
        );
        context.UseExecutionStateContext(
            _ => new MyFrameworkExecutionStateContext()
        );
    }
);
```

Use a stable, adapter-specific runtime name. It identifies both the runtime
registration and its in-process Allure endpoint that connects `AllureApi` calls with the runtime.

`AddEmbeddedAllure` returns a registration object that exposes `ConfigurationReference`, `RuntimeReference`,
and `MessageChannel`. The references and the channel are bound when Microsoft Testing Platform
starts the associated request. Check `MessageChannel.CanPublish` before using
the channel outside a framework callback whose request lifetime is known.

#### Limitations

Corrently, only a single registration is allowed per application. Consequently,
using multiple test framework adapters is not currently supported. We will
address that in the future.

Additionally, a registration supports one active MTP request at a time; parallel
requests are not supported. Sequential requests reuse the runtime, resolved
configuration, and constructed services from the first request. Keep configuration,
command-line options, and the results destination stable when reusing the same MTP
application process. Start a new application process when a later request needs
different Allure settings or a different results destination.

### Endpoint configuration

Every `AddEmbeddedAllure` overload shown in this README also has a form with a
final endpoint-registration callback. The callback receives the endpoint
context, the MTP service provider, and the constructed runtime. In the next example, the embedded endpoint suppresses the route whose ID is
`general-adapter`, so this adapter takes precedence when both routes match.

```csharp
var allure = builder.AddEmbeddedAllure(
    "My Framework",
    (context, _) =>
    {
        // ...
    },
    (endpoint, _, _) =>
    {
        // Prefer this endpoint over a more general adapter.
        endpoint.SuppressRoutes(_ => ["general-adapter"]);
    }
);
```

Use the exact route ID registered by the other integration.

`Allure.TestingPlatform` supplies the endpoint defaults needed for runtime API
routing. The callback can further configure route suppression, availability
and scope predicates, endpoint operations, parameter serialization, and
registration hooks. See the
[Allure.Net routing documentation](https://github.com/allure-framework/allure-csharp/blob/main/src/Allure.Net/README.md#creating-an-integration)
and the
[Allure.Net.Sdk endpoint documentation](https://github.com/allure-framework/allure-csharp/blob/main/src/Allure.Net.Sdk/README.md#expose-the-public-allure-api)
for more details.

### Connect correlation and execution state

Every Allure message has a `CorrelationUid` that associates it with an MTP test
session. Use the default session correlation when the framework exposes the
current MTP `SessionUid`; return the same value from the adapter's
`ICorrelationContext`.

If the framework does not expose `SessionUid`, call
`UseTestNodeMetadataCorrelation()` and add a `TestMetadataProperty` named
`Allure.TestingPlatform.CorrelationUid` to an early `TestNodeUpdateMessage` in
each session. Use that value in every Allure message for the session.

A custom
`ICorrelationStrategy` can be registered with `UseCorrelationStrategy` when
neither built-in strategy fits.

The adapter's `ExecutionStateContext` maps the framework's current scope,
fixture, test, and step to the matching Allure execution-state UID. Both
contexts must follow asynchronous callbacks and isolate concurrent executions.
Together, they route runtime API calls to the correct session and lifecycle
item.

### Map the framework lifecycle

MTP `TestNodeUpdateMessage` events drive the basic test lifecycle. Publish additional messages through the framework's `IMessageBus`, or through
the registration's `MessageChannel` when the framework does not expose the
message bus:

```csharp
await allure.MessageChannel.PublishAsync(
    this,
    new AllureTestUpdateMessage(
        correlationUid,
        new TestExecutionStateUid(testNodeUid)
    )
);
```

The `TestExecutionStateUid` value must match the corresponding `TestNode.Uid`.

Map framework events to the appropriate message group:

- scopes: `AllureScopeStartMessage`, `AllureScopeTestsMessage`, and `AllureScopeStopMessage`;
- fixtures: `AllureBeforeFixtureStartMessage`, `AllureAfterFixtureStartMessage`, `AllureFixtureUpdateMessage`, and `AllureFixtureStopMessage`;
- tests: `AllureTestUpdateMessage`;
- steps: `AllureStepStartMessage`, `AllureStepUpdateMessage`, and `AllureStepStopMessage`;
- the active test, fixture, or step: `AllureExecutableItemUpdateMessage`.

Use `ScopeExecutionStateUid`, `FixtureExecutionStateUid`,
`TestExecutionStateUid`, and `StepExecutionStateUid` for their respective
items. Start parents before children, stop children before parents, and keep
identifiers unique among concurrently active items of the same kind.

### Add metadata, status, and attachments

Add Allure data through the message's `Properties` collection. Each property
targets a specific Allure model:

| Message | Property target |
| --- | --- |
| `AllureTestUpdateMessage` | `TestResult` |
| Fixture start, update, and stop messages | `FixtureResult` |
| Step start, update, and stop messages | `StepResult` |
| `AllureExecutableItemUpdateMessage` | `ExecutableItem` |
| Scope messages | No `Properties` support |

Generic properties must use the message's exact target type; properties for a
non-matching model type are ignored.

| Data | Any executable item | Tests only |
| --- | --- | --- |
| Identity and hierarchy | `AllureNameProperty<TModel>` | `AllureFullNameProperty`, `AllureTitlePathProperty`, `AllureDefaultSuitesProperty`, `AllureTestMethodProperty` |
| Descriptions | `AllureDescriptionProperty<TModel>`, `AllureDescriptionHtmlProperty<TModel>` | — |
| Status and errors | `AllureStatusProperty<TModel>`, `AllureStatusDetailsProperty<TModel>`, `AllureExceptionProperty<TModel>` | — |
| Labels and links | — | `AllureLabelsProperty`, `AllureSetLabelProperty`, `AllureLinksProperty` |
| Parameters | `AllureParametersProperty<TModel>`, `AllureTestMethodArgumentsProperty<TModel>` | — |
| Timing | `AllureStartProperty<TModel>`, `AllureStopProperty<TModel>`, `AllureDurationProperty<TModel>` | — |
| Attachments | `AllureAttachmentProperty<TModel>`, `AllureAttachmentFileProperty<TModel>`, `AllureScreenDiffProperty<TModel>`, `AllureScreenDiffFileProperty<TModel>` | — |

For example:

```csharp
using Allure.Model;

new AllureTestUpdateMessage(correlationUid, testUid)
{
    Properties =
    [
        new AllureTestMethodProperty(testMethod)
        {
            Arguments = [.. arguments],
        },
        new AllureStatusProperty<TestResult>(Status.Passed),
        new AllureAttachmentFileProperty<TestResult>("log", logPath)
        {
            MediaType = "text/plain",
        },
    ],
};
```

Properties are applied in the same order they are provided to the message.

### Support runtime APIs and attributes

`AddEmbeddedAllure` registers an in-process endpoint for `AllureApi` and
`AllureInProcessApi`. The endpoint is available while the registration is
active, Allure is enabled, and its message channel can publish. Operations on
the current test or fixture additionally require the corresponding value from
`ExecutionStateContext`; global operations do not.

Direct in-process model reads and arbitrary callback-based model updates are
not supported yet. This includes the `AllureInProcessApi` `Read*`, `TryRead*`,
and `Update*` methods and the corresponding fixture and step context methods. MTP
lifecycle messages are processed asynchronously, so those synchronous APIs
cannot safely access the model owned by the data consumer. Use message-backed
`AllureApi` operations or publish Allure messages with properties instead.

Runtime routing uses `ICorrelationContext` to select the session and
`ExecutionStateContext` to select the active lifecycle item. Attribute support
is separate: obtain the reflected test method and publish
`AllureTestMethodProperty`, or translate the framework's metadata into the
corresponding properties.

### Customize the integration

Use the smallest extension point that meets the adapter's needs.

#### Custom configuration

Derive from `AllureTestingPlatformConfiguration` and register the derived type
with `AddEmbeddedAllure<TConfiguration>`:

```csharp
public sealed record MyFrameworkAllureConfiguration
    : AllureTestingPlatformConfiguration
{
    public bool CaptureFrameworkOutput { get; init; } = true;
}

var allure = builder.AddEmbeddedAllure<MyFrameworkAllureConfiguration>(
    "My Framework",
    (context, _) =>
    {
        context.UseCorrelationContext(
            _ => new MyFrameworkCorrelationContext()
        );
        context.UseExecutionStateContext(
            _ => new MyFrameworkExecutionStateContext()
        );
    }
);
```

The derived configuration is available from the registration's
`ConfigurationReference` after the MTP request has started.

#### Custom components

The registration callback can replace or configure individual components. For
example:

```csharp
context.UseDestination(configuration => new MyResultsDestination(configuration));
context.ConfigureSerialization(rules => rules.UseNullRepresentation("<null>"));
```

The available component APIs cover configuration sources and transforms,
results destinations, serialization, the Allure context and APIs, logging,
correlation, execution state, registration-hook discovery, enablement, and the
host-process watchdog. Prefer replacing an individual component when the
adapter does not need an additional runtime service.

#### Registration hooks

Registration hooks allow users of the adapter to configure Allure without
modifying the adapter's code.

With the default configuration and the non-generic `AddEmbeddedAllure`
overload, an application defines a hook by implementing
`IAllureTestingPlatformRegistrationHook`. Its `SetUp` method can configure
serialization, configuration sources and transformations, parameter
serialization, the results destination, enablement, and the process watchdog:

```csharp
public sealed class MyAllureRegistrationHook
    : IAllureTestingPlatformRegistrationHook
{
    public void SetUp(IAllureTestingPlatformRegistrationContext context)
    {
        context.ConfigureSerialization(
            rules => rules.UseNullRepresentation("<null>")
        );
    }
}
```

Adapters that use custom configuration or runtime services should define a branded hook interface, as described under **Custom
registration context**.

Users select the hook through the `ALLURE_RUNTIME_REGISTRATION_HOOK` environment
variable or the `runtimeRegistrationHook` configuration property. Specify its
assembly-qualified type name; the type must implement the matching hook
interface and have a public parameterless constructor.

Use `UseRegistrationHooks` when the adapter needs a different hook
discovery mechanism.

#### Custom runtime services

To add services to the runtime, define a runtime based on
`AllureTestingPlatformRuntime<TConfiguration>` (or implement
`IAllureTestingPlatformRuntime<TConfiguration>`) and a registration session
based on `AllureTestingPlatformRuntimeRegistrationSession<TConfiguration,
TRuntime>`:

```csharp
public sealed class FrameworkOutputCapture(bool enabled)
{
    public bool Enabled { get; } = enabled;
}

sealed class MyFrameworkAllureRuntime(
    RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
    AllureTestingPlatformRuntimeArguments platform,
    FrameworkOutputCapture? outputCapture = null
) : AllureTestingPlatformRuntime<MyFrameworkAllureConfiguration>(common, platform)
{
    public FrameworkOutputCapture OutputCapture { get; } =
        outputCapture ?? new(common.Configuration.CaptureFrameworkOutput);
}

sealed class MyFrameworkAllureRegistrationSession :
    AllureTestingPlatformRuntimeRegistrationSession<
        MyFrameworkAllureConfiguration,
        MyFrameworkAllureRuntime
    >
{
    protected override MyFrameworkAllureRuntime CreateRuntime(
        RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
        AllureTestingPlatformRuntimeArguments platform
    ) =>
        new(common, platform);
}
```

Register the session with the two-type overload of `AddEmbeddedAllure`:

```csharp
var allure = builder.AddEmbeddedAllure(
    "My Framework",
    () => new MyFrameworkAllureRegistrationSession(),
    (context, _) =>
    {
        context.UseCorrelationContext(
            _ => new MyFrameworkCorrelationContext()
        );
        context.UseExecutionStateContext(
            _ => new MyFrameworkExecutionStateContext()
        );
    }
);
```

#### Custom registration context

The registration context is the API available to application hooks. Define a
branded context when an adapter with custom configuration or runtime services
needs to expose framework-specific settings or operations that are not part of
the standard context:

```csharp
public interface IMyFrameworkAllureRegistrationContext :
    IAllureTestingPlatformRegistrationContext<MyFrameworkAllureConfiguration>
{
    void UseOutputCapture(
        Func<MyFrameworkAllureConfiguration, FrameworkOutputCapture> factory
    );
}

public interface IMyFrameworkAllureRegistrationHook :
    IAllureTestingPlatformRegistrationHook<
        IMyFrameworkAllureRegistrationContext
    >
{
}
```

Replace the two-type registration session from the previous section with one
that implements the branded context and its operations. Use the branded
interface as `TRegistrationContext` and
`IAllureTestingPlatformIntegrationContext<TConfiguration, TRuntime,
TRegistrationContext>` as `TIntegrationContext`:

```csharp
sealed class MyFrameworkAllureRegistrationSession :
    AllureTestingPlatformRuntimeRegistrationSession<
        MyFrameworkAllureConfiguration,
        MyFrameworkAllureRuntime,
        IMyFrameworkAllureRegistrationContext,
        IAllureTestingPlatformIntegrationContext<
            MyFrameworkAllureConfiguration,
            MyFrameworkAllureRuntime,
            IMyFrameworkAllureRegistrationContext
        >
    >,
    IMyFrameworkAllureRegistrationContext
{
    private Func<
        MyFrameworkAllureConfiguration,
        FrameworkOutputCapture
    > outputCaptureFactory = configuration =>
        new(configuration.CaptureFrameworkOutput);

    public void UseOutputCapture(
        Func<MyFrameworkAllureConfiguration, FrameworkOutputCapture> factory
    ) =>
        this.Modify(() => this.outputCaptureFactory = factory);

    protected override IMyFrameworkAllureRegistrationContext
        RegistrationContext => this;

    protected override IAllureTestingPlatformIntegrationContext<
        MyFrameworkAllureConfiguration,
        MyFrameworkAllureRuntime,
        IMyFrameworkAllureRegistrationContext
    > IntegrationContext => this;

    protected override MyFrameworkAllureRuntime CreateRuntime(
        RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
        AllureTestingPlatformRuntimeArguments platform
    ) =>
        new(
            common,
            platform,
            this.outputCaptureFactory(common.Configuration)
        );
}
```

Replace the preceding two-type registration call with the three-type
`AddEmbeddedAllure` overload. Its `TIntegrationContext` must be the same type
used by the session:

```csharp
var allure = builder.AddEmbeddedAllure(
    "My Framework",
    () => new MyFrameworkAllureRegistrationSession(),
    (context, _) =>
    {
        context.UseCorrelationContext(
            _ => new MyFrameworkCorrelationContext()
        );
        context.UseExecutionStateContext(
            _ => new MyFrameworkExecutionStateContext()
        );
    }
);
```

The `AddEmbeddedAllure` callback receives the integration context for adapter
setup. Application hooks receive the narrower branded registration context.

Application users implement the adapter's hook interface and call the exposed
operations in `SetUp`:

```csharp
public sealed class MyFrameworkAllureRegistrationHook :
    IMyFrameworkAllureRegistrationHook
{
    public void SetUp(IMyFrameworkAllureRegistrationContext context)
    {
        context.UseOutputCapture(
            _ => new FrameworkOutputCapture(enabled: false)
        );
    }
}
```

The hook is selected through the same environment variable or configuration
property as a standard hook.

### Adapter checklist

- Register one embedded runtime under a stable, adapter-specific name.
- Use the exact MTP test-node UID in Allure test updates.
- Choose a correlation strategy and provide a matching `ICorrelationContext`.
- Map the framework context through `ExecutionStateContext`, including parallel execution.
- Publish balanced lifecycle messages and preserve parent-child ordering.
- Apply properties to the correct Allure model before its lifecycle item stops.
- Expose runtime APIs only while the adapter owns an active execution.
- Use the default configuration and runtime unless the adapter needs additional settings or services.
- Expose registration hooks for user-facing customization.
- Test passing, failing, broken, skipped, parameterized, fixture, step, attachment, and parallel scenarios with `InMemoryResultsDestination`.

## Troubleshooting standalone mode

If standalone mode writes no results:

- make sure `--allure off` was not passed and `isEnabled` is not `false`;
- if explicit registration is used, make sure automatic registration is disabled and `AddAllure` is called once from the test application builder;
- if automatic registration is used, make sure `Allure_TestingPlatformEnableSelfRegistration` is not `false`;
- check `--allure-results-directory`, `resultsDirectory`, and the MTP results directory for the generated files.

If you see the following error:

```
Option '--allure' is declared by multiple providers: ...
```

- make sure Allure registered only once;
- if the test application calls `AddAllure` or `AddEmbeddedAllure`, make sure the `Allure_TestingPlatformEnableSelfRegistration` MSBuild property is set to `false`.

If standalone mode lacks framework metadata, attributes, or runtime API behavior, install a framework-specific adapter. Those capabilities cannot be inferred reliably from generic Microsoft Testing Platform events.
