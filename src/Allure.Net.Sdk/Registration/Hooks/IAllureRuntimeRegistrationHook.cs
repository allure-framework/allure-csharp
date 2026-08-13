namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes a standard Allure runtime during registration.
/// </summary>
public interface IAllureRuntimeRegistrationHook :
    IAllureRegistrationHook<IAllureRuntimeRegistrationContext>;
