using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Correlation;

/// <summary>
/// Uses the Microsoft Testing Platform session UID as the Allure correlation identifier.
/// Should be used by integrations that have access to the current session UID.
/// </summary>
public sealed class TestingPlatformSessionUidCorrelationStrategy : ICorrelationStrategy
{
    /// <inheritdoc />
    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<CorrelationUid?>(
            new CorrelationUid(message.SessionUid.Value)
        );
}
