using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Fixtures;

public class TearDownTests : ApiOperationTestsBase
{
    [Test]
    public async Task TearDownActionRoutedToCurrentEndpoint()
    {
        Action body = () => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.TearDown("Fixture name", body);

        await Assert.That(endpoint.SyncApi.TearDown(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureApi.TearDown("Fixture name", () => called = true);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureFixtureContext> body = _ => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.TearDown("Fixture name", body);

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

        AllureApi.TearDown("Fixture name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownFunctionReturnsEndpointValue()
    {
        Func<int> body = () => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(Any(), _ => true, Any<Func<int>>()).Returns(42);

        var actual = AllureApi.TearDown("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.TearDown(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TearDownFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureApi.TearDown("Fixture name", () =>
        {
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownContextFunctionReturnsEndpointValue()
    {
        Func<IAllureFixtureContext, int> body = _ => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(
            Any(),
            _ => true,
            Any<Func<IAllureFixtureContext, int>>()
        ).Returns(42);

        var actual = AllureApi.TearDown("Fixture name", body);

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

        var actual = AllureApi.TearDown("Fixture name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncActionReturnsEndpointTask()
    {
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.TearDownAsync("Fixture name", body);

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
    public async Task TearDownAsyncActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureApi.TearDownAsync("Fixture name", () =>
        {
            called = true;
            return Task.CompletedTask;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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
    public async Task TearDownAsyncActionWithTokenExecutesWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.TearDownAsync("Fixture name", () =>
        {
            called = true;
            return Task.CompletedTask;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task TearDownAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.TearDownAsync("Fixture name", body);

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

        await AllureApi.TearDownAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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

        await AllureApi.TearDownAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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

        await AllureApi.TearDownAsync("Fixture name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task TearDownAsyncFunctionReturnsEndpointValue()
    {
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), _ => true, Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.TearDownAsync("Fixture name", body);

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
    public async Task TearDownAsyncFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.TearDownAsync("Fixture name", () =>
        {
            called = true;
            return Task.FromResult(17);
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), _ => true, Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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
    public async Task TearDownAsyncFunctionWithTokenExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.TearDownAsync("Fixture name", () =>
        {
            called = true;
            return Task.FromResult(17);
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task TearDownAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), _ => true, Any<Func<IAllureAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.TearDownAsync("Fixture name", body);

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

        var actual = await AllureApi.TearDownAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(), _ => true, Any<Func<IAllureAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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

        var actual = await AllureApi.TearDownAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync<int>(
            Any(),
            _ => true,
            Any<Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.TearDownAsync("Fixture name", body, cancellation.Token);

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

        var actual = await AllureApi.TearDownAsync("Fixture name", async (context, token) =>
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

    private static void Exercise(IAllureFixtureContext context)
    {
        context.SetName("Updated name");
        context.AddParameter(new Parameter { Name = "parameter", Value = "value" });
        context.AddParameter("text parameter", "text value");
        context.AddParameter("masked parameter", "masked value", ParameterMode.Masked);
        context.AddParameterFromObject("object parameter", 17);
        context.AddParameterFromObject("hidden parameter", 18, ParameterMode.Hidden);
        _ = context.ParameterSerializer.Serialize(19);
    }

    private static async Task ExerciseAsync(IAllureAsyncFixtureContext context)
    {
        using var cancellation = new CancellationTokenSource();
        var parameter = new Parameter { Name = "parameter", Value = "value" };

        await context.SetNameAsync("Updated name");
        await context.SetNameAsync("Updated name", cancellation.Token);
        await context.AddParameterAsync(parameter);
        await context.AddParameterAsync(parameter, cancellation.Token);
        await context.AddParameterAsync("text parameter", "text value");
        await context.AddParameterAsync("text parameter", "text value", cancellation.Token);
        await context.AddParameterAsync("masked parameter", "masked value", ParameterMode.Masked);
        await context.AddParameterAsync(
            "masked parameter",
            "masked value",
            ParameterMode.Masked,
            cancellation.Token
        );
        await context.AddParameterFromObjectAsync("object parameter", 17);
        await context.AddParameterFromObjectAsync("object parameter", 17, cancellation.Token);
        await context.AddParameterFromObjectAsync("hidden parameter", 18, ParameterMode.Hidden);
        await context.AddParameterFromObjectAsync(
            "hidden parameter",
            18,
            ParameterMode.Hidden,
            cancellation.Token
        );
        _ = context.ParameterSerializer.Serialize(19);
    }
}
