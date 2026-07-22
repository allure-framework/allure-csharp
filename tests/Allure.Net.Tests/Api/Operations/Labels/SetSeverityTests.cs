using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class SetSeverityTests : ApiOperationTestsBase
{
    [Test]
    public async Task SetSeverityRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.SetSeverity(Severity.Critical);

        await Assert.That(endpoint.SyncApi.SetLabel("severity", "critical")).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void SetSeverityDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.SetSeverity(Severity.Normal);
    }

    [Test]
    public async Task SetSeverityAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetSeverityAsync(Severity.Normal);
    }

    [Test]
    public async Task SetSeverityAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.SetSeverityAsync(Severity.Normal, CancellationToken.None);
    }

    [Test]
    public async Task SetSeverityAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetSeverityAsync(Severity.Trivial);

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("severity", "trivial", CancellationToken.None)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetSeverityAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetSeverityAsync(Severity.Minor);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task SetSeverityAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.SetSeverityAsync(Severity.Blocker, ts.Token);

        await Assert.That(endpoint.AsyncApi.SetLabelAsync("severity", "blocker", ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SetSeverityAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetLabelAsync(Any(), Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.SetSeverityAsync(Severity.Critical, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
