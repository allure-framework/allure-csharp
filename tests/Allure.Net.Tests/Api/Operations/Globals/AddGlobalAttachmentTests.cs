using System.Text;
using Allure.Abstractions;
using TUnit.Assertions.Enums;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Globals;

public class AddGlobalAttachmentTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddGlobalAttachmentStreamRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachment("Attachment name", content);

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            (stream) => ReferenceEquals(stream, content),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentStreamDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync("Attachment name", content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncWithTokenRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync("Attachment name", content, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.SyncApi.AddGlobalAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) => actualBytes = ToBytes(stream)
        );

        AllureApi.AddGlobalAttachment("Attachment name", content);

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentMemoryDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content);
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryAsyncRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryAsyncWithTokenRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentTextAsyncRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddGlobalAttachmentTextAsyncWithTokenRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            (stream) => ReferenceEquals(stream, content),
            "text/example",
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentStreamWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            "text/example",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncWithTokenRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            "text/example",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMediaTypeRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.SyncApi.AddGlobalAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) => actualBytes = ToBytes(stream)
        );

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentMemoryWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMediaTypeAsyncRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMediaTypeAsyncWithTokenRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMediaTypeAsyncRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMediaTypeAsyncWithTokenRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            (stream) => ReferenceEquals(stream, content),
            "text/example",
            ".example"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentStreamWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            "text/example",
            ".example",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncWithTokenRoutedToGlobalEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            Is<Stream>((stream) => ReferenceEquals(stream, content)),
            "text/example",
            ".example",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddGlobalAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddGlobalAttachmentStreamWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMetadataRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.SyncApi.AddGlobalAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) => actualBytes = ToBytes(stream)
        );

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example"
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddGlobalAttachmentMemoryWithMetadataDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddGlobalAttachment("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMetadataAsyncRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMetadataAsyncWithTokenRoutedToGlobalEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        byte[] actualBytes = [];
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualBytes = ToBytes(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentMemoryWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMetadataAsyncRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMetadataAsyncWithTokenRoutedToGlobalEndpoint()
    {
        const string content = "attachment body";
        string? actualText = null;
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Global);
        endpoint.AsyncApi.AddGlobalAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _, _) => actualText = GetString(stream)
        );

        await AllureApi.AddGlobalAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddGlobalAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddGlobalAttachmentTextWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddGlobalAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );
    }

    static string? GetString(Stream? stream) =>
        stream is not null
            ? Encoding.UTF8.GetString(ToBytes(stream))
            : null;

    static byte[] ToBytes(Stream stream)
    {
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }
}
