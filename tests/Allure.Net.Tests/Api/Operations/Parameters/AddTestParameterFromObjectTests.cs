using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Parameters;

public class AddTestParameterFromObjectTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddTestParameterFromObjectByNameAndValueRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        AllureApi.AddTestParameterFromObject("parameter-name", 42);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: false,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterFromObjectByNameAndValueDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameterFromObject("parameter-name", 42);
    }

    [Test]
    public async Task AddTestParameterFromObjectByNameAndValueAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: false,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectByNameAndValueAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectByNameAndValueAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42);
    }

    [Test]
    public async Task AddTestParameterFromObjectAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: false,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, CancellationToken.None);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        AllureApi.AddTestParameterFromObject("parameter-name", 42, ParameterMode.Masked);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Masked,
                Excluded: false,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterFromObjectWithModeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameterFromObject("parameter-name", 42, ParameterMode.Masked);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Masked);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Masked,
                Excluded: false,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Masked);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Masked);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Masked, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Masked,
                Excluded: false,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync(
            "parameter-name",
            42,
            ParameterMode.Masked,
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync(
            "parameter-name",
            42,
            ParameterMode.Masked,
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        AllureApi.AddTestParameterFromObject("parameter-name", 42, true);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: true,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterFromObjectWithExcludedDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameterFromObject("parameter-name", 42, true);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: true,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: null,
                Excluded: true,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithExcludedAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, true, CancellationToken.None);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        AllureApi.AddTestParameterFromObject("parameter-name", 42, ParameterMode.Hidden, true);

        await Assert.That(endpoint.SyncApi.AddTestParameter(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            }
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddTestParameterFromObjectWithModeAndExcludedDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddTestParameterFromObject("parameter-name", 42, ParameterMode.Hidden, true);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Hidden, true);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            },
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Hidden, true);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Hidden, true);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );

        await AllureApi.AddTestParameterFromObjectAsync("parameter-name", 42, ParameterMode.Hidden, true, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddTestParameterAsync(
            (parameter) => parameter is
            {
                Name: "parameter-name",
                Value: "serialized:42",
                Mode: ParameterMode.Hidden,
                Excluded: true,
            },
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer()
        );
        endpoint.AsyncApi.AddTestParameterAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddTestParameterFromObjectAsync(
            "parameter-name",
            42,
            ParameterMode.Hidden,
            true,
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddTestParameterFromObjectWithModeAndExcludedAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddTestParameterFromObjectAsync(
            "parameter-name",
            42,
            ParameterMode.Hidden,
            true,
            CancellationToken.None
        );
    }

}
