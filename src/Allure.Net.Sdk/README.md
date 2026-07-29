# Allure.Net.Sdk

[![Nuget release](https://img.shields.io/nuget/v/Allure.Net.Sdk?style=flat)](https://www.nuget.org/packages/Allure.Net.Sdk)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.Net.Sdk?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Net.Sdk)


> Allure.Net.Sdk provides APIs and utilities for building Allure test-framework integrations.

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

## Purpose

`Allure.Net.Sdk` is the foundation for authors of Allure adapters and test
framework integrations. It provides:

- a configurable Allure runtime;
- lifecycle and model APIs for building test results and fixture containers;
- execution-state propagation for concurrent and asynchronous test runs;
- an optional in-process endpoint that routes Allure API calls to the runtime;
- result destinations, configuration sources, parameter serialization, and
  common model-building functions.

Application test authors normally depend on `Allure.Net`, or on a framework
adapter built with this package, rather than using the SDK directly.

## Build a runtime

Create one runtime for each independently configured integration instance and
keep it alive for the lifetime of that instance. Choose the simplest builder
that provides the extension points your integration needs.

### Use the standard runtime and configuration

If the integration does not add configuration properties or runtime services,
use the non-generic builder:

```csharp
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

var builder = new AllureRuntimeBuilder("My Test Framework");
IAllureRuntime<AllureConfiguration> runtime = builder.Build();
```

Before calling `Build`, the integration can use the builder to:

- select configuration sources;
- configure the results destination and parameter serialization;
- replace the execution context, lifecycle API, or model API;
- register runtime and endpoint hooks;
- register an in-process endpoint that connects the Allure API with the runtime.

#### Let framework users customize registration

Framework users can customize runtime registration by implementing
`IAllureRuntimeRegistrationHook`. For example:

```csharp
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;

public sealed class ProjectAllureRegistration :
    IAllureRuntimeRegistrationHook
{
    public void SetUp(IAllureRuntimeRegistrationContext context)
    {
        context.ConfigureSerialization(
            rules => rules.UseNullRepresentation("<null>")
        );
    }
}
```

By default, the builder first loads the hook named by the
`ALLURE_RUNTIME_REGISTRATION_HOOK` environment variable, then the hook named
by the resolved configuration's `registrationHook` property. Unset hooks are
ignored. For example, a user can select a hook in `allureConfig.json`:

```json
{
  "registrationHook": "MyTests.ProjectAllureRegistration, MyTests"
}
```

A hook loaded by name must implement the expected hook interface and have a
public parameterless constructor.

Runtime hooks run after the initial configuration is resolved. If a hook
replaces the configuration sources, the builder resolves configuration again
before constructing the runtime.

An integration can replace the default hook provider with
`UseRegistrationHooks`, for example to obtain hooks from its own extension
system:

```csharp
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

public static class MyFrameworkRegistration
{
    public static IAllureRuntimeRegistrationHook? CurrentHook { get; set; }
}

var builder = new AllureRuntimeBuilder("My Test Framework");

builder.UseRegistrationHooks(configuration => [
    ReflectionHooks.FromConfiguration<
        AllureConfiguration,
        IAllureRuntimeRegistrationHook
    >(configuration),
    MyFrameworkRegistration.CurrentHook,
]);

IAllureRuntime<AllureConfiguration> runtime = builder.Build();
```

To disable registration hooks, return an empty collection:

```csharp
builder.UseRegistrationHooks(_ => []);
```

### Add integration-specific configuration

Derive from `AllureConfiguration` when the integration needs additional
settings. The integration should provide its configuration type, runtime and
endpoint registration contexts, hook interfaces, a route builder, and a
runtime builder:

```csharp
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;

public sealed record class MyFrameworkConfiguration : AllureConfiguration
{
    public bool CaptureFrameworkOutput { get; init; } = true;
}

public interface IMyFrameworkRuntimeRegistrationContext :
    IAllureRuntimeRegistrationContext<MyFrameworkConfiguration>;

public interface IMyFrameworkRuntimeRegistrationHook :
    IAllureRuntimeRegistrationHook<
        MyFrameworkConfiguration,
        IMyFrameworkRuntimeRegistrationContext
    >;

public interface IMyFrameworkEndpointRegistrationContext :
    IAllureInProcessEndpointRegistrationContext<MyFrameworkConfiguration>;

public interface IMyFrameworkEndpointRegistrationHook :
    IAllureInProcessEndpointRegistrationHook<
        MyFrameworkConfiguration,
        IMyFrameworkEndpointRegistrationContext
    >;

sealed class MyFrameworkRouteBuilder(
    AllureRouteBuilderArgs<MyFrameworkConfiguration> args
) :
    AllureInProcessRouteBuilder<
        MyFrameworkConfiguration,
        IMyFrameworkEndpointRegistrationContext,
        IMyFrameworkEndpointRegistrationHook
    >(args),
    IMyFrameworkEndpointRegistrationContext
{
    protected override IMyFrameworkEndpointRegistrationContext
        RegistrationContext => this;
}

public sealed class MyFrameworkRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        MyFrameworkConfiguration,
        IMyFrameworkRuntimeRegistrationContext,
        IMyFrameworkRuntimeRegistrationHook,
        IMyFrameworkEndpointRegistrationContext,
        IMyFrameworkEndpointRegistrationHook
    >(runtimeName),
    IMyFrameworkRuntimeRegistrationContext
{
    protected override IMyFrameworkRuntimeRegistrationContext
        RegistrationContext => this;

    protected override AllureInProcessRouteBuilder<
        MyFrameworkConfiguration,
        IMyFrameworkEndpointRegistrationContext,
        IMyFrameworkEndpointRegistrationHook
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<MyFrameworkConfiguration> args
    ) => new MyFrameworkRouteBuilder(args);
}
```

The integration constructs the standard runtime with this builder:

```csharp
IAllureRuntime<MyFrameworkConfiguration> runtime =
    new MyFrameworkRuntimeBuilder("My Test Framework").Build();
```

Framework users customize registration by implementing the hook interface
provided by the integration:

```csharp
public sealed class ProjectAllureRegistration :
    IMyFrameworkRuntimeRegistrationHook
{
    public void SetUp(
        IMyFrameworkRuntimeRegistrationContext context
    )
    {
        context.ConfigureSerialization(
            rules => rules.UseNullRepresentation("<null>")
        );
    }
}
```

### Add integration-specific runtime services

If the integration also exposes additional runtime services, derive a
runtime from `AllureRuntime<TConfiguration>` and a builder from the
six-parameter `AllureRuntimeBuilder`. Reuse the configuration, context, hook,
and route-builder types from the previous example, and replace the
five-parameter builder with:

```csharp
using Allure.Abstractions;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

// An extra service to add.
public sealed class FrameworkOutputCapture(bool enabled)
{
    public bool Enabled { get; } = enabled;
}

public sealed class MyFrameworkRuntime(
    MyFrameworkConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    IAllureResultsDestination resultsDestination,
    IAllureExecutionContext context,
    IAllureLifecycleApi lifecycleApi,
    IAllureModelApi modelApi,
    FrameworkOutputCapture outputCapture
) : AllureRuntime<MyFrameworkConfiguration>(
    configuration,
    parameterSerializer,
    resultsDestination,
    context,
    lifecycleApi,
    modelApi
)
{
    public FrameworkOutputCapture OutputCapture { get; } = outputCapture;
}

public sealed class MyFrameworkRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        MyFrameworkConfiguration,
        IMyFrameworkRuntimeRegistrationContext,
        IMyFrameworkRuntimeRegistrationHook,
        IMyFrameworkEndpointRegistrationContext,
        IMyFrameworkEndpointRegistrationHook,
        MyFrameworkRuntime
    >(runtimeName),
    IMyFrameworkRuntimeRegistrationContext
{
    protected override IMyFrameworkRuntimeRegistrationContext
        RegistrationContext => this;

    protected override AllureInProcessRouteBuilder<
        MyFrameworkConfiguration,
        IMyFrameworkEndpointRegistrationContext,
        IMyFrameworkEndpointRegistrationHook
    > CreateRouteBuilder(
        AllureRouteBuilderArgs<MyFrameworkConfiguration> args
    ) => new MyFrameworkRouteBuilder(args);

    protected override MyFrameworkRuntime CreateRuntimeInstance(
        MyFrameworkConfiguration configuration,
        IAllureParameterSerializer parameterSerializer,
        IAllureResultsDestination destination,
        IAllureExecutionContext context,
        IAllureLifecycleApi lifecycleApi,
        IAllureModelApi modelApi
    ) => new(
        configuration,
        parameterSerializer,
        destination,
        context,
        lifecycleApi,
        modelApi,
        new FrameworkOutputCapture(configuration.CaptureFrameworkOutput)
    );
}
```

The custom builder returns the concrete runtime type:

```csharp
MyFrameworkRuntime runtime =
    new MyFrameworkRuntimeBuilder("My Test Framework").Build();

if (runtime.OutputCapture.Enabled)
{
    // ...
}
```

## Map the framework lifecycle

Drive the runtime from framework events. A typical execution is:

1. Start a `TestResultScope` for a framework container or fixture scope.
2. Start and stop setup fixtures.
3. Schedule or start each `TestResult`.
4. Start and stop steps while the test is active.
5. Stop the test and write the returned result.
6. Start and stop teardown fixtures.
7. Stop the scope and write the returned container.

The lifecycle API maintains parent-child relationships, nesting, stages, and
timestamps. It does **not** write stopped tests or scopes to the destination.
The integration owns that boundary:

```csharp
using System;
using Allure.Model;
using Allure.Sdk.Functions;

var scope = new TestResultScope
{
    Uuid = Ids.NewUuid(),
    Name = "CheckoutTests",
};

runtime.LifecycleApi.StartScope(scope);

var test = new TestResult
{
    Uuid = Ids.NewUuid(),
    Name = "creates an order",
    FullName = "Example.CheckoutTests.CreatesOrder",
};

test.TestCaseId = Ids.ForTestCase(test.FullName);
runtime.LifecycleApi.StartTest(test);

try
{
    // Invoke the framework test here.
    runtime.ModelApi.UpdateTestResult(result => result.Status = Status.Passed);
}
catch (Exception exception)
{
    runtime.ModelApi.UpdateTestResult(result =>
    {
        result.Status = ErrorStatus.Resolve(
            runtime.Configuration.FailExceptions,
            exception
        );
        result.StatusDetails = StatusDetails.FromException(exception);
    });
    throw;
}
finally
{
    var completedTest = runtime.LifecycleApi.StopTest();
    runtime.ResultsDestination.WriteTestResult(completedTest);

    var completedScope = runtime.LifecycleApi.StopScope();
    runtime.ResultsDestination.WriteContainer(completedScope);
}
```

Use `ScheduleTest` before `StartTest` when the
framework reports discovery and execution as separate events. Setup and teardown
fixtures require an active scope. A step requires an active fixture, test, or
parent step. Stop nested items in reverse order.

`ModelApi` updates or reads the active scope, fixture, test, step, or current
executable item. It supports concurrent access, so prefer it over retaining
mutable model references across unrelated callbacks.

## Preserve state across framework callbacks

The runtime uses `IAllureExecutionContext` to identify the active scope,
fixture, test, and step. The default `AsyncLocalExecutionContext` follows
normal synchronous and asynchronous calls. A framework may, however, deliver
related events on a disconnected execution flow where the original `AsyncLocal`
value is no longer available. In such a case, choose a state storage strategy
that follows the framework's notion of the current execution.

### Use the framework's execution context

If the framework manages a context object for the current test or fixture
and that object is available from both framework callbacks and user test code,
store the `AllureExecutionState` on it and provide an `IAllureExecutionContext`
adapter.

The following example assumes that `FrameworkContext.Current` returns an
object dedicated to the current framework execution and that the integration
can reserve an `"AllureState"` key on it:

```csharp
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

public sealed class FrameworkAllureExecutionContext(
    IReadOnlyLateBoundReference<IAllureRuntime> runtimeReference
) : AllureExecutionContext(runtimeReference)
{
    public override AllureExecutionState CurrentState
    {
        get =>
            FrameworkContext.Current["AllureState"] is AllureExecutionState state
                ? state
                : new();

        protected set => FrameworkContext.Current["AllureState"] = value;
    }
}
```

Register the adapter before building the runtime:

```csharp
builder.UseContext(dependencies =>
    new FrameworkAllureExecutionContext(dependencies.RuntimeReference)
);
```

The framework context must isolate parallel executions, remain available for
the whole test or fixture, and be the same object observed by event handlers
and user code.

### Capture and restore state explicitly

When the framework does not provide a suitable ambient context,
capture the state while it is available, associate it with the framework's
execution ID, and restore it around each later callback:

```csharp
using Allure.Model;
using Allure.Sdk.Runtime;

AllureExecutionState state = runtime.ContextApi.CurrentState;

await runtime.ContextApi.RunWithStateAsync(state, async activeRuntime =>
{
    activeRuntime.LifecycleApi.StartStep(new StepResult { Name = "HTTP request" });
    await SendRequestAsync();
    activeRuntime.LifecycleApi.StopStep();
});
```

The previous caller state is restored even if the callback throws.
`RunWithState` returns the state produced by a synchronous callback. For an
asynchronous callback whose updated state must be retained, use
`GetWithStateAsync` and return `activeRuntime.ContextApi.CurrentState`
explicitly.

Store a separate snapshot for each parallel framework execution and restore
the matching snapshot for each event.

## Expose the public Allure API

Register an in-process endpoint when test code should be able to call
`AllureApi`, use Allure attributes, or access `AllureInProcessApi`:

```csharp
var isIntegrationActive = true;

builder.RegisterInProcessEndpoint(
    "my-framework",
    (_, endpoint) =>
    {
        endpoint.SetAvailabilityPredicate(() => isIntegrationActive);
        endpoint.UseCurrentScopePredicate(
            _ => FrameworkContext.Current.IsActive
        );
        endpoint.UseGlobalScopePredicate(
            _ => isIntegrationActive
        );
    }
);

var runtime = builder.Build();
```

The current-scope predicate should return `true` only while the integration owns
the active test or fixture. The global-scope predicate should return `true` only
while the endpoint should receive global attachments and errors. Broad
predicates can make two installed integrations match the same call, which
causes route resolution to fail.

Endpoint IDs must be stable and unique. If a more specific adapter can run
alongside a general adapter, call `SuppressRoutes` with the general adapter's
route ID. The endpoint uses the runtime's standard operations and serializer by
default; `UseOperations`, endpoint-level `ConfigureSerialization`, and
`UseParameterSerializer` are available for specialized bridges.

## Customize runtime components

The builder lets integrations replace individual runtime components:

- `UseDestination` for a custom `IAllureResultsDestination`. The package also
  includes `InMemoryResultsDestination` for tests and
  `NullResultsDestination` for disabled output.
- `ConfigureSerialization` to add, remove, replace, or reprioritize
  `IParameterSerializationRule` instances. Rules added later take precedence.
- `UseParameterSerializer` to replace rule-based serialization completely.
- `UseContext`, `UseLifecycleApi`, and `UseModelApi` for integrations that need
  a different state transport or lifecycle implementation.
- `UseRegistrationHooks` to replace the default hook provider. See
  [Let framework users customize registration](#let-framework-users-customize-registration).

## Reuse the model functions

`Allure.Sdk.Functions` contains framework-neutral helpers commonly needed by
adapters:

- `Ids` creates UUIDs, test case IDs, and parameter-sensitive history IDs.
- `Parameters` converts reflected arguments and `AllureParameterAttribute`
  metadata into Allure parameters.
- `Titles` derives title paths from test types and methods.
- `SuiteLabels` and `GlobalLabels` apply conventional hierarchy and global
  labels.
- `LinkTemplates` expands configured issue, TMS, and custom links.
- `ErrorStatus` classifies configured assertion exceptions as `Failed` and
  other exceptions as `Broken`.
- `StatusDetails.FromException` creates failure diagnostics.
- `AttachmentSource` creates collision-resistant attachment file names.

Apply identity fields, parameters, labels, and links before writing the final
test result. In particular, calculate `HistoryId` after all non-excluded test
parameters are known.

## Integration checklist

- Use stable full names and test case IDs; include non-excluded parameters in
  history IDs.
- Keep framework correlation IDs separate from generated Allure UUIDs.
- Balance every lifecycle start with the matching stop in `finally` paths.
- Set a final status and status details before stopping an executable item.
- Write stopped tests and scopes exactly once.
- Generate an attachment file name, store it in the model attachment, and copy
  or write the content under that name through `ResultsDestination`.
- Keep Allure execution state isolated for each parallel framework execution.
- Keep endpoint matching narrow and define suppression when adapters overlap.
- Test the adapter with `InMemoryResultsDestination`, including failures,
  skipped tests, fixtures, nested steps, attachments, and parallel execution.
