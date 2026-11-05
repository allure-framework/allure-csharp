using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Documentation;

public class SetExpectedResultsTests : AllureApiTestsBase
{
    const string Content = "## Expected results\n- The order is created.\n- A confirmation is displayed.";

    [Test]
    public async Task SetExpectedResultsRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetExpectedResults(Content);

        await Assert.That(endpoint.SyncApi.SetExpectedResults(Content)).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetExpectedResultsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetExpectedResults("No endpoint value");
    }

    [Test]
    public async Task SetExpectedResultsAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetExpectedResultsAsync("No endpoint value");
    }

    [Test]
    public async Task SetExpectedResultsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetExpectedResultsAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetExpectedResultsAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetExpectedResultsAsync(Content);

        await Assert.That(endpoint.AsyncApi.SetExpectedResultsAsync(Content, CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetExpectedResultsAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetExpectedResultsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetExpectedResultsAsync(Content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetExpectedResultsAsyncWithTokenRoutedToEndpoint()
    {
        using var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetExpectedResultsAsync(Content, ts.Token);

        await Assert.That(endpoint.AsyncApi.SetExpectedResultsAsync(Content, ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetExpectedResultsAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetExpectedResultsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetExpectedResultsAsync(Content, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
