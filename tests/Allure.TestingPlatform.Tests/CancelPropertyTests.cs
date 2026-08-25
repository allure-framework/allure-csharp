using Allure.Model;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

using AllureTestResult = Allure.Model.TestResult;

namespace Allure.TestingPlatform.Tests;

public class CancelPropertyTests : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");
    readonly SessionUid sessionUid = new("Bar");

    [Test]
    public async Task ShouldNotWriteCancelledRunningTest()
    {
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new InProgressTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateCancelMessage(),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldNotWriteCancelledTestWhenCancelArrivesBeforeStart()
    {
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateCancelMessage(),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new InProgressTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldNotWriteCancelledTestCreatedFromSingleTerminalMessage()
    {
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateCancelMessage(),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldIgnoreNonCancellationLabels()
    {
        var updateTest = new AllureTestUpdateMessage(this.correlationUid, new("test-1"))
        {
            Properties =
            [
                new AllureLabelsProperty(
                    [
                        new(){ Name = AllureCancelProperty.CANCEL_LABEL_NAME, Value = "false" },
                        new(){ Name = "not-cancelled", Value = "true" },
                    ]
                ),
            ],
        };

        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new InProgressTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            updateTest,
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.Status).IsEqualTo(Status.Passed);
    }

    [Test]
    public async Task ShouldApplyCancellationTogetherWithOtherPendingUpdates()
    {
        var updateTest = new AllureTestUpdateMessage(this.correlationUid, new("test-1"))
        {
            Properties =
            [
                new AllureNameProperty<AllureTestResult>("Updated test name"),
                new AllureCancelProperty(),
                new AllureDescriptionProperty<AllureTestResult>("Updated description"),
            ],
        };

        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            updateTest,
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new InProgressTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );

        await Assert.That(this.writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldStillWriteContainersForCancelledScopedTest()
    {
        var startScope = new AllureScopeStartMessage(this.correlationUid, new("scope-1"));
        var testsInScope = new AllureScopeTestsMessage(
            this.correlationUid,
            new("scope-1"),
            [new("test-1")]
        );
        var startFixture = new AllureSetUpFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Scope fixture"
        );
        var stopFixture = new AllureFixtureStopMessage(this.correlationUid, new("fixture-1"));
        var stopScope = new AllureScopeStopMessage(this.correlationUid, new("scope-1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testsInScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new InProgressTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateCancelMessage(),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            this.CreateTestNodeMessage(new PassedTestNodeStateProperty()),
            CancellationToken.None
        );
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Befores).HasSingleItem();

        await Assert.That(this.writer.TestResults).IsEmpty();
        await Assert.That(fixture.Name).IsEqualTo("Scope fixture");
        await Assert.That(container.Children).HasSingleItem();
    }

    AllureTestUpdateMessage CreateCancelMessage() => new(this.correlationUid, new("test-1"))
    {
        Properties = [new AllureCancelProperty()],
    };

    TestNodeUpdateMessage CreateTestNodeMessage(IProperty state) => new(
        this.sessionUid,
        new()
        {
            Uid = "test-1",
            DisplayName = "Test",
            Properties = new(state),
        }
    );
}
