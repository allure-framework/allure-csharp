using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Represents an Allure Testing Platform registration managed by Microsoft Testing Platform.
/// </summary>
/// <remarks>
/// Provides late-bound access to the registration's configuration and runtime, and exposes the
/// message channel associated with the active Microsoft Testing Platform request. Microsoft
/// Testing Platform manages the registration's lifetime.
/// </remarks>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformRegistration<out TConfiguration, out TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    /// <summary>
    /// Gets a late-bound reference to the registration's resolved runtime configuration.
    /// </summary>
    IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference { get; }

    /// <summary>
    /// Gets a late-bound reference to the registered runtime.
    /// </summary>
    IReadOnlyLateBoundReference<TRuntime> RuntimeReference { get; }

    /// <summary>
    /// Gets the channel used to publish messages through the active Microsoft Testing Platform
    /// request.
    /// </summary>
    IAllureTestingPlatformMessageChannel MessageChannel { get; }
}
