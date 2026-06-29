using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;

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

    [Test]
    public async Task ShouldPropagateOperationCanceledExceptionFromConsumeAsync()
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
        cancellationTokenSource.Cancel();

        await Assert.That(async () => await this.consumer.ConsumeAsync(
            DataProducerStub.Instance,
            message,
            cancellationTokenSource.Token
        )).Throws<OperationCanceledException>();
    }

    public class CorrelationServiceSpy : ICorrelationSource
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
