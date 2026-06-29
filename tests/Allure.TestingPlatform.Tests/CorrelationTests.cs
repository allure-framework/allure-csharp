using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Sdk.Properties;

using AllureTestResult = Allure.Net.Commons.TestResult;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Tests;

public class CorrelationTests : DataConsumerTestsBase<CorrelationServiceStub, ThrowingLoggerStub>
{
    readonly TestNodeUpdateMessage session1Test1StopPassed = new(
        new("s1"),
        new()
        {
            DisplayName = "test 1",
            Uid = "test1",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        }
    );

    readonly TestNodeUpdateMessage session1Test2Start = new(
        new("s1"),
        new()
        {
            DisplayName = "test 2",
            Uid = "test2",
            Properties = new(
                new InProgressTestNodeStateProperty()
            ),
        }
    );

    readonly TestNodeUpdateMessage session1Test2StopPassed = new(
        new("s1"),
        new()
        {
            DisplayName = "test 2",
            Uid = "test2",
            Properties = new(
                new PassedTestNodeStateProperty()
            ),
        }
    );

    [Test]
    public async Task ShouldBufferMessageWithNoCorrelationUid()
    {
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldEmitMessageWithCorrelationUid()
    {
        this.correlationService.NextValues.Enqueue("c1");

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).HasSingleItem();
    }

    [Test]
    public async Task ShouldEmitMessagesAfterCorrelationEstablished()
    {
        this.correlationService.NextValues.Enqueue("c1");

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
    }

    [Test]
    public async Task ShouldProcessCorrelationMessagesWhenNoPendingSessionCorrelation()
    {
        var correlationUid = new CorrelationUid("c1");
        var startScope = new AllureScopeStartMessage(correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).HasSingleItem();
    }

