using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class FixtureBodySemanticsTests
{
    [Test]
    public async Task SyncSetUpCoversEveryBodyShape() =>
        await VerifySyncFixtureFamily(
            "SetUp",
            body => AllureApi.SetUp("fixture", body),
            body => AllureApi.SetUp("fixture", body),
            body => AllureApi.SetUp("fixture", body),
            body => AllureApi.SetUp("fixture", body)
        );

    [Test]
    public async Task SyncTearDownCoversEveryBodyShape() =>
        await VerifySyncFixtureFamily(
            "TearDown",
            body => AllureApi.TearDown("fixture", body),
            body => AllureApi.TearDown("fixture", body),
            body => AllureApi.TearDown("fixture", body),
            body => AllureApi.TearDown("fixture", body)
        );

    [Test]
    public async Task AsyncSetUpCoversEveryBodyShape() =>
        await VerifyAsyncFixtureFamily(
            "SetUpAsync",
            (body, token) => AllureApi.SetUpAsync("fixture", body, token),
            (body, token) => AllureApi.SetUpAsync("fixture", body, token),
            (body, token) => AllureApi.SetUpAsync("fixture", body, token),
            (body, token) => AllureApi.SetUpAsync("fixture", body, token),
            (body, token) => AllureApi.SetUpAsync("fixture", body, token),
            (body, token) => AllureApi.SetUpAsync("fixture", body, token)
        );

    [Test]
    public async Task AsyncTearDownCoversEveryBodyShape() =>
        await VerifyAsyncFixtureFamily(
            "TearDownAsync",
            (body, token) => AllureApi.TearDownAsync("fixture", body, token),
            (body, token) => AllureApi.TearDownAsync("fixture", body, token),
            (body, token) => AllureApi.TearDownAsync("fixture", body, token),
            (body, token) => AllureApi.TearDownAsync("fixture", body, token),
            (body, token) => AllureApi.TearDownAsync("fixture", body, token),
            (body, token) => AllureApi.TearDownAsync("fixture", body, token)
        );

    static async Task VerifySyncFixtureFamily(
        string expectedMethod,
        Action<Action> runAction,
        Action<Action<IAllureFixtureContext>> runContextAction,
        Func<Func<int>, int> runFunction,
        Func<Func<IAllureFixtureContext, int>, int> runContextFunction
    )
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        var executions = 0;
        Action action = () => executions++;
        Action<IAllureFixtureContext> contextAction = context =>
        {
            executions++;
            context.SetName("inside");
        };
        Func<int> function = () => { executions++; return 17; };
        Func<IAllureFixtureContext, int> contextFunction = _ =>
        {
            executions++;
            return 18;
        };

        runAction(action);
        runContextAction(contextAction);
        var result = runFunction(function);
        var contextResult = runContextFunction(contextFunction);

        await Assert.That(executions).IsEqualTo(4);
        await Assert.That(result).IsEqualTo(17);
        await Assert.That(contextResult).IsEqualTo(18);
        await Assert.That(operations.Sync.Calls.Count).IsEqualTo(4);
        await Assert.That(operations.Sync.Calls.All(call => call.Method.Name == expectedMethod)).IsTrue();
        await Assert.That(operations.Sync.Calls[0].Arguments[2]).IsSameReferenceAs(action);
        await Assert.That(operations.Sync.Calls[3].Arguments[2]).IsSameReferenceAs(contextFunction);
    }

    static async Task VerifyAsyncFixtureFamily(
        string expectedMethod,
        Func<Func<Task>, CancellationToken, Task> runAction,
        Func<Func<IAllureAsyncFixtureContext, Task>, CancellationToken, Task> runContextAction,
        Func<Func<IAllureAsyncFixtureContext, CancellationToken, Task>, CancellationToken, Task> runTokenAction,
        Func<Func<Task<int>>, CancellationToken, Task<int>> runFunction,
        Func<Func<IAllureAsyncFixtureContext, Task<int>>, CancellationToken, Task<int>> runContextFunction,
        Func<Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>>, CancellationToken, Task<int>> runTokenFunction
    )
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        using var cancellation = new CancellationTokenSource();
        var executions = 0;
        CancellationToken observedToken = default;
        Func<Task> action = () => { executions++; return Task.CompletedTask; };
        Func<IAllureAsyncFixtureContext, Task> contextAction = _ =>
        {
            executions++;
            return Task.CompletedTask;
        };
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> tokenAction = (_, token) =>
        {
            executions++;
            observedToken = token;
            return Task.CompletedTask;
        };
        Func<Task<int>> function = () => { executions++; return Task.FromResult(17); };
        Func<IAllureAsyncFixtureContext, Task<int>> contextFunction = _ =>
        {
            executions++;
            return Task.FromResult(18);
        };
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> tokenFunction = (_, token) =>
        {
            executions++;
            observedToken = token;
            return Task.FromResult(19);
        };

        await runAction(action, cancellation.Token);
        await runContextAction(contextAction, cancellation.Token);
        await runTokenAction(tokenAction, cancellation.Token);
        var result = await runFunction(function, cancellation.Token);
        var contextResult = await runContextFunction(contextFunction, cancellation.Token);
        var tokenResult = await runTokenFunction(tokenFunction, cancellation.Token);

        await Assert.That(executions).IsEqualTo(6);
        await Assert.That(result).IsEqualTo(17);
        await Assert.That(contextResult).IsEqualTo(18);
        await Assert.That(tokenResult).IsEqualTo(19);
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
        await Assert.That(operations.Async.Calls.Count).IsEqualTo(6);
        await Assert.That(operations.Async.Calls.All(call => call.Method.Name == expectedMethod)).IsTrue();
        await Assert.That(operations.Async.Calls.All(call =>
            Equals(call.Arguments[^1], cancellation.Token)
        )).IsTrue();
        await Assert.That(operations.Async.Calls[5].Arguments[2]).IsSameReferenceAs(tokenFunction);
    }
}
