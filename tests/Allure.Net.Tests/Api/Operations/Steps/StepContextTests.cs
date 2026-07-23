using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Steps;

public class StepContextTests
{
    [Test]
    public async Task AddParameterFromNameAndValueCreatesParameter()
    {
        var context = IAllureSyncStepContext.Mock();

        context.AddParameter("Parameter name", "Parameter value");

        await Assert.That(context.AddParameter(parameter => parameter is
        {
            Name: "Parameter name",
            Value: "Parameter value",
            Mode: null,
        })).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterWithModeCreatesParameter()
    {
        var context = IAllureSyncStepContext.Mock();

        context.AddParameter("Parameter name", "Parameter value", ParameterMode.Masked);

        await Assert.That(context.AddParameter(parameter => parameter is
        {
            Name: "Parameter name",
            Value: "Parameter value",
            Mode: ParameterMode.Masked,
        })).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterFromObjectSerializesValueAndCreatesParameter()
    {
        var context = CreateSyncContext();

        context.AddParameterFromObject("Parameter name", 17);

        await Assert.That(context.AddParameter(parameter => parameter is
        {
            Name: "Parameter name",
            Value: "serialized:17",
            Mode: null,
        })).WasCalled(Times.Once);
    }

    [Test]
    public async Task AddParameterFromObjectWithModeSerializesValueAndCreatesParameter()
    {
        var context = CreateSyncContext();

        context.AddParameterFromObject("Parameter name", 17, ParameterMode.Hidden);

        await Assert.That(context.AddParameter(parameter => parameter is
        {
            Name: "Parameter name",
            Value: "serialized:17",
            Mode: ParameterMode.Hidden,
        })).WasCalled(Times.Once);
    }

    [Test]
    public async Task SetNameAsyncWithoutTokenForwardsDefaultTokenAndTask()
    {
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.SetNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.SetNameAsync("Updated name");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.SetNameAsync(
            "Updated name",
            CancellationToken.None
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterAsyncWithoutTokenForwardsParameterDefaultTokenAndTask()
    {
        var parameter = new Parameter { Name = "Parameter name", Value = "Parameter value" };
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.AddParameterAsync(parameter);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            Is(parameter),
            CancellationToken.None
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterAsyncFromNameAndValueCreatesParameter()
    {
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.AddParameterAsync("Parameter name", "Parameter value");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "Parameter value",
                Mode: null,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterAsyncFromNameAndValueWithTokenCreatesParameter()
    {
        using var cancellation = new CancellationTokenSource();
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.AddParameterAsync(
            "Parameter name",
            "Parameter value",
            cancellation.Token
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "Parameter value",
                Mode: null,
            },
            cancellation.Token
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterAsyncWithModeCreatesParameter()
    {
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.AddParameterAsync(
            "Parameter name",
            "Parameter value",
            ParameterMode.Masked
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "Parameter value",
                Mode: ParameterMode.Masked,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterAsyncWithModeAndTokenCreatesParameter()
    {
        using var cancellation = new CancellationTokenSource();
        TaskCompletionSource tcs = new();
        var context = IAllureAsyncStepContext.Mock();
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = context.AddParameterAsync(
            "Parameter name",
            "Parameter value",
            ParameterMode.Masked,
            cancellation.Token
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "Parameter value",
                Mode: ParameterMode.Masked,
            },
            cancellation.Token
        )).WasCalled(Times.Once);
        context.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParameterFromObjectAsyncSerializesValueAndCreatesParameter()
    {
        TaskCompletionSource tcs = new();
        var context = CreateAsyncContext(tcs.Task);

        var actual = context.AddParameterFromObjectAsync("Parameter name", 17);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "serialized:17",
                Mode: null,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task AddParameterFromObjectAsyncWithTokenSerializesValueAndCreatesParameter()
    {
        using var cancellation = new CancellationTokenSource();
        TaskCompletionSource tcs = new();
        var context = CreateAsyncContext(tcs.Task);

        var actual = context.AddParameterFromObjectAsync(
            "Parameter name",
            17,
            cancellation.Token
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "serialized:17",
                Mode: null,
            },
            cancellation.Token
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task AddParameterFromObjectAsyncWithModeSerializesValueAndCreatesParameter()
    {
        TaskCompletionSource tcs = new();
        var context = CreateAsyncContext(tcs.Task);

        var actual = context.AddParameterFromObjectAsync(
            "Parameter name",
            17,
            ParameterMode.Hidden
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "serialized:17",
                Mode: ParameterMode.Hidden,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task AddParameterFromObjectAsyncWithModeAndTokenSerializesValueAndCreatesParameter()
    {
        using var cancellation = new CancellationTokenSource();
        TaskCompletionSource tcs = new();
        var context = CreateAsyncContext(tcs.Task);

        var actual = context.AddParameterFromObjectAsync(
            "Parameter name",
            17,
            ParameterMode.Hidden,
            cancellation.Token
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
        await Assert.That(context.AddParameterAsync(
            parameter => parameter is
            {
                Name: "Parameter name",
                Value: "serialized:17",
                Mode: ParameterMode.Hidden,
            },
            cancellation.Token
        )).WasCalled(Times.Once);
    }

    private static IAllureSyncStepContextMock CreateSyncContext()
    {
        var context = IAllureSyncStepContext.Mock();
        context.ParameterSerializer.Returns(new TestParameterSerializer());
        return context;
    }

    private static IAllureAsyncStepContextMock CreateAsyncContext(Task resultTask)
    {
        var context = IAllureAsyncStepContext.Mock();
        context.ParameterSerializer.Returns(new TestParameterSerializer());
        context.AddParameterAsync(Any(), Any()).ReturnsAsync(resultTask);
        return context;
    }
}
