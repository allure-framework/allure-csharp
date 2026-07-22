using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddParentSuiteTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddParentSuiteRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddParentSuite("foo");

        await Assert.That(
            endpoint.SyncApi.AddLabel(
                (label) => label.Name == "parentSuite" && label.Value == "foo"
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParentSuiteAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddParentSuiteAsync("foo");

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "parentSuite" && label.Value == "foo",
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParentSuiteAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddParentSuiteAsync("foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddParentSuiteAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddParentSuiteAsync("foo", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "parentSuite" && label.Value == "foo",
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddParentSuiteAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddParentSuiteAsync("foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
