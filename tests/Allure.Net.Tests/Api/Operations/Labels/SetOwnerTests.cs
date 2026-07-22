using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class SetOwnerTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetOwnerRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetOwner("Ada");

        await Assert.That(endpoint.SyncApi.SetLabel("owner", "Ada")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetOwnerDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetOwner("No endpoint owner");
    }

    [Test]
    public async Task SetOwnerAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetOwnerAsync("No endpoint owner");
    }

    [Test]
    public async Task SetOwnerAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetOwnerAsync("No endpoint owner", CancellationToken.None);
    }

    [Test]
    public async Task SetOwnerAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetOwnerAsync("Ada");

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("owner", "Ada", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetOwnerAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetOwnerAsync("Ada");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetOwnerAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetOwnerAsync("Ada", cts.Token);

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("owner", "Ada", cts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetOwnerAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetOwnerAsync("Ada", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
