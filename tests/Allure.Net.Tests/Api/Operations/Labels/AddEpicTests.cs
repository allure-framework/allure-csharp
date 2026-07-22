using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddEpicTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddEpicRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddEpic("foo");

        await Assert.That(
            endpoint.SyncApi.AddLabel(
                (label) => label.Name == "epic" && label.Value == "foo"
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddEpicDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddEpic("No endpoint value");
    }

    [Test]
    public async Task AddEpicAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddEpicAsync("No endpoint value");
    }

    [Test]
    public async Task AddEpicAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddEpicAsync("No endpoint value", CancellationToken.None);
    }

    [Test]
    public async Task AddEpicAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddEpicAsync("foo");

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "epic" && label.Value == "foo",
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddEpicAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddEpicAsync("foo");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddEpicAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddEpicAsync("foo", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLabelAsync(
                (label) => label.Name == "epic" && label.Value == "foo",
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddEpicAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddEpicAsync("foo", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
