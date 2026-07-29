namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure endpoint during registration.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureEndpointRegistrationHook<TContext>
    where TContext : IAllureEndpointRegistrationContext
{
    /// <summary>
    /// Applies custom endpoint configuration.
    /// </summary>
    void SetUp(TContext context);
}

/// <summary>
/// Customizes a standard in-process Allure runtime endpoint.
/// </summary>
public interface IAllureEndpointRegistrationHook :
    IAllureEndpointRegistrationHook<IAllureEndpointRegistrationContext>;
