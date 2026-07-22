using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddFileScreenDiffTests : AllureApiTestsBase
{
    [Test]
    public async Task AddFileScreenDiffRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddFileScreenDiff("/tmp/expected.png", "/tmp/actual.png", "/tmp/diff.png");

        await Assert.That(endpoint.SyncApi.AddFileScreenDiff(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddFileScreenDiffDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddFileScreenDiff("/tmp/expected.png", "/tmp/actual.png", "/tmp/diff.png");
    }

    [Test]
    public async Task AddFileScreenDiffAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );

        await Assert.That(endpoint.AsyncApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileScreenDiffAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileScreenDiffAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileScreenDiffAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );
    }

    [Test]
    public async Task AddFileScreenDiffAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileScreenDiffAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileScreenDiffAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileScreenDiffAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileScreenDiffAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        );
    }
}
