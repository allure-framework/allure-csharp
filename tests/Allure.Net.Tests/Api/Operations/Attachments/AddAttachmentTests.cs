using System.Text;
using Allure.Abstractions;
using TUnit.Assertions.Enums;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddAttachmentTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddAttachmentStreamRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachment("Attachment name", content);

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            (s) => ReferenceEquals(s, content),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentStreamDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentStreamAsyncRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync("Attachment name", content);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentStreamAsyncWithTokenRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync("Attachment name", content, CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentMemoryRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content);

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentMemoryDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentMemoryAsyncRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentMemoryAsyncWithTokenRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentTextRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content);

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentTextDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentTextAsyncRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content);
    }

    [Test]
    public async Task AddAttachmentTextAsyncWithTokenRoutedToEndpoint()
    {
        const string content = "attachment body";
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachment("Attachment name", content, "text/example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            (s) => ReferenceEquals(s, content),
            "text/example",
            ""
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentStreamWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            "text/example",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncWithTokenRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            "text/example",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentMemoryWithMediaTypeRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content, "text/example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentMemoryWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentMemoryWithMediaTypeAsyncRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentMemoryWithMediaTypeAsyncWithTokenRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            "",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentTextWithMediaTypeRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content, "text/example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ""
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentTextWithMediaTypeDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentTextWithMediaTypeAsyncRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextWithMediaTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example");
    }

    [Test]
    public async Task AddAttachmentTextWithMediaTypeAsyncWithTokenRoutedToEndpoint()
    {
        const string content = "attachment body";
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextWithMediaTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", CancellationToken.None);
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            (s) => ReferenceEquals(s, content),
            "text/example",
            ".example"
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentStreamWithMetadataDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            "text/example",
            ".example",
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncWithTokenRoutedToEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            Is<Stream>((s) => ReferenceEquals(s, content)),
            "text/example",
            ".example",
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var content = new MemoryStream([1, 2, 3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddAttachmentStreamWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var content = new MemoryStream([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddAttachmentMemoryWithMetadataRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example"
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentMemoryWithMetadataDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentMemoryWithMetadataAsyncRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentMemoryWithMetadataAsyncWithTokenRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualBytes = [];
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualBytes = ToBytes(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example",
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualBytes).IsEquivalentTo(new byte[]{ 1, 2, 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddAttachmentMemoryWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> content = new([1, 2, 3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync(
            "Attachment name",
            content,
            "text/example",
            ".example",
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddAttachmentTextWithMetadataRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment name",
            IsNotNull<Stream>(),
            "text/example",
            ".example"
        )).WasCalled(Times.Once);
        await Assert.That(actualText).IsEqualTo(content);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddAttachmentTextWithMetadataDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        AllureApi.AddAttachment("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentTextWithMetadataAsyncRoutedToEndpoint()
    {
        const string content = "attachment body";
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextWithMetadataAsyncDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example");
    }

    [Test]
    public async Task AddAttachmentTextWithMetadataAsyncWithTokenRoutedToEndpoint()
    {
        const string content = "attachment body";
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        string? actualText = null;
        endpoint.AsyncApi.AddAttachmentAsync(Any(), Any(), Any(), Any(), Any()).Callback(
            (_, s, _, _, _) =>
            {
                actualText = GetString(s);
            }
        );

        await AllureApi.AddAttachmentAsync("Attachment name", content, "text/example", ".example", cts.Token);

        await Assert.That(endpoint.AsyncApi.AddAttachmentAsync(
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
    public async Task AddAttachmentTextWithMetadataAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        const string content = "attachment body";
        using var _ = InstallNoEndpoint();

        await AllureApi.AddAttachmentAsync(
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

    static byte[] ToBytes(Stream s)
    {
        using var memoryStream = new MemoryStream();
        s.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
