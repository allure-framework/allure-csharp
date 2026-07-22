using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Fixtures;

public class SetUpTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetUpActionRoutedToCurrentEndpoint()
    {
        Action body = () => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetUp("Fixture name", body);

        await Assert.That(endpoint.SyncApi.SetUp(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureApi.SetUp("Fixture name", () => called = true);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureFixtureContext> body = _ => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetUp("Fixture name", body);

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

        AllureApi.SetUp("Fixture name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpFunctionReturnsEndpointValue()
    {
        Func<int> body = () => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(Any(), _ => true, Any<Func<int>>()).Returns(42);

        var actual = AllureApi.SetUp("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.SetUp(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureApi.SetUp("Fixture name", () =>
        {
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpContextFunctionReturnsEndpointValue()
    {
        Func<IAllureFixtureContext, int> body = _ => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(
            Any(),
            _ => true,
            Any<Func<IAllureFixtureContext, int>>()
        ).Returns(42);

        var actual = AllureApi.SetUp("Fixture name", body);

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

        var actual = AllureApi.SetUp("Fixture name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncActionReturnsEndpointTask()
    {
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetUpAsync("Fixture name", body);

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
    public async Task SetUpAsyncActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureApi.SetUpAsync("Fixture name", () =>
        {
            called = true;
            return Task.CompletedTask;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

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
    public async Task SetUpAsyncActionWithTokenExecutesWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.SetUpAsync("Fixture name", () =>
        {
            called = true;
            return Task.CompletedTask;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task SetUpAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetUpAsync("Fixture name", body);

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

        await AllureApi.SetUpAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

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

        await AllureApi.SetUpAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(
            Any(), Any(), Any<Func<IAllureAsyncFixtureContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

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

        await AllureApi.SetUpAsync("Fixture name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task SetUpAsyncFunctionReturnsEndpointValue()
    {
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync<int>(
            Any(), _ => true, Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.SetUpAsync("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync<int>(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.SetUpAsync("Fixture name", () =>
        {
            called = true;
            return Task.FromResult(17);
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync<int>(
            Any(), _ => true, Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync<int>(
            "Fixture name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetUpAsyncFunctionWithTokenExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.SetUpAsync("Fixture name", () =>
        {
            called = true;
            return Task.FromResult(17);
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task SetUpAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync<int>(
            Any(), _ => true, Any<Func<IAllureAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.SetUpAsync("Fixture name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync<int>(
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

        var actual = await AllureApi.SetUpAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync<int>(
            Any(), _ => true, Any<Func<IAllureAsyncFixtureContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync<int>(
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

        var actual = await AllureApi.SetUpAsync("Fixture name", async context =>
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
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync<int>(
            Any(),
            _ => true,
            Any<Func<IAllureAsyncFixtureContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.SetUpAsync("Fixture name", body, cancellation.Token);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync<int>(
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

        var actual = await AllureApi.SetUpAsync("Fixture name", async (context, token) =>
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
