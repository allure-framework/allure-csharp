using Allure.Model;
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
        await Assert.That(testResult.StatusDetails?.Message).IsNull();
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Skipped);
        await Assert.That(testResult.StatusDetails?.Message).IsNull();
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Failed);
        await Assert.That(testResult.StatusDetails?.Message).IsNull();
        await Assert.That(testResult.StatusDetails?.Trace).IsNull();
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Broken);
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("The test has timed out.");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.StatusDetails.Trace).Contains("System.Exception");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.StatusDetails.Trace).IsNull();
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.StatusDetails.Trace).Contains("System.Exception");
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Broken);
        await Assert.That(testResult.StatusDetails).IsNull();
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
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
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Lorem Ipsum");
        await Assert.That(testResult.StatusDetails.Trace).Contains("System.Exception");
    }
}
