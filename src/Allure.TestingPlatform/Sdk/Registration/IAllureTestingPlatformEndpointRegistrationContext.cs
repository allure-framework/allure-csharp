using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime> :
    IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

public interface IAllureTestingPlatformEndpointRegistrationContext<TConfiguration> :
    IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, IAllureTestingPlatformRuntime<TConfiguration>>

    where TConfiguration : AllureTestingPlatformConfiguration;

public interface IAllureTestingPlatformEndpointRegistrationContext :
    IAllureTestingPlatformEndpointRegistrationContext<AllureTestingPlatformConfiguration>;
