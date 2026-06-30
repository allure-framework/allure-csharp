using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Correlation;

/// <summary>
/// Resolves correlation identifiers for Microsoft Testing Platform messages.
/// </summary>
public interface ICorrelationStrategy
{
    /// <summary>
    /// Gets the correlation identifier for a message,
    /// or <see langword="null"/> when it is not available.
    /// </summary>
    Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    );
}
