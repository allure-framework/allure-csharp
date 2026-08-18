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
        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("Foo");
        await Assert.That(attachment.Type).IsNull();
        await Assert.That(attachment.Source).IsNotEmpty();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.Source);
        await Assert.That(this.writer.FileAttachments[attachment.Source]).IsEqualTo(
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
        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("Standard output");
        await Assert.That(attachment.Type).IsEqualTo("text/plain");
        await Assert.That(attachment.Source).EndsWith("-attachment.txt");
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(this.writer.ByteAttachments[attachment.Source]).IsEquivalentTo(
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
        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("Standard error");
        await Assert.That(attachment.Type).IsEqualTo("text/plain");
        await Assert.That(attachment.Source).EndsWith("-attachment.txt");
        await Assert.That(this.writer.ByteAttachments).ContainsKey(attachment.Source);
        await Assert.That(this.writer.ByteAttachments[attachment.Source]).IsEquivalentTo(
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
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("Foo");
        await Assert.That(attachment.Type).IsNull();
        await Assert.That(attachment.Source).IsNotEmpty();
        await Assert.That(this.writer.FileAttachments).ContainsKey(attachment.Source);
        await Assert.That(this.writer.FileAttachments[attachment.Source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }

    [Test]
    public async Task ShouldUseFileNameAndExtensionForUnnamedFileArtifact()
    {
        var path = Path.Combine(".", "foo", "artifact.log");
        var message = new TestNodeUpdateMessage(
            new SessionUid("Bar"),
            TestNodeWith(
                new PassedTestNodeStateProperty(),
                new FileArtifactProperty(
                    fileInfo: new(path),
                    displayName: null
                )
            )
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachment = await Assert.That(testResult.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("artifact.log");
        await Assert.That(attachment.Source).EndsWith(
            $"-attachment.log"
        );
        await Assert.That(this.writer.FileAttachments[attachment.Source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }

    [Test]
    public async Task ShouldUseFileNameExtensionAndTimestampForUnnamedSessionFileArtifact()
    {
        var path = Path.Combine(".", "foo", "session-output.json");
        var message = new SessionFileArtifact(new SessionUid("Bar"), new(path), null);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var global = await Assert.That(this.writer.Globals).HasSingleItem();
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("session-output.json");
        await Assert.That(attachment.Source).EndsWith(
            $"-attachment.json"
        );
        await Assert.That(attachment.Timestamp).IsGreaterThanOrEqualTo(0);
        await Assert.That(this.writer.FileAttachments[attachment.Source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }

    [Test]
    public async Task ShouldAttachStdoutStderrAndFileArtifactToSameTest()
    {
        var path = Path.Combine(".", "foo", "details.txt");
        var message = new TestNodeUpdateMessage(
            new SessionUid("Bar"),
            TestNodeWith(
                new PassedTestNodeStateProperty(),
                new StandardOutputProperty("out"),
                new StandardErrorProperty("err"),
                new FileArtifactProperty(
                    fileInfo: new(path),
                    displayName: "details"
                )
            )
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachments = testResult.Attachments;
        await Assert.That(attachments).Count().IsEqualTo(3);
        await Assert.That(attachments.Select(static attachment => attachment.Source).Distinct())
            .Count().IsEqualTo(3);

        var stdout = await Assert.That(attachments)
            .HasSingleItem(static (item) => item.Name == "Standard output");
        var stderr = await Assert.That(attachments)
            .HasSingleItem(static (item) => item.Name == "Standard error");
        var file = await Assert.That(attachments)
            .HasSingleItem(static (item) => item.Name == "details");

        await Assert.That(this.writer.ByteAttachments[stdout.Source]).IsEquivalentTo(
            "out"u8.ToArray(),
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
        await Assert.That(this.writer.ByteAttachments[stderr.Source]).IsEquivalentTo(
            "err"u8.ToArray(),
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
        await Assert.That(this.writer.FileAttachments[file.Source]).IsEqualTo(
            Path.GetFullPath(path)
        );
    }

    [Test]
    public async Task ShouldAttachEmptyStandardOutputAndError()
    {
        var message = new TestNodeUpdateMessage(
            new SessionUid("Bar"),
            TestNodeWith(
                new PassedTestNodeStateProperty(),
                new StandardOutputProperty(""),
                new StandardErrorProperty("")
            )
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var attachments = testResult.Attachments;
        await Assert.That(attachments).Count().IsEqualTo(2);

        var stdout = await Assert.That(attachments)
            .HasSingleItem(static (item) => item.Name == "Standard output");
        var stderr = await Assert.That(attachments)
            .HasSingleItem(static (item) => item.Name == "Standard error");

        await Assert.That(this.writer.ByteAttachments[stdout.Source]).IsEmpty();
        await Assert.That(this.writer.ByteAttachments[stderr.Source]).IsEmpty();
    }

    static TestNode TestNodeWith(params IProperty[] properties) => new()
    {
        DisplayName = "Foo",
        Uid = "1",
        Properties = new(properties),
    };
}
