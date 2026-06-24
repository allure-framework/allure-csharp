using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public class PendingUpdateTests : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");

    [Test]
    public async Task ShouldApplyPendingFixtureStartWhenScopeBecomesAvailable()
    {
        var startFixture = new AllureBeforeFixtureStartMessage(
            this.correlationUid,
            new("fixture-1"),
            new("scope-1"),
            "Pending fixture");
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
}
