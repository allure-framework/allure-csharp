using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Documentation;

public class SetExpectedResultsHtmlTests : AllureApiTestsBase
{
    const string Content = "<h2>Expected results</h2><ul><li>The order is created.</li><li>A confirmation is displayed.</li></ul>";

    [Test]
    public async Task SetExpectedResultsHtmlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetExpectedResultsHtml(Content);

        await Assert.That(endpoint.SyncApi.SetExpectedResultsHtml(Content)).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetExpectedResultsHtmlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetExpectedResultsHtml("No endpoint value");
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetExpectedResultsHtmlAsync("No endpoint value");
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetExpectedResultsHtmlAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetExpectedResultsHtmlAsync(Content);

        await Assert.That(endpoint.AsyncApi.SetExpectedResultsHtmlAsync(Content, CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetExpectedResultsHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetExpectedResultsHtmlAsync(Content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncWithTokenRoutedToEndpoint()
    {
        using var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetExpectedResultsHtmlAsync(Content, ts.Token);

        await Assert.That(endpoint.AsyncApi.SetExpectedResultsHtmlAsync(Content, ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetExpectedResultsHtmlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetExpectedResultsHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetExpectedResultsHtmlAsync(Content, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
