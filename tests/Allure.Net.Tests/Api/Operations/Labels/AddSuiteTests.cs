using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddSuiteTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddSuiteRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddSuite("foo");

        await Assert.That(
            endpoint.SyncApi.AddLabel(
                (label) => label.Name == "suite" && label.Value == "foo"
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddSuiteDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddSuite("No endpoint value");
    }

    [Test]
    public async Task AddSuiteAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddSuiteAsync("No endpoint value");
    }

    [Test]
    public async Task AddSuiteAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddSuiteAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task AddSuiteAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddSuiteAsync("foo");

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "suite" && label.Value == "foo",
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddSuiteAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddSuiteAsync("foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddSuiteAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddSuiteAsync("foo", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "suite" && label.Value == "foo",
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddSuiteAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddSuiteAsync("foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
