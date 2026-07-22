using Allure.Abstractions;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Globals;

public class AddGlobalFileAttachmentTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddGlobalFileAttachmentByPathRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalFileAttachment("/tmp/report.json");

        await Assert.That(endpoint.SyncApi.AddGlobalFileAttachment(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalFileAttachmentByPathDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalFileAttachment("/tmp/report.json");
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json");

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json");
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "report.json",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentByPathAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.SyncApi.AddGlobalFileAttachment(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalFileAttachmentWithNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report");

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report");
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            IsNull<string?>(),
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.SyncApi.AddGlobalFileAttachment(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalFileAttachmentWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json");
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync("/tmp/report.json", "JSON report", "application/json", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report", "application/json", ".allure-json");

        await Assert.That(endpoint.SyncApi.AddGlobalFileAttachment(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalFileAttachmentWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalFileAttachment("/tmp/report.json", "JSON report", "application/json", ".allure-json");
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncRoutedToGlobalEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json"
        );
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncWithTokenRoutedToGlobalEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            cts.Token
        );

        await Assert.That(endpoint.AsyncApi.AddGlobalFileAttachmentAsync(
            "JSON report",
            "/tmp/report.json",
            "application/json",
            ".allure-json",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalFileAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalFileAttachmentWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalFileAttachmentAsync(
            "/tmp/report.json",
            "JSON report",
            "application/json",
            ".allure-json",
            CancellationToken.None
        );
    }

}
