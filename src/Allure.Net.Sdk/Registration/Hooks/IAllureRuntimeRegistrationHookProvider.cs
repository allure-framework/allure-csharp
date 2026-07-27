using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public interface IAllureRuntimeRegistrationHookProvider<TConfiguration, out THook>
    where TConfiguration : AllureConfiguration
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    bool HasHook { get; }

    THook GetHook();
}
