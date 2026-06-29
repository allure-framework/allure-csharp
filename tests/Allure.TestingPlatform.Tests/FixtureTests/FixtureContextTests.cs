using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Tests.FixtureTests;

public class FixtureContextTests : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");

    [Test]
    public async Task ShouldEmitContainerWithBeforeFixture()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureBeforeFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.befores).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Foo");
        await Assert.That(fixture.stage).IsEqualTo(Stage.finished);
        await Assert.That(fixture.status).IsEqualTo(Status.none);
    }

    [Test]
    public async Task ShouldEmitContainerWithAfterFixture()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureAfterFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.afters).HasSingleItem();
        await Assert.That(fixture.name).IsEqualTo("Foo");
        await Assert.That(fixture.stage).IsEqualTo(Stage.finished);
        await Assert.That(fixture.status).IsEqualTo(Status.none);
    }
}