    [Test]
    public async Task ShouldBufferCorrelationMessagesWhenSessionCorrelationIsPending()
    {
        var correlationUid = new CorrelationUid("c-pending");
        var startScope = new AllureScopeStartMessage(correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();
    }

    [Test]
    public async Task ShouldFlushBufferedCorrelationMessagesWhenSessionCorrelationIsEstablished()
    {
        var correlationUid = new CorrelationUid("c-flush");
        var startScope = new AllureScopeStartMessage(correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        this.correlationService.NextValues.Enqueue(correlationUid.Value);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).HasSingleItem();
    }

    [Test]
    public async Task ShouldIgnoreUnknownDataMessages()
    {
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, new UnknownDataMessage(), CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();
        await Assert.That(this.writer.TestContainers).IsEmpty();
    }

    [Test]
    public async Task ShouldReuseStoredSessionCorrelationWithoutConsumingNewValue()
    {
        var session2TestStopPassed = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "test 3",
                Uid = "test3",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        this.correlationService.NextValues.Enqueue("c1");
        this.correlationService.NextValues.Enqueue("c2");

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(3);
    }

    [Test]
    public async Task ShouldPreserveBufferedSessionMessageOrderWhenCorrelationBecomesAvailable()
    {
        var session1Test3StopPassed = new TestNodeUpdateMessage(
            new("s1"),
            new()
            {
                DisplayName = "test 3",
                Uid = "test3",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();

        this.correlationService.NextValues.Enqueue("c1");
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1Test3StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(3);
        await Assert.That(this.writer.TestResults[0].name).IsEqualTo("test 1");
        await Assert.That(this.writer.TestResults[1].name).IsEqualTo("test 2");
        await Assert.That(this.writer.TestResults[2].name).IsEqualTo("test 3");
    }

    [Test]
    public async Task ShouldProcessKnownCorrelationMessagesWhileOtherSessionsArePending()
    {
        var session2TestStopPassed = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "test s2",
                Uid = "test-s2",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        var knownCorrelationUid = new CorrelationUid("c-known");
        var startScope = new AllureScopeStartMessage(knownCorrelationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(knownCorrelationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(knownCorrelationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(knownCorrelationUid, new("scope-1"));

        this.correlationService.NextValues.Enqueue(knownCorrelationUid.Value);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("test 1");
    }

    [Test]
    public async Task ShouldNotFlushBufferedCorrelationMessagesForDifferentCorrelationUid()
    {
        var session2TestStopPassed = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "test s2",
                Uid = "test-s2",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        var unrelatedCorrelationUid = new CorrelationUid("c-unrelated");
        var startScope = new AllureScopeStartMessage(unrelatedCorrelationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(unrelatedCorrelationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(unrelatedCorrelationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(unrelatedCorrelationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        this.correlationService.NextValues.Enqueue("c-other");
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2TestStopPassed, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("test s2");
        await Assert.That(this.writer.TestContainers).IsEmpty();
    }

    [Test]
    public async Task ShouldDiscardBufferedSessionMessagesWhenSessionFinishes()
    {
        var sessionContext = new TestSessionContextStub
        {
            SessionUid = new("s1"),
        };

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);

        await this.consumer.OnTestSessionFinishingAsync(sessionContext);

        this.correlationService.NextValues.Enqueue("c-new");
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("test 2");
    }

    [Test]
    public async Task ShouldRemoveCorrelationFromActiveSetWhenSessionFinishes()
    {
        var finishedSessionContext = new TestSessionContextStub
        {
            SessionUid = new("s1"),
        };

        var pendingSessionMessage = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "pending s2",
                Uid = "pending-s2",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        var oldCorrelationUid = new CorrelationUid("c1");
        var startScope = new AllureScopeStartMessage(oldCorrelationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(oldCorrelationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(oldCorrelationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(oldCorrelationUid, new("scope-1"));

        this.correlationService.NextValues.Enqueue(oldCorrelationUid.Value);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1Test1StopPassed, CancellationToken.None);

        await this.consumer.OnTestSessionFinishingAsync(finishedSessionContext);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, pendingSessionMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();
    }

    [Test]
    public async Task ShouldKeepExistingCorrelationWhenFinishingUnknownSession()
    {
        var unknownSessionContext = new TestSessionContextStub
        {
            SessionUid = new("unknown"),
        };

        var pendingSessionMessage = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "pending s2",
                Uid = "pending-s2",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        var knownCorrelationUid = new CorrelationUid("c-known");
        var startScope = new AllureScopeStartMessage(knownCorrelationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(knownCorrelationUid, new("fixture-1"), new("scope-1"), "fixture");
        var stopFixture = new AllureFixtureStopMessage(knownCorrelationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(knownCorrelationUid, new("scope-1"));

        this.correlationService.NextValues.Enqueue(knownCorrelationUid.Value);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1Test1StopPassed, CancellationToken.None);

        await this.consumer.OnTestSessionFinishingAsync(unknownSessionContext);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, pendingSessionMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("test 1");
    }

    [Test]
    public async Task ShouldPreserveInterleavedSessionAndCorrelationBufferOrderWhenFlushing()
    {
        var sessionTriggerDiscovered = new TestNodeUpdateMessage(
            new("s1"),
            new()
            {
                DisplayName = "trigger",
                Uid = "trigger",
                Properties = new(
                    new DiscoveredTestNodeStateProperty()
                ),
            }
        );

        var correlationUid = new CorrelationUid("c-mixed-order");
        var testContextUid = new TestContextUid("test2");

        var updateStatus = new AllureTestUpdateMessage(correlationUid, testContextUid)
        {
            Properties = [new AllureStatusProperty<AllureTestResult>(Status.skipped)],
        };
        var updateName = new AllureTestUpdateMessage(correlationUid, testContextUid)
        {
            Properties = [new AllureNameProperty<AllureTestResult>("Foo")],
        };
        var addFeature = new AllureTestUpdateMessage(correlationUid, testContextUid)
        {
            Properties = [new AllureLabelsProperty([Label.Feature("Bar")])],
        };

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2Start, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateStatus, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateName, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, addFeature, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).IsEmpty();

        this.correlationService.NextValues.Enqueue(correlationUid.Value);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, sessionTriggerDiscovered, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.status).IsEqualTo(Status.skipped);
        await Assert.That(testResult.name).IsEqualTo("Foo");
        var feature = await Assert.That(testResult.labels).HasSingleItem((l) => l.name == "feature");
        await Assert.That(feature.value).IsEqualTo("Bar");
    }

    [Test]
    public async Task ShouldAllowReusingCorrelationUidAcrossSequentialSessions()
    {
        var finishedSessionContext = new TestSessionContextStub
        {
            SessionUid = new("s1"),
        };

        var sharedCorrelationUid = new CorrelationUid("c-shared");
        var test2ContextUid = new TestContextUid("test2");
        var updateTest2Name = new AllureTestUpdateMessage(sharedCorrelationUid, test2ContextUid)
        {
            Properties = [new AllureNameProperty<AllureTestResult>("Foo")],
        };
        var session2Test2StopPassed = new TestNodeUpdateMessage(
            new("s2"),
            new()
            {
                DisplayName = "test 2",
                Uid = "test2",
                Properties = new(
                    new PassedTestNodeStateProperty()
                ),
            }
        );

        this.correlationService.NextValues.Enqueue(sharedCorrelationUid.Value);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test2Start, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateTest2Name, CancellationToken.None);

        // session1Test2Start and updateTest2Name should be discarded
        await this.consumer.OnTestSessionFinishingAsync(finishedSessionContext);

        var testResult1 = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult1.name).IsEqualTo("test 1");

        this.correlationService.NextValues.Enqueue(sharedCorrelationUid.Value);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2Test2StopPassed, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(this.writer.TestResults[1].name).IsEqualTo("test 2");
    }

    [Test]
    public async Task ShouldDiscardActiveSessionWithDuplicatedCorrelation()
    {
        var sharedCorrelationUid = new CorrelationUid("c-shared");
        var session2Test2StopPassed = new TestNodeUpdateMessage(
            new("s2"),
                new()
                {
                    DisplayName = "test 2",
                    Uid = "test2",
                    Properties = new(
                        new PassedTestNodeStateProperty()
                    ),
                }
        );

        this.correlationService.NextValues.Enqueue(sharedCorrelationUid.Value);
        this.correlationService.NextValues.Enqueue(sharedCorrelationUid.Value);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, this.session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2Test2StopPassed, CancellationToken.None);

        var testResult1 = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult1.name).IsEqualTo("test 1");
    }

    sealed class UnknownDataMessage : IData
    {
        public string DisplayName => "unknown";

        public string Description => string.Empty;
    }
}
