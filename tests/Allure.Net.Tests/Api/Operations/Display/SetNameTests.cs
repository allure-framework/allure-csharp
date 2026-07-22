using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Display;

public class SetNameTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetName("Foo");

        await Assert.That(endpoint.SyncApi.SetName("Foo")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetName("No endpoint value");
    }

    [Test]
    public async Task SetNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetNameAsync("No endpoint value");
    }

    [Test]
    public async Task SetNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetNameAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task SetNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetNameAsync("Foo");

        await Assert.That(endpoint.AsyncApi.SetNameAsync("Foo", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetNameAsync("Foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetNameAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetNameAsync("Foo", ts.Token);

        await Assert.That(endpoint.AsyncApi.SetNameAsync("Foo", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetNameAsync("Foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
