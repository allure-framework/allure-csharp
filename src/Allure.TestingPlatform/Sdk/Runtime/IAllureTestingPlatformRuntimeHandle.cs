using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Provides late-bound access to an Allure Microsoft Testing Platform runtime and publishes
/// messages through the active Microsoft Testing Platform request.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformRuntimeHandle<out TConfiguration, out TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <summary>
    /// Gets a late-bound reference to the resolved runtime configuration.
    /// </summary>
    IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference { get; }

    /// <summary>
    /// Gets a late-bound reference to the runtime.
    /// </summary>
    IReadOnlyLateBoundReference<TRuntime> RuntimeReference { get; }

    /// <summary>
    /// Gets a value indicating whether messages can currently be published.
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
