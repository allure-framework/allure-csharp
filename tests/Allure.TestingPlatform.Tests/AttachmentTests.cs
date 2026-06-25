using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public class AttachmentTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldAttachFileArtifacts()
    {
        var path = Path.Combine(".", "foo", "bar");
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new FileArtifactProperty(
                    fileInfo: new(path),
                    displayName: "Foo"
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.name).IsEqualTo("Foo");
        await Assert.That(attachment.type).IsNull();
        await Assert.That(attachment.source).IsNotEmpty();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.source);
        await Assert.That(this.writer.FileAttachments[attachment.source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }

    [Test]
    public async Task ShouldAttachStandardOutput()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new StandardOutputProperty("Lorem Ipsum")
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.name).IsEqualTo("Standard output");
        await Assert.That(attachment.type).IsEqualTo("text/plain");
        await Assert.That(attachment.source).EndsWith("-attachment.txt");
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.source);
        await Assert.That(this.writer.ByteAttachments[attachment.source]).IsEquivalentTo(
            "Lorem Ipsum"u8.ToArray(),
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task ShouldAttachStandardError()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new StandardErrorProperty("Lorem Ipsum")
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachment = await Assert.That(testResult.attachments).HasSingleItem();
        await Assert.That(attachment.name).IsEqualTo("Standard error");
        await Assert.That(attachment.type).IsEqualTo("text/plain");
        await Assert.That(attachment.source).EndsWith("-attachment.txt");
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.source);
        await Assert.That(this.writer.ByteAttachments[attachment.source]).IsEquivalentTo(
            "Lorem Ipsum"u8.ToArray(),
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task ShouldAttachSessionFileArtifact()
    {
        var path = Path.Combine(".", "foo", "bar");
        var message = new SessionFileArtifact(new SessionUid("Bar"), new(path), "Foo");

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var global = await Assert.That(this.writer.Globals).HasSingleItem();
        var attachment = await Assert.That(global.attachments).HasSingleItem();
        await Assert.That(attachment.name).IsEqualTo("Foo");
        await Assert.That(attachment.type).IsNull();
        await Assert.That(attachment.source).IsNotEmpty();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.source);
        await Assert.That(this.writer.FileAttachments[attachment.source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }
}
