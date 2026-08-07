using Allure.Model;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Tests;

using AllureTestResult = Model.TestResult;

public class FailExceptionsTests : DataConsumerTestsBase
{
    readonly SessionUid sessionUid = new("Bar");

    readonly CorrelationUid correlationUid = new("Bar");

    [Test]
    public async Task ShouldSetStatusToFailedIfExceptionMatchesFailException()
    {
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
        await Assert.That(testResult.Status).IsEqualTo(Status.Failed);
    }

    [Test]
    public async Task ShouldSetTestFailedStatusAndDetailsFromError()
    {
        var testResult = await this.ArrangeAndAct(
            new AllureExceptionProperty<AllureTestResult>(new Exception("Foo"))
        );

        await Assert.That(testResult.Status).IsEqualTo(Status.Failed);
        await Assert.That(testResult.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(testResult.StatusDetails.Trace).Contains("System.Exception");
    }

    [Test]
    public async Task ShouldSetBeforeFixtureFailedStatusAndDetailsFromError()
    {
        var fixture = await this.ArrangeAndAct(
            new AllureExceptionProperty<FixtureResult>(new Exception("Foo"))
        );

        await Assert.That(fixture.Status).IsEqualTo(Status.Failed);
        await Assert.That(fixture.StatusDetails.Message).IsEqualTo("Foo");
        await Assert.That(fixture.StatusDetails.Trace).Contains("System.Exception");
    }

    async Task<AllureTestResult> ArrangeAndAct(params IAllureProperty<AllureTestResult>[] properties)
    {
        var testNodeInProgress = new TestNodeUpdateMessage(
            sessionUid,
            new()
            {
                Uid = "1",
                DisplayName = "Foo",
                Properties = new(new InProgressTestNodeStateProperty())
            }
        );

        var updateTest = new AllureTestUpdateMessage(correlationUid, new("1"))
        {
            Properties = [..properties],
        };

        var testNodePassed = new TestNodeUpdateMessage(
            sessionUid,
            new()
            {
                Uid = "1",
                DisplayName = "Foo",
                Properties = new(new PassedTestNodeStateProperty())
            }
        );

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodeInProgress, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateTest, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, testNodePassed, CancellationToken.None);

        return await Assert.That(this.writer.TestResults).HasSingleItem();
    }

    async Task<FixtureResult> ArrangeAndAct(params IAllureProperty[] properties)
    {
        var startScopeMessage = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixtureMessage = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var updateFixtureMessage = new AllureFixtureUpdateMessage(correlationUid, new("2"))
        {
            Properties = [.. properties],
        };
        var stopFixtureMessage = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScopeMessage = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScopeMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, updateFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixtureMessage, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScopeMessage, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        return await Assert.That(container.Befores).HasSingleItem();
    }

    protected override AllureTestingPlatformConfiguration Config => new()
    {
        FailExceptions = ["System.Exception"],
    };
}
