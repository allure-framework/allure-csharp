using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime.Correlation;

public class TestMetadataCorrelation : ICorrelationSource
{
    public const string METADATA_KEY = "Allure.TestingPlatform.CorrelationUid";

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
                        static (meta) => meta.Key == METADATA_KEY
                    )?.Value switch
                    {
                        null => null,
                        var value => new(value),
                    }
                : null
        );

    public static string CreateCorrelationUid() => Guid.NewGuid().ToString();
}
