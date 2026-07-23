using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddAttachmentFromFileTests : AllureApiTestsBase
{
    [Test]
    public async Task AddAttachmentFromFileByPathRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachmentFromFile("/tmp/report.json");

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentFromFileByPathDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachmentFromFile("/tmp/report.json");
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json");

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync("/tmp/report.json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json");
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileByPathAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentFromFileWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentFromFileWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentFromFileWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachmentFromFile("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddAttachmentFromFileAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentFromFileAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentFromFileWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentFromFileAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );
    }
}
