using Allure.Net.Commons;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public class TestNodeStateTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldNotEmitTestResultForUnknownState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldNotEmitTestResultForDiscoveredState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new DiscoveredTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldNotEmitTestResultForInProgressNode()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new InProgressTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldEmitPassedTestResultForMessageWithPassedState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new PassedTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.passed);
        await Assert.That(testResult.statusDetails?.message).IsNull();
    }

    [Test]
    public async Task ShouldEmitSkippedTestResultForMessageWithSkippedState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new SkippedTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.skipped);
        await Assert.That(testResult.statusDetails?.message).IsNull();
    }

    [Test]
    public async Task ShouldEmitFailedTestResultForMessageWithFailedState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new FailedTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.failed);
        await Assert.That(testResult.statusDetails?.message).IsNull();
        await Assert.That(testResult.statusDetails?.trace).IsNull();
    }

    [Test]
    public async Task ShouldEmitBrokenTestResultForMessageWithTimeoutState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new TimeoutTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.broken);
        await Assert.That(testResult.statusDetails.message).IsEqualTo("The test has timed out.");
    }

    [Test]
    public async Task ShouldUseExplanationInTestDetailsForTimeoutStateIfGiven()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new TimeoutTestNodeStateProperty("Lorem Ipsum"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldUseProvidedStateExplanation()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new PassedTestNodeStateProperty("Lorem Ipsum"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldFillDetailsFromExceptionOfFailedNodeState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new FailedTestNodeStateProperty(new Exception("Lorem Ipsum")),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.statusDetails.trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldFallBackToExplanationIfFailedNodeStateMissException()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new FailedTestNodeStateProperty("Lorem Ipsum"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.statusDetails.trace).IsNull();
    }

    [Test]
    public async Task ShouldPreferExceptionForStatusDetailsOfFailedTest()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new FailedTestNodeStateProperty(new Exception("Lorem Ipsum"), "Dolor Sit Amet"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldPreferExceptionForStatusDetailsOfTimeOutedTest()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new TimeoutTestNodeStateProperty(new Exception("Lorem Ipsum"), "Dolor Sit Amet"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.statusDetails.trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldEmitBrokenTestResultForMessageWithErrorState()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new ErrorTestNodeStateProperty(),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.broken);
        await Assert.That(testResult.statusDetails.message).IsNull();
    }

    [Test]
    public async Task ShouldSetStatusDetailsMessageFromErrorStateExplanation()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new ErrorTestNodeStateProperty("Lorem Ipsum"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task ShouldPreferExceptionWhenSettingStatusDetailsForErrorTest()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new ErrorTestNodeStateProperty(new Exception("Lorem Ipsum"), "Dolor Sit Amen"),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.statusDetails.message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.statusDetails.trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldSetStatusToFailedIfExceptionMatchesFailException()
    {
        this.config.FailExceptions = ["System.Exception"];
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Properties = new([
                new ErrorTestNodeStateProperty(new Exception()),
            ]),
            Uid = "1"
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.failed);
    }
}
