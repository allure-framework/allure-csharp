using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Parameters;

public class AddTestParameterTests : AllureApiTestsBase
{
    [Test]
    public async Task AddTestParameterByNameAndValueRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTestParameter("parameter-name", "parameter-value");

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: false,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterByNameAndValueDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameter("parameter-name", "parameter-value");
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value");

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: false,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value");
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: false,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterByNameAndValueAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", CancellationToken.None);
    }

    [Test]
    public async Task AddTestParameterWithModeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTestParameter("parameter-name", "parameter-value", ParameterMode.Masked);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Masked,
                Excluded: false,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterWithModeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameter("parameter-name", "parameter-value", ParameterMode.Masked);
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Masked);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Masked,
                Excluded: false,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Masked);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Masked);
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Masked, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Masked,
                Excluded: false,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync(
            "parameter-name",
            "parameter-value",
            ParameterMode.Masked,
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithModeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync(
            "parameter-name",
            "parameter-value",
            ParameterMode.Masked,
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddTestParameterWithExcludedRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTestParameter("parameter-name", "parameter-value", true);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: true,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterWithExcludedDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameter("parameter-name", "parameter-value", true);
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: true,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true);
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: null,
                Excluded: true,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithExcludedAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", true, CancellationToken.None);
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTestParameter("parameter-name", "parameter-value", ParameterMode.Hidden, true);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterWithModeAndExcludedDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameter("parameter-name", "parameter-value", ParameterMode.Hidden, true);
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Hidden, true);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Hidden, true);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync("parameter-name", "parameter-value", ParameterMode.Hidden, true);
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync(
            "parameter-name",
            "parameter-value",
            ParameterMode.Hidden,
            true,
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "parameter-value",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync(
            "parameter-name",
            "parameter-value",
            ParameterMode.Hidden,
            true,
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterWithModeAndExcludedAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync(
            "parameter-name",
            "parameter-value",
            ParameterMode.Hidden,
            true,
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddTestParameterModelRoutedToEndpoint()
    {
        Parameter parameter = new() { Name = "parameter-name", Value = "parameter-value" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddTestParameter(parameter);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (value) => ReferenceEquals(value, parameter)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterModelDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameter(new Parameter
        {
            Name = "parameter-name",
            Value = "parameter-value"
        });
    }

    [Test]
    public async Task AddTestParameterModelAsyncRoutedToEndpoint()
    {
        Parameter parameter = new() { Name = "parameter-name", Value = "parameter-value" };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync(parameter);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (value) => ReferenceEquals(value, parameter),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterModelAsyncResultTaskForwardedToCaller()
    {
        Parameter parameter = new() { Name = "parameter-name", Value = "parameter-value" };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync(parameter);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterModelAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync(new Parameter
        {
            Name = "parameter-name",
            Value = "parameter-value"
        });
    }

    [Test]
    public async Task AddTestParameterModelAsyncWithTokenRoutedToEndpoint()
    {
        Parameter parameter = new() { Name = "parameter-name", Value = "parameter-value" };
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddTestParameterAsync(parameter, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (value) => ReferenceEquals(value, parameter),
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterModelAsyncWithTokenResultTaskForwardedToCaller()
    {
        Parameter parameter = new() { Name = "parameter-name", Value = "parameter-value" };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterAsync(parameter, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterModelAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterAsync(
            new Parameter { Name = "parameter-name", Value = "parameter-value" },
            CancellationToken.None
        );
    }
}
