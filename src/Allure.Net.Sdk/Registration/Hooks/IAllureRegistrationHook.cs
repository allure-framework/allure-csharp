using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public interface IAllureRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetUp(IAllureRuntimeRegistrationContext<TConfiguration> context);
}
