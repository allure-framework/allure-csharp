using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Documentation;

public class SetPreconditionsTests : AllureApiTestsBase
{
    const string Content = "## Preconditions\n- The user is signed in.\n- The cart contains an item.";

    [Test]
    public async Task SetPreconditionsRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetPreconditions(Content);

        await Assert.That(endpoint.SyncApi.SetPreconditions(Content)).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetPreconditionsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetPreconditions("No endpoint value");
    }

    [Test]
    public async Task SetPreconditionsAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetPreconditionsAsync("No endpoint value");
    }

    [Test]
    public async Task SetPreconditionsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetPreconditionsAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetPreconditionsAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetPreconditionsAsync(Content);

        await Assert.That(endpoint.AsyncApi.SetPreconditionsAsync(Content, CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetPreconditionsAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetPreconditionsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetPreconditionsAsync(Content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetPreconditionsAsyncWithTokenRoutedToEndpoint()
    {
        using var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetPreconditionsAsync(Content, ts.Token);

        await Assert.That(endpoint.AsyncApi.SetPreconditionsAsync(Content, ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetPreconditionsAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetPreconditionsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetPreconditionsAsync(Content, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
