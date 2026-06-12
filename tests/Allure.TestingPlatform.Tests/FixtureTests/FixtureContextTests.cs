
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Messages;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Tests.FixtureTests;

public class FixtureContextTests : DataConsumerTestsBase
{
    readonly SessionUid sessionUid = new("Bar");

    [Test]
    public async Task ShouldEmitContainerWithBeforeFixture()
    {
        var startScope = new AllureScopeStartMessage(sessionUid, "1");
        var startFixture = new AllureBeforeFixtureStartMessage(sessionUid, "2", "1", "Foo");
        var stopFixture = new AllureFixtureStopMessage(sessionUid, "2");
        var stopScope = new AllureScopeStopMessage(sessionUid, "1");

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
        var startScope = new AllureScopeStartMessage(sessionUid, "1");
        var startFixture = new AllureAfterFixtureStartMessage(sessionUid, "2", "1", "Foo");
        var stopFixture = new AllureFixtureStopMessage(sessionUid, "2");
        var stopScope = new AllureScopeStopMessage(sessionUid, "1");

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
