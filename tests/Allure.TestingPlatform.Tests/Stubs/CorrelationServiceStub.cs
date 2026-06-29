using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.Stubs;

public class CorrelationServiceStub : ICorrelationSource
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
