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
    /// <param name="dataProducer">The extension that produced the message.</param>
    /// <param name="message">The message whose correlation identifier is requested.</param>
    /// <param name="cancellationToken">A token that requests cancellation of the operation.</param>
    /// <returns>
    /// A task whose result is the correlation identifier, or <see langword="null"/> when
    /// the message cannot yet be correlated.
    /// </returns>
    Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    );
}
