using Allure.Sdk.Registration.Hooks;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TContext, TRuntime> :
    IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

public interface IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TContext> :
    IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TContext, IAllureTestingPlatformRuntime<TConfiguration>>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, IAllureTestingPlatformRuntime<TConfiguration>>;

public interface IAllureTestingPlatformEndpointRegistrationHook :
    IAllureTestingPlatformEndpointRegistrationHook<AllureTestingPlatformConfiguration, IAllureTestingPlatformEndpointRegistrationContext>;
