using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration;

public interface IAllureRegistrationHook
{
    void SetUp(IAllureRegistrationContext context);
}
