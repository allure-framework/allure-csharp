namespace Allure.Sdk.Registration.Hooks;

public interface IAllureEndpointRegistrationHookProvider<out THook>
    where THook : IAllureEndpointRegistrationHook
{
    bool HasHook { get; }

    THook GetHook();
}
