using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddScreenDiffFromFilesTests : AllureApiTestsBase
{
    [Test]
    public async Task AddScreenDiffFromFilesRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddScreenDiffFromFiles("/tmp/expected.png", "/tmp/actual.png", "/tmp/diff.png");

        await Assert.That(endpoint.SyncApi.AddScreenDiffFromFiles(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddScreenDiffFromFilesDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddScreenDiffFromFiles("/tmp/expected.png", "/tmp/actual.png", "/tmp/diff.png");
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );

        await Assert.That(endpoint.AsyncApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddScreenDiffFromFilesAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png"
        );
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddScreenDiffFromFilesAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddScreenDiffFromFilesAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffFromFilesAsync(
            "/tmp/expected.png",
            "/tmp/actual.png",
            "/tmp/diff.png",
            CancellationToken.None
        );
    }
}
