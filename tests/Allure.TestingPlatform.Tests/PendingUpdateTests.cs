using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

using AllureTestResult = Allure.Net.Commons.TestResult;

namespace Allure.TestingPlatform.Tests;

public class PendingUpdateTests : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");
    readonly SessionUid sessionUid = new("Bar");

    [Test]
    public async Task ShouldApplyPendingFixtureStartWhenScopeBecomesAvailable()
    {
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Pending fixture"
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Pending fixture");
    }

    [Test]
    public async Task ShouldApplyPendingFixtureUpdateWhenFixtureStartsLater()
    {
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var updateFixture = new AllureFixtureUpdateMessage(this.correlationUid, new("fixture-1"))
        {
            Properties = [new AllureNameProperty<FixtureResult>("Updated fixture")],
        };
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Initial fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Updated fixture");
    }

    [Test]
    public async Task ShouldPreservePendingFixtureUpdateOrderWhenFixtureStartsLater()
    {
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var updateFixture1 = new AllureFixtureUpdateMessage(this.correlationUid, new("fixture-1"))
        {
            Properties = [new AllureDescriptionProperty<FixtureResult>("First")],
        };
        var updateFixture2 = new AllureFixtureUpdateMessage(this.correlationUid, new("fixture-1"))
        {
            Properties = [ new AllureDescriptionProperty<FixtureResult>("Second") { Append = true } ],
        };
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Ordered fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixture1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixture2, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.description).IsEqualTo("First\n\nSecond");
    }

    [Test]
    public async Task ShouldReplayPendingFixtureStartAndUpdatesWhenScopeBecomesAvailable()
    {
        var updateFixture = new AllureFixtureUpdateMessage(this.correlationUid, new("fixture-1"))
        {
            Properties = [new AllureNameProperty<FixtureResult>("Updated pending fixture")],
        };
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Initial fixture"
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Updated pending fixture");
    }

    [Test]
    public async Task ShouldReplayEntireFixtureWhenScopeBecomesAvailable()
    {
        var updateFixture = new AllureFixtureUpdateMessage(this.correlationUid, new("fixture-1"))
        {
            Properties = [new AllureNameProperty<FixtureResult>("Updated pending fixture")],
        };
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Initial fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Updated pending fixture");
    }

    [Test]
    public async Task ShouldEmitParentAndChildContainersWhenParentScopeBecomesAvailable()
    {
        var startChildScope = new AllureScopeStartMessage(
            this.correlationUid,
            new("child-scope"),
            new("parent-scope")
        );
        var startChildFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("child-fixture"),
            new("child-scope"),
            "Child fixture"
        );
        var startParentScope = new AllureScopeStartMessage(this.correlationUid, new("parent-scope"));
        var startParentFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("parent-fixture"),
            new("parent-scope"),
            "Parent fixture"
        );
        var stopChildFixture = new AllureFixtureStopMessage(this.correlationUid, new("child-fixture"));
        var stopParentFixture = new AllureFixtureStopMessage(this.correlationUid, new("parent-fixture"));
        var stopChildScope = new AllureScopeStopMessage(this.correlationUid, new("child-scope"));
        var stopParentScope = new AllureScopeStopMessage(this.correlationUid, new("parent-scope"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(2);

        var childContainer = this.writer.TestContainers[0];
        var parentContainer = this.writer.TestContainers[1];

        var childFixture = await Assert.That(childContainer.befores).HasSingleItem();
        var parentFixture = await Assert.That(parentContainer.befores).HasSingleItem();

        await Assert.That(childFixture.name).IsEqualTo("Child fixture");
        await Assert.That(parentFixture.name).IsEqualTo("Parent fixture");
    }

    [Test]
    public async Task ShouldApplyPendingTestsScopeAssociationWhenScopeBecomesAvailable()
    {
        var testsInScope = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1")]
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var testNode = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Associated test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNode, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var child = await Assert.That(container.children).HasSingleItem();

        await Assert.That(fixture.name).IsEqualTo("Scope fixture");
        await Assert.That(child).IsEqualTo(testResult.uuid);
    }

    [Test]
    public async Task ShouldApplyPendingTestUpdateWhenTestResultIsCreatedFromSingleMessage()
    {
        var updateTest = new AllureTestUpdateMessage(this.correlationUid, new("test-1"))
        {
            Properties = [new AllureNameProperty<AllureTestResult>("Updated test name")],
        };
        var testNodeMessage = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Initial test name",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateTest, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeMessage, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("Updated test name");
    }

    [Test]
    public async Task ShouldApplyPendingTestsScopeAssociationWhenChildScopeWaitsForParent()
    {
        var testsInChildScope = new AllureTestsScopeMessage(
            this.correlationUid,
            new("child-scope"),
            [new("test-1")]
        );
        var startChildScope = new AllureScopeStartMessage(
            this.correlationUid,
            new("child-scope"),
            new("parent-scope")
        );
        var startChildFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("child-fixture"),
            new("child-scope"),
            "Child scope fixture"
        );
        var startParentScope = new AllureScopeStartMessage(this.correlationUid, new("parent-scope"));
        var stopChildFixture = new AllureFixtureStopMessage(this.correlationUid, new("child-fixture"));
        var testNode = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Associated child test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopChildScope = new AllureScopeStopMessage(this.correlationUid, new("child-scope"));
        var startParentFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("parent-fixture"),
            new("parent-scope"),
            "Parent scope fixture"
        );
        var stopParentFixture = new AllureFixtureStopMessage(this.correlationUid, new("parent-fixture"));
        var stopParentScope = new AllureScopeStopMessage(this.correlationUid, new("parent-scope"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNode, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(2);

        var childContainer = this.writer.TestContainers[0];
        var parentContainer = this.writer.TestContainers[1];

        var childFixture = await Assert.That(childContainer.befores).HasSingleItem();
        var parentFixture = await Assert.That(parentContainer.befores).HasSingleItem();

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        var childScopeChild = await Assert.That(childContainer.children).HasSingleItem();
        var parentScopeChild = await Assert.That(parentContainer.children).HasSingleItem();

        await Assert.That(childFixture.name).IsEqualTo("Child scope fixture");
        await Assert.That(parentFixture.name).IsEqualTo("Parent scope fixture");

        await Assert.That(childScopeChild).IsEqualTo(testResult.uuid);
        await Assert.That(parentScopeChild).IsEqualTo(testResult.uuid);
    }

    [Test]
    public async Task ShouldPreservePendingTestsScopeAssociationOrderWhenScopeBecomesAvailable()
    {
        var test1Started = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "First associated test",
                Properties = new(new InProgressTestNodeStateProperty()),
            }
        );
        var test2Started = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-2",
                DisplayName = "Second associated test",
                Properties = new(new InProgressTestNodeStateProperty()),
            }
        );
        var testsInScope1 = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1")]
        );
        var testsInScope2 = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-2")]
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var test1Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "First associated test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var test2Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-2",
                DisplayName = "Second associated test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test1Started, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test2Started, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope2, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test1Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test2Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(container.children).IsEquivalentTo([
            this.writer.TestResults[0].uuid,
            this.writer.TestResults[1].uuid,
        ], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task ShouldEmitSiblingChildContainersWhenSharedParentBecomesAvailable()
    {
        var startChildScope1 = new AllureScopeStartMessage(
            this.correlationUid,
            new("child-scope-1"),
            new("parent-scope")
        );
        var startChildFixture1 = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("child-fixture-1"),
            new("child-scope-1"),
            "Child fixture 1"
        );
        var startChildScope2 = new AllureScopeStartMessage(
            this.correlationUid,
            new("child-scope-2"),
            new("parent-scope")
        );
        var startChildFixture2 = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("child-fixture-2"),
            new("child-scope-2"),
            "Child fixture 2"
        );
        var startParentScope = new AllureScopeStartMessage(this.correlationUid, new("parent-scope"));
        var startParentFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("parent-fixture"),
            new("parent-scope"),
            "Parent fixture"
        );
        var stopChildFixture1 = new AllureFixtureStopMessage(this.correlationUid, new("child-fixture-1"));
        var stopChildFixture2 = new AllureFixtureStopMessage(this.correlationUid, new("child-fixture-2"));
        var stopParentFixture = new AllureFixtureStopMessage(this.correlationUid, new("parent-fixture"));
        var stopChildScope1 = new AllureScopeStopMessage(this.correlationUid, new("child-scope-1"));
        var stopChildScope2 = new AllureScopeStopMessage(this.correlationUid, new("child-scope-2"));
        var stopParentScope = new AllureScopeStopMessage(this.correlationUid, new("parent-scope"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildScope1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildFixture1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildScope2, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildFixture2, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildFixture1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildFixture2, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildScope1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildScope2, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(3);

        var siblingContainer1 = this.writer.TestContainers[0];
        var siblingContainer2 = this.writer.TestContainers[1];
        var parentContainer = this.writer.TestContainers[2];

        var siblingFixture1 = await Assert.That(siblingContainer1.befores).HasSingleItem();
        await Assert.That(siblingFixture1.name).IsEqualTo("Child fixture 1");

        var siblingFixture2 = await Assert.That(siblingContainer2.befores).HasSingleItem();
        await Assert.That(siblingFixture2.name).IsEqualTo("Child fixture 2");

        var parentFixture = await Assert.That(parentContainer.befores).HasSingleItem();
        await Assert.That(parentFixture.name).IsEqualTo("Parent fixture");
    }

    [Test]
    public async Task ShouldReplayDeeplyNestedPendingScopesInOrder()
    {
        var startChildScope = new AllureScopeStartMessage(
            this.correlationUid,
            new("child-scope"),
            new("parent-scope")
        );
        var startChildFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("child-fixture"),
            new("child-scope"),
            "Child fixture"
        );
        var startParentScope = new AllureScopeStartMessage(
            this.correlationUid,
            new("parent-scope"),
            new("grandparent-scope")
        );
        var startParentFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("parent-fixture"),
            new("parent-scope"),
            "Parent fixture"
        );
        var startGrandparentScope = new AllureScopeStartMessage(this.correlationUid, new("grandparent-scope"));
        var startGrandparentFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("grandparent-fixture"),
            new("grandparent-scope"),
            "Grandparent fixture"
        );
        var stopChildFixture = new AllureFixtureStopMessage(this.correlationUid, new("child-fixture"));
        var stopParentFixture = new AllureFixtureStopMessage(this.correlationUid, new("parent-fixture"));
        var stopGrandparentFixture = new AllureFixtureStopMessage(this.correlationUid, new("grandparent-fixture"));
        var stopChildScope = new AllureScopeStopMessage(this.correlationUid, new("child-scope"));
        var stopParentScope = new AllureScopeStopMessage(this.correlationUid, new("parent-scope"));
        var stopGrandparentScope = new AllureScopeStopMessage(this.correlationUid, new("grandparent-scope"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startChildFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startParentFixture, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).IsEmpty();

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startGrandparentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startGrandparentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopGrandparentFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopChildScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopParentScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopGrandparentScope, CancellationToken.None);

        await Assert.That(this.writer.TestContainers).Count().IsEqualTo(3);

        var childContainer = this.writer.TestContainers[0];
        var parentContainer = this.writer.TestContainers[1];
        var grandParentContainer = this.writer.TestContainers[2];

        var childFixture = await Assert.That(childContainer.befores).HasSingleItem();
        await Assert.That(childFixture.name).IsEqualTo("Child fixture");

        var parentFixture = await Assert.That(parentContainer.befores).HasSingleItem();
        await Assert.That(parentFixture.name).IsEqualTo("Parent fixture");

        var grandParentFixture = await Assert.That(grandParentContainer.befores).HasSingleItem();
        await Assert.That(grandParentFixture.name).IsEqualTo("Grandparent fixture");
    }

    [Test]
    public async Task ShouldApplyPendingTestsScopeAssociationBeforeTestsStart()
    {
        var testsInScope = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1"), new("test-2")]
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var test1Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "First test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var test2Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-2",
                DisplayName = "Second test",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test1Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test2Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(container.children).IsEquivalentTo(
            this.writer.TestResults.Select(static testResult => testResult.uuid)
        );
    }

    [Test]
    public async Task ShouldApplyPendingTestsScopeAssociationToRetries()
    {
        var testsInScope = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1")]
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var testAttempt1Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Attempt 1",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var testAttempt2Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Attempt 2",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testAttempt1Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testAttempt2Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(this.writer.TestResults).Count().IsEqualTo(2);
        await Assert.That(container.children).IsEquivalentTo(
            this.writer.TestResults.Select(static testResult => testResult.uuid)
        );
    }

    [Test]
    public async Task ShouldNotKeepStaleScopeAssociationsAfterScopeFinished()
    {
        var testsInScope1 = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1")]
        );
        var testsInScope2 = new AllureTestsScopeMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-2")]
        );
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var test1Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Scoped test 1",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var test2Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-2",
                DisplayName = "Scoped test 2",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));
        var reusedTest1Passed = new TestNodeUpdateMessage(
            this.sessionUid,
            new()
            {
                Uid = "test-1",
                DisplayName = "Reused test 1",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope1, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope2, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test1Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, test2Passed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, reusedTest1Passed, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        await Assert.That(this.writer.TestResults).Count().IsEqualTo(3);

        await Assert.That(container.children).Count().IsEqualTo(2);
        await Assert.That(container.children).Contains(this.writer.TestResults[0].uuid);
        await Assert.That(container.children).Contains(this.writer.TestResults[1].uuid);
        await Assert.That(container.children).DoesNotContain(this.writer.TestResults[2].uuid);
    }
}
