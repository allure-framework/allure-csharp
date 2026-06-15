using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using System.Text.RegularExpressions;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Tests;

public partial class DefaultPropertyTests : DataConsumerTestsBase
{
    [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
    private static partial Regex UuidPattern();

    static readonly Regex uuidPattern = UuidPattern();

    [Test]
    public async Task ShouldSetUuid()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.uuid).Matches(uuidPattern);
    }

    [Test]
    public async Task ShouldSetStage()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.stage).IsEqualTo(Stage.finished);
    }

    [Test]
    public async Task ShouldSetName()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("Foo");
    }

    [Test]
    public async Task ShouldSetFullName()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.fullName).IsEqualTo("1");
    }

    [Test]
    public async Task ShouldSetTimestampsWithoutInProgressMessage()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.start).IsBetween(before, after);
        await Assert.That(testResult.stop).IsBetween(before, after);
        await Assert.That(testResult.stop).IsGreaterThanOrEqualTo(testResult.start);
    }

    [Test]
    public async Task ShouldSetTimestampsWithInProgressMessage()
    {
        var session = new SessionUid("Bar");
        var testNodeInProgress = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new InProgressTestNodeStateProperty()
            )
        };
        var testNodePassed = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var messageInProgress = new TestNodeUpdateMessage(session, testNodeInProgress);
        var messagePassed = new TestNodeUpdateMessage(session, testNodePassed);

        var beforeStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, messageInProgress, CancellationToken.None);
        var afterStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        SpinWait.SpinUntil(static () => false, 1);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, messagePassed, CancellationToken.None);
        var afterStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.start).IsBetween(beforeStart, afterStart);
        await Assert.That(testResult.stop).IsBetween(afterStart, afterStop);
    }

    [Test]
    public async Task DefaultTitlePathMustBeEmpty()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.titlePath).IsEmpty();
    }

    [Test]
    public async Task ShouldAddDefaultLabels()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.labels).Contains(
            l => l.name == "host" && l.value == Environment.MachineName
        ).And.Contains(
            l => l.name == "language" && l.value == "C#"
        );
    }
}
