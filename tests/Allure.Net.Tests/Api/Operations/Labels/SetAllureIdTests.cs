using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class SetAllureIdTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetAllureIdRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetAllureId(12345);

        await Assert.That(endpoint.SyncApi.SetLabel("ALLURE_ID", "12345")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetAllureIdAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetAllureIdAsync(12345);

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("ALLURE_ID", "12345", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetAllureIdAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetAllureIdAsync(12345);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetAllureIdAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetAllureIdAsync(12345, cts.Token);

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("ALLURE_ID", "12345", cts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetAllureIdAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetAllureIdAsync(12345, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
