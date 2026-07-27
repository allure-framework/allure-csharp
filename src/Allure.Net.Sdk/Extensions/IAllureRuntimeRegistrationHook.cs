using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;

namespace Allure.Sdk.Extensions;

public interface IAllureRuntimeRegistrationHook<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetUp(IAllureRuntimeRegistrationContext<TConfiguration> context);
}
