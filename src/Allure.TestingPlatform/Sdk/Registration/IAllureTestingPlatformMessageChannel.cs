using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Provides a communication channel for the active Microsoft Testing Platform request.
/// </summary>
/// <remarks>
/// The channel is available only while its associated Allure Testing Platform registration is
/// bound to an active Microsoft Testing Platform request.
/// </remarks>
public interface IAllureTestingPlatformMessageChannel
{
    /// <summary>
    /// Gets a value indicating whether the channel is bound to an active request and can publish
    /// messages.
    /// </summary>
    bool CanPublish { get; }

    /// <summary>
    /// Publishes a message through the active Microsoft Testing Platform request.
    /// </summary>
    /// <param name="dataProducer">The extension producing the message.</param>
    /// <param name="data">The message to publish.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishAsync(IDataProducer dataProducer, IData data);
}
