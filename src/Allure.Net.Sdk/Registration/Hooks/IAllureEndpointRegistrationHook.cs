namespace Allure.Sdk.Registration.Hooks;

public interface IAllureEndpointRegistrationHook
{
    void SetUp(IAllureEndpointRegistrationContext context);
}
