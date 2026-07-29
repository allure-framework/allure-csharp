using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an in-process Allure endpoint during registration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureInProcessEndpointRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Applies custom endpoint configuration.
    /// </summary>
    void SetUp(IAllureInProcessEndpointRegistrationContext<TConfiguration> context);
}
