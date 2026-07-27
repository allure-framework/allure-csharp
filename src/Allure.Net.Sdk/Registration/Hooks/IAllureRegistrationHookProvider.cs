using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public interface IAllureRegistrationHookProvider<TConfiguration, out THook>
    where TConfiguration : AllureConfiguration
    where THook : IAllureRegistrationHook<TConfiguration>
{
    bool HasHook { get; }

    THook GetHook();
}
