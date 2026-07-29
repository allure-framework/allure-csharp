using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure runtime during registration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureRuntimeRegistrationHook<TConfiguration, TContext>
    where TConfiguration : AllureConfiguration
    where TContext : IAllureRuntimeRegistrationContext<TConfiguration>
{
    /// <summary>
    /// Applies custom runtime configuration.
    /// </summary>
    void SetUp(TContext context);
}

/// <summary>
/// Customizes an Allure runtime that uses the standard
/// <see cref="AllureConfiguration"/> during registration.
/// </summary>
public interface IAllureRuntimeRegistrationHook :
    IAllureRuntimeRegistrationHook<
        AllureConfiguration,
        IAllureRuntimeRegistrationContext
    >;
