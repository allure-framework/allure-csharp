using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Fixtures;

public class InProcessSetUpTests : AllureApiTestsBase
{
    [Test]
    public async Task SetUpContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureInProcessFixtureContext> body = _ => { };
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);

        AllureInProcessApi.SetUp("Fixture name", body);

        await Assert.That(endpoint.SyncApi.SetUp(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureInProcessApi.SetUp("Fixture name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpContextFunctionReturnsEndpointValue()
    {
        Func<IAllureInProcessFixtureContext, int> body = _ => 17;
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(
            Any(),
            Any(),
            Any<Func<IAllureInProcessFixtureContext, int>>()
        ).Returns(42);

        var actual = AllureInProcessApi.SetUp("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.SetUp(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureInProcessApi.SetUp("Fixture name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureAsyncInProcessFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncInProcessFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.SetUpAsync("Fixture name", body);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.SetUpAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncContextActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureAsyncInProcessFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncInProcessFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncContextActionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.SetUpAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncCancellableContextActionReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureInProcessApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncCancellableContextActionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureInProcessApi.SetUpAsync("Fixture name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task SetUpAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureAsyncInProcessFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncInProcessFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncContextFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureAsyncInProcessFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncInProcessFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncContextFunctionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", async context =>
        {
            await ExerciseAsync(context);
            called = true;
            return 17;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncCancellableContextFunctionReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallInProcessEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(),
            Any(),
            Any<Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncCancellableContextFunctionExecutesWithContextAndTokenWithoutEndpoint()
    {
        var called = false;
        CancellationToken observedToken = default;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureInProcessApi.SetUpAsync("Fixture name", async (context, token) =>
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

    private static void Exercise(IAllureInProcessFixtureContext context)
    {
        context.UpdateFixtureResult(_ =>
            throw new InvalidOperationException("A null context must not invoke the update.")
        );
        if (context.TryReadFixtureResult(_ => 17, out _))
        {
            throw new InvalidOperationException("A null context must not expose a result.");
        }
    }

    private static Task ExerciseAsync(IAllureAsyncInProcessFixtureContext context)
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
