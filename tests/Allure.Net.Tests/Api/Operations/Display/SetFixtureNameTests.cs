using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Display;

public class SetFixtureNameTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetFixtureNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetFixtureName("Foo");

        await Assert.That(endpoint.SyncApi.SetFixtureName("Foo")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetFixtureNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetFixtureNameAsync("Foo");

        await Assert.That(endpoint.AsyncApi.SetFixtureNameAsync("Foo", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetFixtureNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetFixtureNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetFixtureNameAsync("Foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetFixtureNameAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetFixtureNameAsync("Foo", ts.Token);

        await Assert.That(endpoint.AsyncApi.SetFixtureNameAsync("Foo", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetFixtureNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetFixtureNameAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetFixtureNameAsync("Foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}