using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddFileAttachmentTests : AllureApiTestsBase
{
    [Test]
    public async Task AddFileAttachmentByPathRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddFileAttachment("/tmp/report.json");

        await Assert.That(endpoint.SyncApi.AddFileAttachment(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddFileAttachmentByPathDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddFileAttachment("/tmp/report.json");
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json");

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync("/tmp/report.json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json");
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync("/tmp/report.json", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentByPathAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", CancellationToken.None);
    }

    [Test]
    public async Task AddFileAttachmentWithNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.SyncApi.AddFileAttachment(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddFileAttachmentWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", CancellationToken.None);
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.SyncApi.AddFileAttachment(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddFileAttachmentWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.SyncApi.AddFileAttachment(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddFileAttachmentWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddFileAttachment("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddFileAttachmentWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );
    }
}
