using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Steps;

public class StepTests : AllureApiTestsBase
{
    [Test]
    public async Task StepWithNameRoutedToCurrentEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.Step("Step name");

        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Passed,
            IsNull<StatusDetails?>()
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void StepWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.Step("Step name");
    }

    [Test]
    public async Task StepWithStatusRoutedToCurrentEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.Step("Step name", Status.Skipped);

        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Skipped,
            IsNull<StatusDetails?>()
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void StepWithStatusDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.Step("Step name", Status.Skipped);
    }

    [Test]
    public async Task StepWithStatusDetailsRoutedToCurrentEndpoint()
    {
        var details = new StatusDetails { Message = "Failure message" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.Step("Step name", Status.Failed, details);

        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Failed,
            Is<StatusDetails?>(details)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void StepWithStatusDetailsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.Step(
            "Step name",
            Status.Failed,
            new StatusDetails { Message = "Failure message" }
        );
    }

    [Test]
    public async Task StepActionRoutedToCurrentEndpoint()
    {
        Action body = () => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.Step("Step name", body);

        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        AllureApi.Step("Step name", () => called = true);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepContextActionRoutedToCurrentEndpoint()
    {
        Action<IAllureStepContext> body = _ => { };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.Step("Step name", body);

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

        AllureApi.Step("Step name", context =>
        {
            Exercise(context);
            called = true;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepFunctionReturnsEndpointValue()
    {
        Func<int> body = () => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Func<int>>()).Returns(42);

        var actual = AllureApi.Step("Step name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.Step(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = AllureApi.Step("Step name", () =>
        {
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepContextFunctionReturnsEndpointValue()
    {
        Func<IAllureStepContext, int> body = _ => 17;
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(
            Any(),
            Any(),
            Any<Func<IAllureStepContext, int>>()
        ).Returns(42);

        var actual = AllureApi.Step("Step name", body);

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

        var actual = AllureApi.Step("Step name", context =>
        {
            Exercise(context);
            called = true;
            return 17;
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncWithNameRoutedToCurrentEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name");

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Passed,
            IsNull<StatusDetails?>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithStatusRoutedToCurrentEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name", Status.Skipped);

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Skipped,
            IsNull<StatusDetails?>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name");
    }

    [Test]
    public async Task StepAsyncWithStatusDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", Status.Skipped);
    }

    [Test]
    public async Task StepAsyncWithNameAndTokenRoutedToCurrentEndpoint()
    {
        using var cancellation = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name", cancellation.Token);

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Passed,
            IsNull<StatusDetails?>(),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithNameAndTokenDoesNotThrowWithoutEndpoint()
    {
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", cancellation.Token);
    }

    [Test]
    public async Task StepAsyncWithStatusAndTokenRoutedToCurrentEndpoint()
    {
        using var cancellation = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name", Status.Broken, cancellation.Token);

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Broken,
            IsNull<StatusDetails?>(),
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithStatusAndTokenDoesNotThrowWithoutEndpoint()
    {
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", Status.Broken, cancellation.Token);
    }

    [Test]
    public async Task StepAsyncWithStatusDetailsRoutedToCurrentEndpoint()
    {
        var details = new StatusDetails { Message = "Failure message" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name", Status.Failed, details);

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Failed,
            details,
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithStatusDetailsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync(
            "Step name",
            Status.Failed,
            new StatusDetails { Message = "Failure message" }
        );
    }

    [Test]
    public async Task StepAsyncWithStatusDetailsAndTokenRoutedToCurrentEndpoint()
    {
        var details = new StatusDetails { Message = "Failure message" };
        using var cancellation = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.StepAsync("Step name", Status.Failed, details, cancellation.Token);

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Status.Failed,
            details,
            cancellation.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncWithStatusDetailsAndTokenDoesNotThrowWithoutEndpoint()
    {
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync(
            "Step name",
            Status.Failed,
            new StatusDetails { Message = "Failure message" },
            cancellation.Token
        );
    }

    [Test]
    public async Task StepAsyncActionReturnsEndpointTask()
    {
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.StepAsync("Step name", body);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncActionExecutesWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", () =>
        {
            called = true;
            return Task.CompletedTask;
        });

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepAsyncActionWithTokenReturnsEndpointTask()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task> body = () => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(Any(), Any(), Any<Func<Task>>(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.StepAsync("Step name", body, cancellation.Token);

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
    public async Task StepAsyncActionWithTokenExecutesWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", () =>
        {
            called = true;
            return Task.CompletedTask;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task StepAsyncContextActionReturnsEndpointTask()
    {
        Func<IAllureAsyncStepContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureAsyncStepContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.StepAsync("Step name", body);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextActionExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", async context =>
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
        Func<IAllureAsyncStepContext, Task> body = _ => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureAsyncStepContext, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.StepAsync("Step name", body, cancellation.Token);

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
    public async Task StepAsyncContextActionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        await AllureApi.StepAsync("Step name", async context =>
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
        Func<IAllureAsyncStepContext, CancellationToken, Task> body = (_, _) => Task.CompletedTask;
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(), Any(), Any<Func<IAllureAsyncStepContext, CancellationToken, Task>>(), Any()
        ).ReturnsAsync(tcs.Task);

        var actual = AllureApi.StepAsync("Step name", body, cancellation.Token);

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

        await AllureApi.StepAsync("Step name", async (context, token) =>
        {
            await ExerciseAsync(context);
            called = true;
            observedToken = token;
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(observedToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task StepAsyncFunctionReturnsEndpointValue()
    {
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.StepAsync("Step name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync<int>(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncFunctionExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.StepAsync("Step name", () =>
        {
            called = true;
            return Task.FromResult(17);
        });

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncFunctionWithTokenReturnsEndpointValue()
    {
        using var cancellation = new CancellationTokenSource();
        Func<Task<int>> body = () => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.StepAsync("Step name", body, cancellation.Token);

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
    public async Task StepAsyncFunctionWithTokenExecutesAndReturnsValueWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.StepAsync("Step name", () =>
        {
            called = true;
            return Task.FromResult(17);
        }, cancellation.Token);

        await Assert.That(called).IsTrue();
        await Assert.That(actual).IsEqualTo(17);
    }

    [Test]
    public async Task StepAsyncContextFunctionReturnsEndpointValue()
    {
        Func<IAllureAsyncStepContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<IAllureAsyncStepContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.StepAsync("Step name", body);

        await Assert.That(actual).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync<int>(
            "Step name",
            IsEmpty<IEnumerable<Parameter>>(),
            Is(body),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task StepAsyncContextFunctionExecutesAndReturnsValueWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.StepAsync("Step name", async context =>
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
        Func<IAllureAsyncStepContext, Task<int>> body = _ => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(), Any(), Any<Func<IAllureAsyncStepContext, Task<int>>>(), Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.StepAsync("Step name", body, cancellation.Token);

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
    public async Task StepAsyncContextFunctionWithTokenExecutesWithUsableContextWithoutEndpoint()
    {
        var called = false;
        using var cancellation = new CancellationTokenSource();
        using var _ = InstallNoEndpoint();

        var actual = await AllureApi.StepAsync("Step name", async context =>
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
        Func<IAllureAsyncStepContext, CancellationToken, Task<int>> body = (_, _) => Task.FromResult(17);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync<int>(
            Any(),
            Any(),
            Any<Func<IAllureAsyncStepContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var actual = await AllureApi.StepAsync("Step name", body, cancellation.Token);

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

        var actual = await AllureApi.StepAsync("Step name", async (context, token) =>
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

    private static void Exercise(IAllureStepContext context)
    {
        context.SetName("Updated name");
        context.AddParameter(new Parameter { Name = "parameter", Value = "value" });
        context.AddParameter("text parameter", "text value");
        context.AddParameter("masked parameter", "masked value", ParameterMode.Masked);
        context.AddParameterFromObject("object parameter", 17);
        context.AddParameterFromObject("hidden parameter", 18, ParameterMode.Hidden);
        _ = context.ParameterSerializer.Serialize(19);
    }

    private static async Task ExerciseAsync(IAllureAsyncStepContext context)
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
