using Allure.Sdk.Registration.Hooks;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Represents a registration hook for an Allure Microsoft Testing Platform runtime.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureTestingPlatformRuntimeRegistrationHook<TContext> :
    IAllureRegistrationHook<TContext>

    where TContext : IAllureTestingPlatformRuntimeRegistrationContextBase;
