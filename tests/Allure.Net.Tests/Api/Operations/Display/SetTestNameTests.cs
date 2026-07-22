using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Display;

public class SetTestNameTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetTestNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetTestName("Foo");

        await Assert.That(endpoint.SyncApi.SetTestName("Foo")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetTestNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetTestName("No endpoint value");
    }

    [Test]
    public async Task SetTestNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetTestNameAsync("No endpoint value");
    }

    [Test]
    public async Task SetTestNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetTestNameAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetTestNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetTestNameAsync("Foo");

        await Assert.That(endpoint.AsyncApi.SetTestNameAsync("Foo", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetTestNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetTestNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetTestNameAsync("Foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetTestNameAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetTestNameAsync("Foo", ts.Token);

        await Assert.That(endpoint.AsyncApi.SetTestNameAsync("Foo", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetTestNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetTestNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetTestNameAsync("Foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
