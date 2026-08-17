using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Correlation;

/// <summary>
/// Resolves correlation identifiers from test node metadata.
/// Should be used by integrations that does not have access to the
/// current session UID, but can attach metadata to <see cref="TestNodeUpdateMessage"/>
/// sent by the test framework.
/// </summary>
public sealed class TestNodeMetadataCorrelationStrategy : ICorrelationStrategy
{
    /// <summary>
    /// The metadata key that stores the correlation identifier.
    /// </summary>
    public const string MetadataKey = "Allure.TestingPlatform.CorrelationUid";

    /// <inheritdoc />
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

    /// <summary>
    /// Creates a new correlation identifier suitable for test node metadata.
    /// </summary>
    public static string CreateCorrelationUid() => Guid.NewGuid().ToString();
}
