using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.Model;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Tests.FixtureTests;

public class FixtureContextTests : DataConsumerTestsBase
{
    readonly CorrelationUid correlationUid = new("Bar");

    [Test]
    public async Task ShouldEmitContainerWithSetUpFixture()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureSetUpFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Befores).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("Foo");
        await Assert.That(fixture.Stage).IsEqualTo(Stage.Finished);
        await Assert.That(fixture.Status).IsEqualTo(Status.Unknown);
    }

    [Test]
    public async Task ShouldEmitContainerWithTearDownFixture()
    {
        var startScope = new AllureScopeStartMessage(correlationUid, new("1"));
        var startFixture = new AllureTearDownFixtureStartMessage(correlationUid, new("2"), new("1"), "Foo");
        var stopFixture = new AllureFixtureStopMessage(correlationUid, new("2"));
        var stopScope = new AllureScopeStopMessage(correlationUid, new("1"));

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startScope, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, startFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopFixture, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, stopScope, CancellationToken.None);

        var container = await Assert.That(this.writer.TestContainers).HasSingleItem();
        var fixture = await Assert.That(container.Afters).HasSingleItem();
        await Assert.That(fixture.Name).IsEqualTo("Foo");
        await Assert.That(fixture.Stage).IsEqualTo(Stage.Finished);
        await Assert.That(fixture.Status).IsEqualTo(Status.Unknown);
    }
}
