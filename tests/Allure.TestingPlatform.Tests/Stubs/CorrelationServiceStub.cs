using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.Stubs;

public class CorrelationStrategyStub : ICorrelationStrategy
{
    public Queue<string> NextValues { get; set; } = [];

    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    )
    {
        CorrelationUid? value =
            this.NextValues.TryDequeue(out var dequeuedValue)
                ? new(dequeuedValue)
                : null;
        return Task.FromResult(value);
    }
}
