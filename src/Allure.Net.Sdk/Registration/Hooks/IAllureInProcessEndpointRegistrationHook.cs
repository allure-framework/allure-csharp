using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an in-process Allure endpoint during registration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The registration context type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext, TRuntime>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
{
    /// <summary>
    /// Applies custom endpoint configuration.
    /// </summary>
    void SetUp(TContext context);
}

/// <summary>
/// Customizes an in-process Allure endpoint for a standard runtime during
/// registration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext> :
    IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext, IAllureRuntime<TConfiguration>>

    where TConfiguration : AllureConfiguration
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>;

/// <summary>
/// Customizes an in-process Allure endpoint that uses the standard
/// <see cref="AllureConfiguration"/> during registration.
/// </summary>
public interface IAllureInProcessEndpointRegistrationHook :
    IAllureInProcessEndpointRegistrationHook<AllureConfiguration, IAllureInProcessEndpointRegistrationContext>;
