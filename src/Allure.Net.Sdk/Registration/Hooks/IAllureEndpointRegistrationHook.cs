namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure endpoint during registration.
/// </summary>
public interface IAllureEndpointRegistrationHook
{
    /// <summary>
    /// Applies custom endpoint configuration.
    /// </summary>
    void SetUp(IAllureEndpointRegistrationContext context);
}
