using Allure.Sdk.Registration.Hooks;
using Allure.TestingPlatform.Configuration;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TContext> :
    IAllureRuntimeRegistrationHook<TConfiguration, TContext>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>;
