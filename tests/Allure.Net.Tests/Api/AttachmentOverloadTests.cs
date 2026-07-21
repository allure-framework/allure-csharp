using System.Text;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class AttachmentOverloadTests
{
    [Test]
    public async Task SyncContentFamiliesPreserveBytesAndDefaults()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var stream = new MemoryStream([1, 2, 3]);

        AllureApi.AddAttachment("stream", stream);
        AllureApi.AddAttachment("memory", new ReadOnlyMemory<byte>([4, 5]));
        AllureApi.AddAttachment("text", "Hello");

        await AssertAttachment(recorder.Attachments[0], "stream", [1, 2, 3], null, "");
        await AssertAttachment(recorder.Attachments[1], "memory", [4, 5], null, "");
        await AssertAttachment(
            recorder.Attachments[2],
            "text",
            Encoding.UTF8.GetBytes("Hello"),
            null,
            ""
        );
    }

    [Test]
    public async Task SyncContentFamiliesPreserveExplicitMetadata()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var stream = new MemoryStream([1]);

        AllureApi.AddAttachment("stream", stream, "type/stream", ".stream");
        AllureApi.AddAttachment("memory", new ReadOnlyMemory<byte>([2]), "type/memory", ".memory");
        AllureApi.AddAttachment("text", "text", "type/text", ".text");

        await AssertAttachment(recorder.Attachments[0], "stream", [1], "type/stream", ".stream");
        await AssertAttachment(recorder.Attachments[1], "memory", [2], "type/memory", ".memory");
        await AssertAttachment(recorder.Attachments[2], "text", Encoding.UTF8.GetBytes("text"), "type/text", ".text");
    }

    [Test]
    public async Task AsyncContentFamiliesPreserveBytesMetadataAndToken()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();
        using var stream = new MemoryStream([1]);

        await AllureApi.AddAttachmentAsync("stream", stream, "type/stream", ".stream", cancellation.Token);
        await AllureApi.AddAttachmentAsync("memory", new ReadOnlyMemory<byte>([2]), "type/memory", ".memory", cancellation.Token);
        await AllureApi.AddAttachmentAsync("text", "text", "type/text", ".text", cancellation.Token);

        await Assert.That(recorder.Attachments.Select(item => item.Content!))
            .IsEquivalentTo(new byte[][] { [1], [2], Encoding.UTF8.GetBytes("text") });
        await Assert.That(recorder.Attachments.All(item => item.CancellationToken == cancellation.Token)).IsTrue();
        await Assert.That(recorder.Attachments.Select(item => item.FileExtension))
            .IsEquivalentTo(new[] { ".stream", ".memory", ".text" });
    }

    [Test]
    public async Task FileAttachmentDerivesNameAndPreservesExplicitMetadata()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        var path = Path.Combine("directory", "artifact.json");

        AllureApi.AddFileAttachment(path);
        AllureApi.AddFileAttachment(path, null!, "application/json", ".custom");

        await Assert.That(recorder.Attachments[0].Name).IsEqualTo("artifact.json");
        await Assert.That(recorder.Attachments[0].Path).IsEqualTo(path);
        await Assert.That(recorder.Attachments[0].MediaType).IsNull();
        await Assert.That(recorder.Attachments[0].FileExtension).IsEqualTo("");
        await Assert.That(recorder.Attachments[1].Name).IsEqualTo("artifact.json");
        await Assert.That(recorder.Attachments[1].MediaType).IsEqualTo("application/json");
        await Assert.That(recorder.Attachments[1].FileExtension).IsEqualTo(".custom");
    }

    [Test]
    public async Task AsyncFileAttachmentPreservesTokenAndArguments()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();
        var path = Path.Combine("directory", "artifact.bin");

        await AllureApi.AddFileAttachmentAsync(
            path,
            "display",
            "application/octet-stream",
            ".data",
            cancellation.Token
        );

        var attachment = recorder.Attachments.Single();
        await Assert.That(attachment.Operation).IsEqualTo("AddFileAttachmentAsync");
        await Assert.That(attachment.Name).IsEqualTo("display");
        await Assert.That(attachment.Path).IsEqualTo(path);
        await Assert.That(attachment.MediaType).IsEqualTo("application/octet-stream");
        await Assert.That(attachment.FileExtension).IsEqualTo(".data");
        await Assert.That(attachment.CancellationToken).IsEqualTo(cancellation.Token);
    }

    static async Task AssertAttachment(
        CapturedAttachment attachment,
        string name,
        byte[] content,
        string? mediaType,
        string extension
    )
    {
        await Assert.That(attachment.Name).IsEqualTo(name);
        await Assert.That(attachment.Content).IsEquivalentTo(content);
        await Assert.That(attachment.MediaType).IsEqualTo(mediaType);
        await Assert.That(attachment.FileExtension).IsEqualTo(extension);
    }
}
