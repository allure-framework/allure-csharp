using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class StepBodySemanticsTests
{
    [Test]
    public async Task SyncStepCoversEveryBodyShape()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        var executions = 0;
        Action action = () => executions++;
        Action<IAllureStepContext> contextAction = context =>
        {
            executions++;
            context.SetName("inside");
        };
        Func<int> function = () => { executions++; return 17; };
        Func<IAllureStepContext, int> contextFunction = context =>
        {
            executions++;
            context.AddParameter("argument", "value");
            return 18;
        };

        AllureApi.Step("action", action);
        AllureApi.Step("context action", contextAction);
        var result = AllureApi.Step("function", function);
        var contextResult = AllureApi.Step("context function", contextFunction);

        await Assert.That(executions).IsEqualTo(4);
        await Assert.That(result).IsEqualTo(17);
        await Assert.That(contextResult).IsEqualTo(18);
        await Assert.That(operations.Sync.Calls.Count).IsEqualTo(4);
        await Assert.That(operations.Sync.Calls[0].Arguments[2]).IsSameReferenceAs(action);
        await Assert.That(operations.Sync.Calls[1].Arguments[2]).IsSameReferenceAs(contextAction);
        await Assert.That(operations.Sync.Calls[2].Arguments[2]).IsSameReferenceAs(function);
        await Assert.That(operations.Sync.Calls[3].Arguments[2]).IsSameReferenceAs(contextFunction);
    }

    [Test]
    public async Task AsyncStepCoversEveryBodyShapeAndToken()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        using var cancellation = new CancellationTokenSource();
        var executions = 0;
        CancellationToken observedToken = default;
        Func<Task> action = () => { executions++; return Task.CompletedTask; };
        Func<IAllureAsyncStepContext, Task> contextAction = context =>
        {
            executions++;
            return context.SetNameAsync("inside");
        };
        Func<IAllureAsyncStepContext, CancellationToken, Task> tokenAction = (_, token) =>
        {
            executions++;
            observedToken = token;
            return Task.CompletedTask;
        };
        Func<Task<int>> function = () => { executions++; return Task.FromResult(17); };
        Func<IAllureAsyncStepContext, Task<int>> contextFunction = _ =>
        {
            executions++;
            return Task.FromResult(18);
        };
        Func<IAllureAsyncStepContext, CancellationToken, Task<int>> tokenFunction = (_, token) =>
        {
            executions++;
            observedToken = token;
            return Task.FromResult(19);
        };

        await AllureApi.StepAsync("action", action, cancellation.Token);
        await AllureApi.StepAsync("context action", contextAction, cancellation.Token);
        await AllureApi.StepAsync("token action", tokenAction, cancellation.Token);
        var result = await AllureApi.StepAsync("function", function, cancellation.Token);
        var contextResult = await AllureApi.StepAsync("context function", contextFunction, cancellation.Token);
        var tokenResult = await AllureApi.StepAsync("token function", tokenFunction, cancellation.Token);

        await Assert.That(executions).IsEqualTo(6);
        await Assert.That(result).IsEqualTo(17);
        await Assert.That(contextResult).IsEqualTo(18);
        await Assert.That(tokenResult).IsEqualTo(19);
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
        await Assert.That(operations.Async.Calls.Count).IsEqualTo(6);
        await Assert.That(operations.Async.Calls.All(call =>
            Equals(call.Arguments[^1], cancellation.Token)
        )).IsTrue();
        await Assert.That(operations.Async.Calls[2].Arguments[2]).IsSameReferenceAs(tokenAction);
        await Assert.That(operations.Async.Calls[5].Arguments[2]).IsSameReferenceAs(tokenFunction);
    }
}
