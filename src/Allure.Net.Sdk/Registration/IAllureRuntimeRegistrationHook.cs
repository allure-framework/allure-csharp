using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration;

public interface IAllureRuntimeRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetUp(IAllureRuntimeRegistrationContext<TConfiguration> context);
}

public interface IAllureRuntimeRegistrationHook
{
    void SetUp(IAllureRuntimeRegistrationContext context);
}
