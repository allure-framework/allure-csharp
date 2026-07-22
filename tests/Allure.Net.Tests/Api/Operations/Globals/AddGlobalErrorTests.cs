using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Globals;

public class AddGlobalErrorTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddGlobalErrorExceptionRoutedToGlobalEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalError(error);

        await Assert.That(endpoint.SyncApi.AddGlobalError(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "exception message",
                Trace: not null,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalErrorExceptionDoesNotThrowWithoutEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalError(error);
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncRoutedToGlobalEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "exception message",
                Trace: not null,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncResultTaskForwardedToCaller()
    {
        var error = new InvalidOperationException("exception message");
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncDoesNotThrowWithoutEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error);
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "exception message",
                Trace: not null,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncWithTokenResultTaskForwardedToCaller()
    {
        var error = new InvalidOperationException("exception message");
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorExceptionAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        var error = new InvalidOperationException("exception message");
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalErrorMessageRoutedToGlobalEndpoint()
    {
        const string error = "string message";
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalError(error);

        await Assert.That(endpoint.SyncApi.AddGlobalError(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "string message",
                Trace: null,
                Flaky: false,
                Known: false,
                Muted: false,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalErrorMessageDoesNotThrowWithoutEndpoint()
    {
        const string error = "string message";
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalError(error);
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncRoutedToGlobalEndpoint()
    {
        const string error = "string message";
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "string message",
                Trace: null,
                Flaky: false,
                Known: false,
                Muted: false,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncResultTaskForwardedToCaller()
    {
        const string error = "string message";
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncDoesNotThrowWithoutEndpoint()
    {
        const string error = "string message";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error);
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncWithTokenRoutedToGlobalEndpoint()
    {
        const string error = "string message";
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "string message",
                Trace: null,
                Flaky: false,
                Known: false,
                Muted: false,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncWithTokenResultTaskForwardedToCaller()
    {
        const string error = "string message";
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorMessageAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string error = "string message";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsRoutedToGlobalEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalError(error);

        await Assert.That(endpoint.SyncApi.AddGlobalError(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "details message",
                Trace: "details trace",
                Flaky: true,
                Known: true,
                Muted: true,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalErrorStatusDetailsDoesNotThrowWithoutEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalError(error);
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncRoutedToGlobalEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "details message",
                Trace: "details trace",
                Flaky: true,
                Known: true,
                Muted: true,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncResultTaskForwardedToCaller()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncDoesNotThrowWithoutEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error);
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncWithTokenRoutedToGlobalEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalErrorAsync(error, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalErrorAsync(
            (value) => value is
            {
                Timestamp: > 0,
                Message: "details message",
                Trace: "details trace",
                Flaky: true,
                Known: true,
                Muted: true,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncWithTokenResultTaskForwardedToCaller()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalErrorAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalErrorStatusDetailsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        StatusDetails error = new()
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalErrorAsync(error, CancellationToken.None);
    }

}

