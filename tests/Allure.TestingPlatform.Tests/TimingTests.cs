using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public class TimingTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldFillStartAndStop()
    {
        var start = DateTimeOffset.Now;
        var stop = start.AddMilliseconds(
            Random.Shared.NextInt64(1, 1000)
        );

        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty(),
                new TimingProperty(
                    new TimingInfo(
                        start,
                        stop,
                        stop - start
                    )
                )
            )
        };
        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        await Assert.That(this.writer.TestResults).Count().IsEqualTo(1);
        var testResult = this.writer.TestResults[0];
        await Assert.That(testResult.Start).IsEqualTo(start.ToUnixTimeMilliseconds());
        await Assert.That(testResult.Stop).IsEqualTo(stop.ToUnixTimeMilliseconds());
    }
}
