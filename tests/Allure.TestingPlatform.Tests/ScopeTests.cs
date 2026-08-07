using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.TestHost;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Tests;

public class ScopeTests : DataConsumerTestsBase
{
    readonly SessionUid sessionUid = new("Bar");
    readonly CorrelationUid correlationUid = new("Bar");

    [Test]
    public async Task ShouldEmitContainerOnScopeStop()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).HasSingleItem();
    }

    [Test]
    public async Task ShouldAssociateSingleTestMessageTestWithScopeByUid()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "1",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var child = await Assert.That(container.Children).HasSingleItem();
        await Assert.That(child).IsEqualTo(testResult.Uuid);
    }

    [Test]
    public async Task ShouldAssociateTestMessagePairWithScopeByUid()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeInProgressMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "1",
            DisplayName = "Node",
            Properties = new(
                new InProgressTestNodeStateProperty()
            ),
        });
        var testNodePassedMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "1",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeInProgressMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodePassedMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var child = await Assert.That(container.Children).HasSingleItem();
        await Assert.That(child).IsEqualTo(testResult.Uuid);
    }

    [Test]
    public async Task ShouldConsumeExplicitScopeAssociationOnSingleMessageTestStart()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var testsInScope = new AllureScopeTestsMessage(correlationUid, new("1"), [new("3"), new("4"), new("5")]);
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "3",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var child = await Assert.That(container.Children).HasSingleItem();
        await Assert.That(child).IsEqualTo(testResult.Uuid);
    }

    [Test]
    public async Task ShouldConsumeExplicitScopeAssociationOnDoubleMessageTestStart()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var testsInScope = new AllureScopeTestsMessage(correlationUid, new("1"), [new("3"), new("4"), new("5")]);
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeInProgressMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "1",
            DisplayName = "Node",
            Properties = new(
                new InProgressTestNodeStateProperty()
            ),
        });
        var testNodePassedMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "1",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeInProgressMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodePassedMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var child = await Assert.That(container.Children).HasSingleItem();
        await Assert.That(child).IsEqualTo(testResult.Uuid);
    }

    [Test]
    public async Task ShouldKeepTestScopeAssociationActiveAfterTestWritten()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var testsInScope = new AllureScopeTestsMessage(correlationUid, new("1"), [new("3"), new("4"), new("5")]);
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "3",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(container.Children).IsEquivalentTo(
            this.writer.TestResults.Select(static tr => tr.Uuid)
        );
    }

    [Test]
    public async Task ShouldRemoveTestScopeAssociationAfterScopeStop()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var testsInScope = new AllureScopeTestsMessage(correlationUid, new("1"), [new("3"), new("4"), new("5")]);
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var testNodeMessage = new TestNodeUpdateMessage(sessionUid, new()
        {
            Uid = "3",
            DisplayName = "Node",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        });
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        // empty scope not written
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(container.Children).IsEmpty();
    }
}