using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Fixtures;

public class InProcessTearDownTests : AllureApiTestsBase
{
    [Test]
    public async Task TearDownContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureInProcessSyncFixtureContext> body = _ => { };
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.TearDown("Fixture name", body);

        await Assert.That(endpoint.SyncApi.TearDown(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.TearDown("Fixture name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownContextFunctionReturnsEndpointValue()
    {
        Func<IAllureInProcessSyncFixtureContext, int> body = _ => 17;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(
            Any(),
            Any(),
            Any<Func<IAllureInProcessSyncFixtureContext, int>>()
        ).Returns(42);

        var actual = AllureInProcessApi.TearDown("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.TearDown(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureInProcessApi.TearDown("Fixture name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureInProcessAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.TearDownAsync("Fixture name", body);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.TearDownAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.TearDownAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncContextActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.TearDownAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.TearDownAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncContextActionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.TearDownAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncCancellableContextActionReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.TearDownAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.TearDownAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncCancellableContextActionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.TearDownAsync("Fixture name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task TearDownAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureInProcessAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), Any(), Any<Func<IAllureInProcessAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.TearDownAsync<int>(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncContextFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), Any(), Any<Func<IAllureInProcessAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.TearDownAsync<int>(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncContextFunctionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncCancellableContextFunctionReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(),
            Any(),
            Any<Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.TearDownAsync<int>(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownAsyncCancellableContextFunctionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.TearDownAsync("Fixture name", async (context, token) =>
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

    private static void Exercise(IAllureInProcessSyncFixtureContext context)
    {
        context.UpdateFixtureResult(_ =>
            throw new InvalidOperationException("A null context must not invoke the update.")
        );
        if (context.TryReadFixtureResult(_ => 17, out _))
        {
            throw new InvalidOperationException("A null context must not expose a result.");
        }
    }

    private static Task ExerciseAsync(IAllureInProcessAsyncFixtureContext context)
    {
        context.UpdateFixtureResult(_ =>
            throw new InvalidOperationException("A null context must not invoke the update.")
        );
        if (context.TryReadFixtureResult(_ => 17, out _))
        {
            throw new InvalidOperationException("A null context must not expose a result.");
        }
        return Task.CompletedTask;
    }
}
