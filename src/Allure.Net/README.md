# Allure.Net

[![Nuget release](https://img.shields.io/nuget/v/Allure.Net?style=flat)](https://www.nuget.org/packages/Allure.Net)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.Net?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.Net)


> Allure.Net is a library that provides APIs and integration infrastructure for reporting .NET test execution to Allure.

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

## Overview

Allure.Net provides APIs and integration infrastructure for reporting .NET test execution to Allure.

The package has two main audiences:

- Test projects and libraries use `AllureApi` and the Allure attributes to add steps, labels, links, parameters, attachments, and other information to a report.
- Test framework integrations implement an Allure runtime endpoint and register a route that connects API calls to the correct test execution.

Allure.Net supports multiple simultaneous executions, both in-process and out-of-process.

## Using the API

The static `AllureApi` facade provides synchronous and asynchronous operations. The corresponding methods share the same facade; asynchronous method names use the `Async` suffix.

```csharp
using Allure;
using Allure.Model;

public async Task CreateOrder()
{
    AllureApi.SetOwner("Payments team");
    AllureApi.SetSeverity(Severity.Critical);
    AllureApi.AddTags("checkout");
    AllureApi.AddIssue("https://issues.example/PAY-123", "PAY-123");
    AllureApi.AddTestParameter("currency", "USD");

    var orderId = await AllureApi.StepAsync(
        "Create the order",
        async () => await CreateOrderAsync()
    );

    AllureApi.AddAttachment(
        "Order response",
        $"{{\"orderId\":\"{orderId}\"}}",
        "application/json",
        ".json"
    );
}
```

Steps may receive a context for changing the step result at runtime:

```csharp
var response = await AllureApi.StepAsync(
    "Send request",
    async context =>
    {
        await context.AddParameterAsync("attempt", "1");
        var result = await SendRequestAsync();
        await context.SetNameAsync($"Send request: {result.StatusCode}");
        return result;
    }
);
```

Attributes provide the same reporting capabilities for method-based instrumentation:

```csharp
public sealed class CheckoutClient
{
    [AllureStep("Create order for {customerId}")]
    public async Task<string> CreateOrderAsync(string customerId)
    {
        // The call is represented as an Allure step.
        return await SendCreateOrderRequestAsync(customerId);
    }

    [AllureAttachment("Response for {orderId}", ContentType = "application/json")]
    public string GetResponseBody(string orderId) => LoadResponse(orderId);
}
```

### Accessing the in-process model

Use `AllureInProcessApi` when a change cannot be expressed by the portable API:

```csharp
AllureInProcessApi.UpdateTestResult(result =>
{
    result.Description = "Description computed during the test";
});

var testName = AllureInProcessApi.ReadTestResult(
    result => result.Name,
    fallback: "No active test"
);
```

These operations throw `InvalidOperationException` when the selected endpoint does not support in-process access.

## Creating an integration

An integration exposes its runtime through `IAllureRuntimeEndpoint` and describes when that endpoint should receive calls with `IAllureRuntimeRoute`. `MatchesCurrentScope` should use framework-owned execution state, typically an `AsyncLocal`-backed test context, to identify calls belonging to the integration. `MatchesGlobalScope` independently controls whether the route accepts global data such as global attachments and errors.

The following example shows the routing portion of an in-process integration. `ExampleEndpoint` supplies the integration's implementations of the synchronous and asynchronous operation interfaces.

```csharp
using System.Collections.Immutable;
using Allure.Abstractions;
using Allure.Runtime;

public sealed class ExampleEndpoint : IAllureInProcessRuntimeEndpoint
{
    public string Name => "Example Framework";

    public bool IsAvailable => true;

    public required IAllureApiOperations Operations { get; init; }

    public required IAllureInProcessApiOperations InProcessOperations { get; init; }

    public required IAllureParameterSerializer ParameterSerializer { get; init; }
}

public sealed class ExampleRoute(ExampleEndpoint endpoint) : IAllureRuntimeRoute
{
    public string Id => "example-framework";

    public bool MatchesCurrentScope =>
        ExampleTestContext.Current?.TestId is not null;

    public bool MatchesGlobalScope => true;

    public ImmutableHashSet<string> SuppressedRouteIds { get; } =
        ImmutableHashSet<string>.Empty;

    public IAllureRuntimeEndpoint Endpoint => endpoint;
}

public sealed class ExampleIntegration : IDisposable
{
    private readonly IDisposable registration;

    public ExampleIntegration(ExampleEndpoint endpoint)
    {
        registration = AllureRuntimeRouter.Install(new ExampleRoute(endpoint));
    }

    public void Dispose() => registration.Dispose();
}
```

Keep the registration alive for as long as the integration can handle calls, and dispose of it during framework shutdown. Routes may declare `SuppressedRouteIds` when a more specific integration must take precedence over another matching route. If multiple unsuppressed routes match the same scope, endpoint resolution fails.
