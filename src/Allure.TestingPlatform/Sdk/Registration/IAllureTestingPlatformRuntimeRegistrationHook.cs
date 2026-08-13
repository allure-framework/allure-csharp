using Allure.Sdk.Registration.Hooks;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeRegistrationHook<TContext> :
    IAllureRegistrationHook<TContext>

    where TContext : IAllureTestingPlatformRuntimeRegistrationContextBase;
