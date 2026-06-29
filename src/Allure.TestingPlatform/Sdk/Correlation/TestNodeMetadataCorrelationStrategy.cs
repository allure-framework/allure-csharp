using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Correlation;

public class TestNodeMetadataCorrelationStrategy : ICorrelationStrategy
{
    public const string MetadataKey = "Allure.TestingPlatform.CorrelationUid";

    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<CorrelationUid?>(
            message is TestNodeUpdateMessage { TestNode.Properties: var properties }
                ? properties
                    .OfType<TestMetadataProperty>()
                    .FirstOrDefault(
                        static (meta) => meta.Key == MetadataKey
                    )?.Value switch
                    {
                        null => null,
                        var value => new(value),
                    }
                : null
        );

    public static string CreateCorrelationUid() => Guid.NewGuid().ToString();
}
