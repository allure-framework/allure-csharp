using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure runtime during registration.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
public interface IAllureRuntimeRegistrationHook<TConfiguration> :
    IAllureRegistrationHook<IAllureRuntimeRegistrationContext<TConfiguration>>

    where TConfiguration : AllureConfiguration;

/// <summary>
/// Customizes a standard Allure runtime during registration.
/// </summary>
public interface IAllureRuntimeRegistrationHook :
    IAllureRegistrationHook<IAllureRuntimeRegistrationContext>;
