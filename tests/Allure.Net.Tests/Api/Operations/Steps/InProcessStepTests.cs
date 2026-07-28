using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Steps;

public class InProcessStepTests : AllureApiTestsBase
{
    [Test]
    public async Task StepContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureInProcessSyncStepContext> body = _ => { };
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.Step("Step name", body);

        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.Step("Step name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepContextFunctionReturnsEndpointValue()
    {
        Func<IAllureInProcessSyncStepContext, int> body = _ => 17;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(
            Any(),
            Any(),
            Any<Func<IAllureInProcessSyncStepContext, int>>()
        ).Returns(42);

        var actual = AllureInProcessApi.Step("Step name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureInProcessApi.Step("Step name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureInProcessAsyncStepContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.StepAsync("Step name", body);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.StepAsync("Step name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepAsyncContextActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncStepContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.StepAsync("Step name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task>>(),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextActionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.StepAsync("Step name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepAsyncCancellableContextActionReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncStepContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.StepAsync("Step name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncCancellableContextActionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.StepAsync("Step name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task StepAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureInProcessAsyncStepContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.StepAsync("Step name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync<int>(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.StepAsync("Step name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncContextFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncStepContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.StepAsync("Step name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync<int>(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>>>(),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextFunctionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.StepAsync("Step name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncCancellableContextFunctionReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(),
            Any(),
            Any<Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.StepAsync("Step name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync<int>(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncCancellableContextFunctionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.StepAsync("Step name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
            return 17;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    private static void Exercise(IAllureInProcessSyncStepContext context)
    {
        context.UpdateStepResult(_ =>
            throw new InvalidOperationException("A null context must not invoke the update.")
        );
        if (context.TryReadStepResult(_ => 17, out _))
        {
            throw new InvalidOperationException("A null context must not expose a result.");
        }
    }

    private static Task ExerciseAsync(IAllureInProcessAsyncStepContext context)
    {
        context.UpdateStepResult(_ =>
            throw new InvalidOperationException("A null context must not invoke the update.")
        );
        if (context.TryReadStepResult(_ => 17, out _))
        {
            throw new InvalidOperationException("A null context must not expose a result.");
        }
        return Task.CompletedTask;
    }
}
