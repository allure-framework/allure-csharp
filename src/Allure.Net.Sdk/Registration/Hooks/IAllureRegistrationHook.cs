namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Customizes an Allure component during registration.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureRegistrationHook<in TContext>
    where TContext : IAllureRegistrationContext
{
    /// <summary>
    /// Applies custom registration configuration.
    /// </summary>
    /// <param name="context">The active registration context.</param>
    void SetUp(TContext context);
}
