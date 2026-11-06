using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Documentation;

public class SetPreconditionsHtmlTests : AllureApiTestsBase
{
    const string Content = "<h2>Preconditions</h2><ul><li>The user is signed in.</li><li>The cart contains an item.</li></ul>";

    [Test]
    public async Task SetPreconditionsHtmlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetPreconditionsHtml(Content);

        await Assert.That(endpoint.SyncApi.SetPreconditionsHtml(Content)).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetPreconditionsHtmlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetPreconditionsHtml("No endpoint value");
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetPreconditionsHtmlAsync("No endpoint value");
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetPreconditionsHtmlAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetPreconditionsHtmlAsync(Content);

        await Assert.That(endpoint.AsyncApi.SetPreconditionsHtmlAsync(Content, CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetPreconditionsHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetPreconditionsHtmlAsync(Content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncWithTokenRoutedToEndpoint()
    {
        using var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetPreconditionsHtmlAsync(Content, ts.Token);

        await Assert.That(endpoint.AsyncApi.SetPreconditionsHtmlAsync(Content, ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetPreconditionsHtmlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetPreconditionsHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetPreconditionsHtmlAsync(Content, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
