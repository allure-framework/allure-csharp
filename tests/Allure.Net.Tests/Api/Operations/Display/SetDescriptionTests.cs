using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Display;

public class SetDescriptionTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetDescriptionRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetDescription("Foo");

        await Assert.That(endpoint.SyncApi.SetDescription("Foo")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetDescriptionAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetDescriptionAsync("Foo");

        await Assert.That(endpoint.AsyncApi.SetDescriptionAsync("Foo", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetDescriptionAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetDescriptionAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetDescriptionAsync("Foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetDescriptionAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetDescriptionAsync("Foo", ts.Token);

        await Assert.That(endpoint.AsyncApi.SetDescriptionAsync("Foo", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetDescriptionAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetDescriptionAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetDescriptionAsync("Foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}