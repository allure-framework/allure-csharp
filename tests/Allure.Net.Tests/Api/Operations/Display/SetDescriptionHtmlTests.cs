using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Display;

public class SetDescriptionHtmlTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetDescriptionHtmlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetDescriptionHtml("Foo");

        await Assert.That(endpoint.SyncApi.SetDescriptionHtml("Foo")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetDescriptionHtmlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetDescriptionHtml("No endpoint value");
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetDescriptionHtmlAsync("No endpoint value");
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetDescriptionHtmlAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetDescriptionHtmlAsync("Foo");

        await Assert.That(endpoint.AsyncApi.SetDescriptionHtmlAsync("Foo", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetDescriptionHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetDescriptionHtmlAsync("Foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetDescriptionHtmlAsync("Foo", ts.Token);

        await Assert.That(endpoint.AsyncApi.SetDescriptionHtmlAsync("Foo", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetDescriptionHtmlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetDescriptionHtmlAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetDescriptionHtmlAsync("Foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
