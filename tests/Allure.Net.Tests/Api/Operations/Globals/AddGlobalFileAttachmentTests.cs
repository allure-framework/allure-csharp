using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Globals;

public class AddGlobalAttachmentFromFileTests : AllureApiTestsBase
{
    [Test]
    public async Task AddGlobalAttachmentFromFileByPathRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachmentFromFile(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentFromFileByPathDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileByPathAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentFromFileWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentFromFileWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentFromFileWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachmentFromFile("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );
    }

}
