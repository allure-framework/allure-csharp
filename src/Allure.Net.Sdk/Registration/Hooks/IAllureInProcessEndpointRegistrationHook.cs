using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public interface IAllureInProcessEndpointRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetUp(IAllureInProcessEndpointRegistrationContext<TConfiguration> context);
}
