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
/// Customizes an Allure endpoint through the standard registration context.
/// </summary>
public interface IAllureEndpointRegistrationHook :
    IAllureEndpointRegistrationHook<IAllureEndpointRegistrationContext>;
