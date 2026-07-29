using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure runtime during registration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntimeRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Applies custom runtime configuration.
    /// </summary>
    void SetUp(IAllureRuntimeRegistrationContext<TConfiguration> context);
}
