namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure runtime during registration.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureRegistrationHook<in TContext>
    where TContext : IAllureRegistrationContext
{
    /// <summary>
    /// Applies custom runtime configuration.
    /// </summary>
    void SetUp(TContext context);
}
