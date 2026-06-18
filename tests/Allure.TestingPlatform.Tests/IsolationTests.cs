using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests;

public partial class IsolationTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldIsolateTestResultsByUid()
    {
        var session = new SessionUid("Bar");
        var testNode1InProgress = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new InProgressTestNodeStateProperty()
            )
        };
        var testNode2InProgress = new TestNode
        {
            DisplayName = "Bar",
            Uid = "2",
            Properties = new(
                new InProgressTestNodeStateProperty()
            )
        };
        var testNode1Passed = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };
        var testNode2Passed = new TestNode
        {
            DisplayName = "Bar",
            Uid = "2",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message1InProgress = new TestNodeUpdateMessage(session, testNode1InProgress);
        var message2InProgress = new TestNodeUpdateMessage(session, testNode2InProgress);
        var message1Passed = new TestNodeUpdateMessage(session, testNode1Passed);
        var message2Passed = new TestNodeUpdateMessage(session, testNode2Passed);

        var before1Start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        SpinWait.SpinUntil(static () => false, 1);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message1InProgress, CancellationToken.None);
        var before2Start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        SpinWait.SpinUntil(static () => false, 1);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message2InProgress, CancellationToken.None);
        var beforeStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        SpinWait.SpinUntil(static () => false, 1);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message1Passed, CancellationToken.None);
        var after1Stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        SpinWait.SpinUntil(static () => false, 1);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message2Passed, CancellationToken.None);
        SpinWait.SpinUntil(static () => false, 1);
        var after2Stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        var testResult1 = this.writer.TestResults[0];
        var testResult2 = this.writer.TestResults[1];
        await Assert.That(testResult1.start).IsBetween(before1Start, before2Start);
        await Assert.That(testResult2.start).IsBetween(before2Start, beforeStop);
        await Assert.That(testResult1.stop).IsBetween(beforeStop, after1Stop);
        await Assert.That(testResult2.stop).IsBetween(after1Stop, after2Stop);
    }

    [Test]
    public async Task ShouldIsolateContextsBySessions()
    {
        var session1 = new SessionUid("session-1");
        var session2 = new SessionUid("session-2");
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
        var testNodeFailed = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new FailedTestNodeStateProperty()
            )
        };

        var session1TestStart = new TestNodeUpdateMessage(session1, testNodeInProgress);
        var session2TestStart = new TestNodeUpdateMessage(session2, testNodeInProgress);
        var session1TestStop = new TestNodeUpdateMessage(session1, testNodePassed);
        var session2TestStop = new TestNodeUpdateMessage(session2, testNodeFailed);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStop, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);

        var testResult1 = this.writer.TestResults[0];
        var testResult2 = this.writer.TestResults[1];

        await Assert.That(testResult1.status).IsEqualTo(Net.Commons.Status.passed);
        await Assert.That(testResult2.status).IsEqualTo(Net.Commons.Status.failed);
    }

    [Test]
    public async Task ShouldIsolateUidSharingBySessions()
    {
        var session1 = new SessionUid("session-1");
        var correlationUid1 = new CorrelationUid("session-1");
        var session2 = new SessionUid("session-2");
        var correlationUid2 = new CorrelationUid("session-2");
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
        var testNodeFailed = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new FailedTestNodeStateProperty()
            )
        };

        var session1ScopeStart = new AllureScopeStartMessage(correlationUid1, new("1"));
        var session2ScopeStart = new AllureScopeStartMessage(correlationUid2, new("1"));
        var session1FixtureStart = new AllureBeforeFixtureStartMessage(correlationUid1, new("3"), new("1"), "Foo");
        var session2FixtureStart = new AllureBeforeFixtureStartMessage(correlationUid2, new("3"), new("1"), "Foo");
        var session1FixtureStop = new AllureFixtureStopMessage(correlationUid1, new("3"));
        var session2FixtureStop = new AllureFixtureStopMessage(correlationUid2, new("3"));
        var session1TestStart = new TestNodeUpdateMessage(session1, testNodeInProgress);
        var session2TestStart = new TestNodeUpdateMessage(session2, testNodeInProgress);
        var session1TestStop = new TestNodeUpdateMessage(session1, testNodePassed);
        var session2TestStop = new TestNodeUpdateMessage(session2, testNodeFailed);
        var session1ScopeStop = new AllureScopeStopMessage(correlationUid1, new("1"));
        var session2ScopeStop = new AllureScopeStopMessage(correlationUid2, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1ScopeStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2ScopeStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1FixtureStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2FixtureStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1FixtureStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2FixtureStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1ScopeStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2ScopeStop, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(2);

        var testResult1 = this.writer.TestResults[0];
        var testResult2 = this.writer.TestResults[1];
        var container1 = this.writer.TestContainers[0];
        var container2 = this.writer.TestContainers[1];

        await Assert.That(container1.children).IsEquivalentTo([testResult1.uuid]);
        await Assert.That(container2.children).IsEquivalentTo([testResult2.uuid]);
    }

    [Test]
    public async Task ShouldIsolateTestScopesBySession()
    {
        var session1 = new SessionUid("session-1");
        var correlationUid1 = new CorrelationUid("session-1");
        var session2 = new SessionUid("session-2");
        var correlationUid2 = new CorrelationUid("session-2");
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
        var testNodeFailed = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new FailedTestNodeStateProperty()
            )
        };

        var session1ScopeStart = new AllureScopeStartMessage(correlationUid1, new("2"));
        var session2ScopeStart = new AllureScopeStartMessage(correlationUid2, new("2"));
        var session1FixtureStart = new AllureBeforeFixtureStartMessage(correlationUid1, new("3"), new("2"), "Foo");
        var session2FixtureStart = new AllureBeforeFixtureStartMessage(correlationUid2, new("3"), new("2"), "Foo");
        var session1FixtureStop = new AllureFixtureStopMessage(correlationUid1, new("3"));
        var session2FixtureStop = new AllureFixtureStopMessage(correlationUid2, new("3"));
        var session1TestsInScope = new AllureTestsScopeMessage(correlationUid1, new("2"), [new("1")]);
        var session2TestsInScope = new AllureTestsScopeMessage(correlationUid2, new("2"), [new("1")]);
        var session1TestStart = new TestNodeUpdateMessage(session1, testNodeInProgress);
        var session2TestStart = new TestNodeUpdateMessage(session2, testNodeInProgress);
        var session1TestStop = new TestNodeUpdateMessage(session1, testNodePassed);
        var session2TestStop = new TestNodeUpdateMessage(session2, testNodeFailed);
        var session1ScopeStop = new AllureScopeStopMessage(correlationUid1, new("2"));
        var session2ScopeStop = new AllureScopeStopMessage(correlationUid2, new("2"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1ScopeStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2ScopeStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1FixtureStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2FixtureStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1FixtureStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2FixtureStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStart, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1TestStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1ScopeStop, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2ScopeStop, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(2);

        var testResult1 = this.writer.TestResults[0];
        var testResult2 = this.writer.TestResults[1];
        var container1 = this.writer.TestContainers[0];
        var container2 = this.writer.TestContainers[1];

        await Assert.That(container1.children).IsEquivalentTo([testResult1.uuid]);
        await Assert.That(container2.children).IsEquivalentTo([testResult2.uuid]);
    }
}
