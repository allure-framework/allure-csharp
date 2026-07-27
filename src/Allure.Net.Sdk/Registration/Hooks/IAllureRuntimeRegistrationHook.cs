using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public interface IAllureRuntimeRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetUp(IAllureRuntimeRegistrationContext<TConfiguration> context);
}
