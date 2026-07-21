using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class MissingEndpointBodyTests
{
    [Test]
    public async Task StepExecutesEveryBodyShapeDirectly()
    {
        using var scope = FacadeTestEnvironment.Use();
        using var cancellation = new CancellationTokenSource();
        var executions = 0;
        IAllureStepContext? syncContext = null;
        IAllureAsyncStepContext? asyncContext = null;
        CancellationToken observedToken = default;

        AllureApi.Step("action", () => { executions++; });
        AllureApi.Step("context action", context => { syncContext = context; executions++; });
        var syncResult = AllureApi.Step("function", () => { executions++; return 17; });
        var syncContextResult = AllureApi.Step("context function", context =>
        {
            syncContext = context;
            executions++;
            return 18;
        });
        await AllureApi.StepAsync("action", () => { executions++; return Task.CompletedTask; }, cancellation.Token);
        await AllureApi.StepAsync("context action", context =>
        {
            asyncContext = context;
            executions++;
            return Task.CompletedTask;
        }, cancellation.Token);
        await AllureApi.StepAsync("token action", (context, token) =>
        {
            asyncContext = context;
            observedToken = token;
            executions++;
            return Task.CompletedTask;
        }, cancellation.Token);
        var asyncResult = await AllureApi.StepAsync(
            "function",
            () => { executions++; return Task.FromResult(19); },
            cancellation.Token
        );
        var asyncContextResult = await AllureApi.StepAsync(
            "context function",
            context => { asyncContext = context; executions++; return Task.FromResult(20); },
            cancellation.Token
        );
        var asyncTokenResult = await AllureApi.StepAsync(
            "token function",
            (context, token) =>
            {
                asyncContext = context;
                observedToken = token;
                executions++;
                return Task.FromResult(21);
            },
            cancellation.Token
        );

        await Assert.That(executions).IsEqualTo(10);
        await Assert.That(syncResult).IsEqualTo(17);
        await Assert.That(syncContextResult).IsEqualTo(18);
        await Assert.That(asyncResult).IsEqualTo(19);
        await Assert.That(asyncContextResult).IsEqualTo(20);
        await Assert.That(asyncTokenResult).IsEqualTo(21);
        await Assert.That(syncContext).IsNotNull();
        await Assert.That(asyncContext).IsNotNull();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(10);
    }

    [Test]
    public async Task SetUpAndTearDownExecuteEveryBodyShapeDirectly()
    {
        using var scope = FacadeTestEnvironment.Use();
        using var cancellation = new CancellationTokenSource();
        var executions = 0;

        foreach (var fixture in new FixtureInvoker[] { SetUpInvoker.Instance, TearDownInvoker.Instance })
        {
            fixture.Action(() => executions++);
            fixture.ContextAction(_ => executions++);
            await Assert.That(fixture.Function(() => { executions++; return 17; })).IsEqualTo(17);
            await Assert.That(fixture.ContextFunction(_ => { executions++; return 18; })).IsEqualTo(18);
            await fixture.AsyncAction(() => { executions++; return Task.CompletedTask; }, cancellation.Token);
            await fixture.AsyncContextAction(_ => { executions++; return Task.CompletedTask; }, cancellation.Token);
            await fixture.AsyncTokenAction((_, token) =>
            {
                executions++;
                if (token != cancellation.Token) throw new InvalidOperationException("Wrong token.");
                return Task.CompletedTask;
            }, cancellation.Token);
            await Assert.That(await fixture.AsyncFunction(
                () => { executions++; return Task.FromResult(19); }, cancellation.Token
            )).IsEqualTo(19);
            await Assert.That(await fixture.AsyncContextFunction(
                _ => { executions++; return Task.FromResult(20); }, cancellation.Token
            )).IsEqualTo(20);
            await Assert.That(await fixture.AsyncTokenFunction(
                (_, token) =>
                {
                    executions++;
                    if (token != cancellation.Token) throw new InvalidOperationException("Wrong token.");
                    return Task.FromResult(21);
                }, cancellation.Token
            )).IsEqualTo(21);
        }

        await Assert.That(executions).IsEqualTo(20);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(20);
    }

    [Test]
    public async Task NullContextsProvideNoOpOperationsAndSerializer()
    {
        using var scope = FacadeTestEnvironment.Use();
        string? syncSerialized = null;
        string? asyncSerialized = null;

        AllureApi.Step("sync", context =>
        {
            context.SetName("ignored");
            context.AddParameter(new Parameter { Name = "parameter", Value = "value" });
            context.AddParameterFromObject("object", 42);
            var inProcess = (IAllureInProcessStepContext)context;
            inProcess.UpdateStepResult(_ => throw new InvalidOperationException("must not run"));
            if (inProcess.TryReadStepResult(_ => "value", out _))
            {
                throw new InvalidOperationException("A null context must not expose a result.");
            }
            syncSerialized = context.ParameterSerializer.Serialize(42);
        });
        await AllureApi.StepAsync("async", async context =>
        {
            await context.SetNameAsync("ignored");
            await context.AddParameterAsync(new Parameter { Name = "parameter", Value = "value" });
            await context.AddParameterFromObjectAsync("object", 42);
            asyncSerialized = context.ParameterSerializer.Serialize(42);
        });

        await Assert.That(syncSerialized).IsEqualTo("42");
        await Assert.That(asyncSerialized).IsEqualTo("42");
    }

    [Test]
    public async Task DirectBodyExceptionsPropagate()
    {
        using var scope = FacadeTestEnvironment.Use();

        await Assert.That(() => AllureApi.Step("sync", () =>
            throw new BodyException("sync failure")
        )).Throws<BodyException>().WithMessage("sync failure");

        await Assert.That(() => AllureApi.StepAsync(
            "async",
            () => Task.FromException(new BodyException("async failure"))
        )).Throws<BodyException>().WithMessage("async failure");
    }

    abstract class FixtureInvoker
    {
        public abstract void Action(Action body);
        public abstract void ContextAction(Action<IAllureFixtureContext> body);
        public abstract int Function(Func<int> body);
        public abstract int ContextFunction(Func<IAllureFixtureContext, int> body);
        public abstract Task AsyncAction(Func<Task> body, CancellationToken token);
        public abstract Task AsyncContextAction(Func<IAllureAsyncFixtureContext, Task> body, CancellationToken token);
        public abstract Task AsyncTokenAction(Func<IAllureAsyncFixtureContext, CancellationToken, Task> body, CancellationToken token);
        public abstract Task<int> AsyncFunction(Func<Task<int>> body, CancellationToken token);
        public abstract Task<int> AsyncContextFunction(Func<IAllureAsyncFixtureContext, Task<int>> body, CancellationToken token);
        public abstract Task<int> AsyncTokenFunction(Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> body, CancellationToken token);
    }

    sealed class SetUpInvoker : FixtureInvoker
    {
        public static SetUpInvoker Instance { get; } = new();
        public override void Action(Action body) => AllureApi.SetUp("fixture", body);
        public override void ContextAction(Action<IAllureFixtureContext> body) => AllureApi.SetUp("fixture", body);
        public override int Function(Func<int> body) => AllureApi.SetUp("fixture", body);
        public override int ContextFunction(Func<IAllureFixtureContext, int> body) => AllureApi.SetUp("fixture", body);
        public override Task AsyncAction(Func<Task> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
        public override Task AsyncContextAction(Func<IAllureAsyncFixtureContext, Task> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
        public override Task AsyncTokenAction(Func<IAllureAsyncFixtureContext, CancellationToken, Task> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
        public override Task<int> AsyncFunction(Func<Task<int>> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
        public override Task<int> AsyncContextFunction(Func<IAllureAsyncFixtureContext, Task<int>> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
        public override Task<int> AsyncTokenFunction(Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> body, CancellationToken token) => AllureApi.SetUpAsync("fixture", body, token);
    }

    sealed class TearDownInvoker : FixtureInvoker
    {
        public static TearDownInvoker Instance { get; } = new();
        public override void Action(Action body) => AllureApi.TearDown("fixture", body);
        public override void ContextAction(Action<IAllureFixtureContext> body) => AllureApi.TearDown("fixture", body);
        public override int Function(Func<int> body) => AllureApi.TearDown("fixture", body);
        public override int ContextFunction(Func<IAllureFixtureContext, int> body) => AllureApi.TearDown("fixture", body);
        public override Task AsyncAction(Func<Task> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
        public override Task AsyncContextAction(Func<IAllureAsyncFixtureContext, Task> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
        public override Task AsyncTokenAction(Func<IAllureAsyncFixtureContext, CancellationToken, Task> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
        public override Task<int> AsyncFunction(Func<Task<int>> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
        public override Task<int> AsyncContextFunction(Func<IAllureAsyncFixtureContext, Task<int>> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
        public override Task<int> AsyncTokenFunction(Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> body, CancellationToken token) => AllureApi.TearDownAsync("fixture", body, token);
    }

    sealed class BodyException(string message) : Exception(message);
}
