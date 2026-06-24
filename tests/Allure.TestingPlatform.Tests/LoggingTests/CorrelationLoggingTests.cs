using Allure.TestingPlatform.Tests.Stubs;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests.LoggingTests;

public class CorrelationLoggingTests : DataConsumerTestsBase<CorrelationServiceStub, LoggerSpy>
{
    [Test]
    public async Task ShouldLogDiscardedSession()
    {
        var sharedCorrelationUid = new CorrelationUid("c-shared");
        var session1Test1StopPassed = new TestNodeUpdateMessage(
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

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session1Test1StopPassed, CancellationToken.None);
        await this.consumer.ConsumeAsync(DataProducerStub.Instance, session2Test2StopPassed, CancellationToken.None);

        await Assert.That(this.logger.Calls).HasSingleItem();

        var (level, state, _) = await Assert.That(this.logger.Calls).HasSingleItem();
        await Assert.That(level).IsEqualTo(LogLevel.Error);
        await Assert.That(state).IsTypeOf<string>()
            .And.Contains(sharedCorrelationUid.Value)
            .And.Contains("s1")
            .And.Contains("s2");
    }
}