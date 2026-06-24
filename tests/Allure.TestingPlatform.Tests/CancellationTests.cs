using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests;

public class CancellationTests : DataConsumerTestsBase<CancellationTests.CorrelationServiceSpy, ThrowingLoggerStub>
{
    [Test]
    public async Task ShouldPassCancellationTokenToCorrelationService()
    {
        var message = new TestNodeUpdateMessage(
            new("session-1"),
            new()
            {
                Uid = "test-1",
                DisplayName = "Node",
                Properties = new(new PassedTestNodeStateProperty()),
            }
        );
        using var cancellationTokenSource = new CancellationTokenSource();

        await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            message,
            cancellationTokenSource.Token
        );

        await Assert.That(this.correlationService.WasCalled).IsTrue();
        await Assert.That(this.correlationService.LastCancellationToken).IsEqualTo(cancellationTokenSource.Token);
    }

    public class CorrelationServiceSpy : ICorrelationService
    {
        public bool WasCalled { get; private set; }
        public CancellationToken LastCancellationToken { get; private set; }

        public Task<CorrelationUid?> GetCorrelationAsync(
            IDataProducer dataProducer,
            DataWithSessionUid message,
            CancellationToken cancellationToken
        )
        {
            this.WasCalled = true;
            this.LastCancellationToken = cancellationToken;
            return Task.FromResult<CorrelationUid?>(null);
        }
    }
}
