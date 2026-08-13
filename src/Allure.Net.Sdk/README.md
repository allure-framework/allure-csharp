# Allure.Net.Sdk

[![NuGet release](https://img.shields.io/nuget/v/Allure.Net.Sdk?style=flat)](https://www.nuget.org/packages/Allure.Net.Sdk)
[![NuGet downloads](https://img.shields.io/nuget/dt/Allure.Net.Sdk?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Net.Sdk)

> Allure.Net.Sdk provides APIs and utilities for building Allure test-framework integrations.

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) – stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) – explore a live example of Allure Report in action

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
keep its registration alive for the lifetime of that instance. A builder is
single-use: call `Prepare` once to resolve configuration and run registration
hooks, then call `Build` once to construct the runtime and install its optional
endpoint. Access the runtime through `registration.Runtime`.

Dispose the registration when the integration shuts down. This removes the
endpoint and disposes the runtime. Use `DisposeAsync` for a runtime that needs
asynchronous disposal.

### Use the standard runtime and configuration

If the integration does not add configuration properties or runtime services,
use the non-generic `AllureRuntimeBuilder`:

```csharp
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

var builder = new AllureRuntimeBuilder("My Test Framework");
var plan = builder.Prepare(_ => { });
using var registration = plan.Build();
IAllureRuntime runtime = registration.Runtime;
```

The registration action passed to `Prepare` receives an
`IAllureRuntimeIntegrationContext`. Use that context to:

- select configuration sources;
- configure the results destination and parameter serialization;
- replace the execution context, lifecycle API, or model API;
- configure runtime hooks;
- register and configure an in-process endpoint that connects the public Allure
  API with the runtime.

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

By default, the builder looks for hooks in two places: first in the
`ALLURE_RUNTIME_REGISTRATION_HOOK` environment variable, then in the resolved
configuration's `runtimeRegistrationHook` property. For example, a user can
select a hook in `allureConfig.json`:

```json
{
  "runtimeRegistrationHook": "MyTests.ProjectAllureRegistration, MyTests"
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

var plan = builder.Prepare(context =>
{
    context.UseRegistrationHooks(configuration => [
        ReflectionHooks.FromConfiguration<
            AllureConfiguration,
            IAllureRuntimeRegistrationHook
        >(configuration),
        MyFrameworkRegistration.CurrentHook,
    ]);
});

using var registration = plan.Build();
IAllureRuntime runtime = registration.Runtime;
```

Unset hooks are ignored.

To disable registration hooks, return an empty collection:

```csharp
var plan = builder.Prepare(context =>
    context.UseRegistrationHooks(_ => [])
);
```

### Add integration-specific configuration

Derive from `AllureConfiguration` when the integration needs additional
settings but can otherwise use the standard runtime:

```csharp
using Allure.Sdk.Configuration;

public sealed record class MyFrameworkConfiguration : AllureConfiguration
{
    public bool CaptureFrameworkOutput { get; init; } = true;
}
```

Then pass the configuration type to `AllureRuntimeBuilder<TConfiguration>`:

```csharp
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

var builder = new AllureRuntimeBuilder<MyFrameworkConfiguration>(
    "My Test Framework"
);
var plan = builder.Prepare(context =>
{
    // Configure sources, serialization, hooks, or an endpoint here.
});
using var registration = plan.Build();

IAllureRuntime<MyFrameworkConfiguration> runtime = registration.Runtime;
```

Expose a framework-specific hook interface so users do not need to reference
the SDK's generic hook contract directly:

```csharp
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;

public interface IMyFrameworkRuntimeRegistrationHook :
    IAllureRegistrationHook<
        IAllureRuntimeRegistrationContext<MyFrameworkConfiguration>
    >;
```

Users can then customize the runtime by implementing that interface:

```csharp
public sealed class ProjectAllureRegistration :
    IMyFrameworkRuntimeRegistrationHook
{
    public void SetUp(
        IAllureRuntimeRegistrationContext<MyFrameworkConfiguration> context
    )
    {
        context.ConfigureSerialization(
            rules => rules.UseNullRepresentation("<null>")
        );
    }
}
```

The branded interface becomes part of the integration's public API. Because it
derives from the generic SDK contract, it remains compatible with the standard
registration pipeline. As in the standard configuration example, use
`UseRegistrationHooks` to customize where hooks are loaded from.

### Add integration-specific runtime services

If the integration also exposes additional runtime services, derive a runtime
from `AllureRuntime<TConfiguration>` and define a registration session to
construct it:

```csharp
using Allure.Abstractions;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

// An extra runtime service.
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

sealed class MyFrameworkRuntimeRegistrationSession :
    AllureRuntimeRegistrationSession<MyFrameworkConfiguration, MyFrameworkRuntime>
{
    protected override MyFrameworkRuntime CreateRuntime(
        RuntimeCreationArguments<MyFrameworkConfiguration> args
    ) => new(
        args.Configuration,
        args.ParameterSerializer,
        args.Destination,
        args.Context,
        args.LifecycleApi,
        args.ModelApi,
        new FrameworkOutputCapture(
            args.Configuration.CaptureFrameworkOutput
        )
    );
}
```

Pass a factory for that session to the two-parameter builder:

```csharp
var builder = new AllureRuntimeBuilder<
    MyFrameworkConfiguration,
    MyFrameworkRuntime
>(
    "My Test Framework",
    () => new MyFrameworkRuntimeRegistrationSession()
);
var plan = builder.Prepare(_ => { });
using var registration = plan.Build();

MyFrameworkRuntime runtime = registration.Runtime;

if (runtime.OutputCapture.Enabled)
{
    // ...
}
```

Framework users can continue to customize standard runtime components through
the branded hook interface from the custom configuration example.

#### Let users customize runtime services

The hook interface in the custom configuration example exposes only the
standard runtime registration operations. To let users customize
integration-specific runtime services, replace it with a branded registration
context and hook pair, then expose the service factory through the context:

```csharp
using System;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;

public interface IMyFrameworkRuntimeRegistrationContext :
    IAllureRuntimeRegistrationContext<MyFrameworkConfiguration>
{
    void UseOutputCapture(
        Func<MyFrameworkConfiguration, FrameworkOutputCapture> factory
    );
}

public interface IMyFrameworkRuntimeRegistrationHook :
    IAllureRegistrationHook<IMyFrameworkRuntimeRegistrationContext>;
```

Implement the context on a session derived from the three-parameter session
class:

```csharp
sealed class MyFrameworkRuntimeRegistrationSession :
    AllureRuntimeRegistrationSession<
        MyFrameworkConfiguration,
        MyFrameworkRuntime,
        IMyFrameworkRuntimeRegistrationContext
    >,
    IMyFrameworkRuntimeRegistrationContext
{
    Func<MyFrameworkConfiguration, FrameworkOutputCapture>
        outputCaptureFactory =
            configuration => new(
                configuration.CaptureFrameworkOutput
            );

    public void UseOutputCapture(
        Func<MyFrameworkConfiguration, FrameworkOutputCapture> factory
    ) =>
        this.Modify(() => this.outputCaptureFactory = factory);

    protected override IMyFrameworkRuntimeRegistrationContext
        RegistrationContext => this;

    protected override MyFrameworkRuntime CreateRuntime(
        RuntimeCreationArguments<MyFrameworkConfiguration> args
    ) => new(
        args.Configuration,
        args.ParameterSerializer,
        args.Destination,
        args.Context,
        args.LifecycleApi,
        args.ModelApi,
        this.outputCaptureFactory(args.Configuration)
    );
}
```

`Modify` ensures that the factory can change only while registration is active.
In this design, users change it through registration hooks.

Pass the corresponding three-parameter `IAllureRuntimeIntegrationContext` and
the session factory to the builder:

```csharp
var builder = new AllureRuntimeBuilder<
    MyFrameworkConfiguration,
    MyFrameworkRuntime,
    IAllureRuntimeIntegrationContext<
        MyFrameworkConfiguration,
        MyFrameworkRuntime,
        IMyFrameworkRuntimeRegistrationContext
    >
>(
    "My Test Framework",
    () => new MyFrameworkRuntimeRegistrationSession()
);
```

The action passed to `Prepare` still receives the standard
`IAllureRuntimeIntegrationContext` shown above. The branded registration
context is passed to registration hooks, where users can configure both
standard and integration-specific services.

Framework users can now customize both standard runtime components and the
integration-specific service through the branded hook API:

```csharp
public sealed class ProjectAllureRegistration :
    IMyFrameworkRuntimeRegistrationHook
{
    public void SetUp(
        IMyFrameworkRuntimeRegistrationContext context
    )
    {
        context.UseOutputCapture(
            _ => new FrameworkOutputCapture(enabled: false)
        );
    }
}
```

#### Build a reusable integration platform

A reusable platform may need to expose additional customization operations to
framework adapters through `Prepare` while retaining a smaller user-facing API
for registration hooks. In that case, define a branded integration context
that extends the branded registration context:

```csharp
using System;

public interface IMyFrameworkRuntimeIntegrationContext :
    IAllureRuntimeIntegrationContext<
        MyFrameworkConfiguration,
        MyFrameworkRuntime,
        IMyFrameworkRuntimeRegistrationContext
    >,
    IMyFrameworkRuntimeRegistrationContext
{
    void UseLogger(Action<string> logger);
}
```

Implement the integration context on a session derived from the four-parameter
session class:

```csharp
sealed class MyFrameworkRuntimeRegistrationSession :
    AllureRuntimeRegistrationSession<
        MyFrameworkConfiguration,
        MyFrameworkRuntime,
        IMyFrameworkRuntimeRegistrationContext,
        IMyFrameworkRuntimeIntegrationContext
    >,
    IMyFrameworkRuntimeIntegrationContext
{
    Func<MyFrameworkConfiguration, FrameworkOutputCapture>
        outputCaptureFactory =
            configuration => new(
                configuration.CaptureFrameworkOutput
            );

    Action<string> logger = _ => { };

    public void UseOutputCapture(
        Func<MyFrameworkConfiguration, FrameworkOutputCapture> factory
    ) =>
        this.Modify(() => this.outputCaptureFactory = factory);

    public void UseLogger(Action<string> logger) =>
        this.Modify(() => this.logger = logger);

    protected override IMyFrameworkRuntimeRegistrationContext
        RegistrationContext => this;

    protected override IMyFrameworkRuntimeIntegrationContext
        IntegrationContext => this;

    protected override MyFrameworkRuntime CreateRuntime(
        RuntimeCreationArguments<MyFrameworkConfiguration> args
    )
    {
        this.logger("Creating the Allure runtime.");

        return new(
            args.Configuration,
            args.ParameterSerializer,
            args.Destination,
            args.Context,
            args.LifecycleApi,
            args.ModelApi,
            this.outputCaptureFactory(args.Configuration)
        );
    }
}
```

Expose the branded integration context through the builder so each framework
adapter can configure the shared platform during `Prepare`:

```csharp
var builder = new AllureRuntimeBuilder<
    MyFrameworkConfiguration,
    MyFrameworkRuntime,
    IMyFrameworkRuntimeIntegrationContext
>(
    "My Test Framework",
    () => new MyFrameworkRuntimeRegistrationSession()
);

var plan = builder.Prepare(context =>
{
    context.UseLogger(
        message => Console.WriteLine($"[My Framework] {message}")
    );
});
```

Framework adapters receive `IMyFrameworkRuntimeIntegrationContext` through
`Prepare`. Because that interface extends
`IMyFrameworkRuntimeRegistrationContext`, adapters receive all user-facing
registration operations as well as adapter-specific operations such as
`UseLogger`. Application users receive only
`IMyFrameworkRuntimeRegistrationContext` through registration hooks.

Implementation checklist:

- Define a custom configuration derived from `AllureConfiguration`.
- Define the custom runtime derived from `AllureRuntime<TConfiguration>` and
  expose its integration-specific services.
- If users need to customize only standard runtime components, derive
  `AllureRuntimeRegistrationSession<TConfiguration, TRuntime>` and use
  `AllureRuntimeBuilder<TConfiguration, TRuntime>`.
- To expose integration-specific service registration, define a branded
  registration context and implement it through
  `AllureRuntimeRegistrationSession<TConfiguration, TRuntime, TRegistrationContext>`.
  Use `IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>`
  as the third type argument of `AllureRuntimeBuilder`.
- For a reusable integration platform, define a branded integration context and
  derive it from the branded registration context. Implement the integration
  context through the four-parameter session and expose it as the third type
  argument of `AllureRuntimeBuilder`.
- Call `Prepare` and `Build` once, then retain the registration.

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

runtime.LifecycleApi.StartTestScope(scope);

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

    var completedScope = runtime.LifecycleApi.StopTestScope();
    runtime.ResultsDestination.WriteContainer(completedScope);
}
```

Use `ScheduleTest` before `StartTest` when the framework reports discovery and
execution as separate events. Setup and teardown fixtures require an active
scope. A step requires an active fixture, test, or parent step. Stop nested
items in reverse order.

`ModelApi` updates or reads the active scope, fixture, test, step, or current
executable item. It supports concurrent access, so prefer it over retaining
mutable model references across unrelated callbacks.

## Preserve state across framework callbacks

The runtime uses `IAllureExecutionContext` to identify the active scope,
fixture, test, and step. The default `AsyncLocalExecutionContext` propagates
state through normal synchronous and asynchronous call flows. A framework may,
however, deliver related events on a disconnected execution flow where the
original `AsyncLocal` value is no longer available. In such a case, choose a
state storage strategy that follows the framework's notion of the current
execution.

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
    IReadOnlyLateBoundReference<IAllureRuntimeBase> runtimeReference
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

Register the adapter while preparing the runtime. The builder's late-bound
runtime reference becomes available after the plan is built:

```csharp
var plan = builder.Prepare(context =>
{
    context.UseContext(_ =>
        new FrameworkAllureExecutionContext(builder.RuntimeReference)
    );
});

using var registration = plan.Build();
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
    try
    {
        await SendRequestAsync();
    }
    finally
    {
        activeRuntime.LifecycleApi.StopStep();
    }
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

var plan = builder.Prepare(context =>
{
    context.RegisterInProcessEndpoint(
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
});

using var registration = plan.Build();
var runtime = registration.Runtime;
```

Keep the registration alive while the integration is active. Dispose it when
the integration shuts down so the endpoint is removed. Use `DisposeAsync` when
the custom runtime requires asynchronous disposal.

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

### Out-of-process endpoints

`Allure.Net.Sdk` currently provides registration support only for in-process
endpoints. Integrations that communicate with the runtime across a process
boundary must implement `Allure.Abstractions.IAllureRuntimeEndpoint` and
`Allure.Abstractions.IAllureRuntimeRoute`, then install the route with
`Allure.Runtime.AllureRuntimeRouter.Install`.

The integration is responsible for the transport, request routing,
serialization, availability checks, and resource cleanup. Keep the
`IDisposable` returned by `Install` and dispose it when the integration shuts
down to remove the route.

## Customize runtime components

The integration context passed to `Prepare` lets integrations replace
individual runtime components:

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
- Dispose the runtime registration when the integration shuts down.
- Test the adapter with `InMemoryResultsDestination`, including failures,
  skipped tests, fixtures, nested steps, attachments, and parallel execution.
