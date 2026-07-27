using System;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public class DelegateRegistrationHookProvider<TConfiguration, THook>(
    Func<THook?> hookFactory
) :
    IAllureRegistrationHookProvider<TConfiguration, THook>

    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRegistrationHook<TConfiguration>
{
    readonly Lazy<THook?> hook = new(hookFactory);

    public bool HasHook => this.hook.Value is not null;

    public THook GetHook() => this.hook.Value
        ?? throw new InvalidOperationException("The hook instance is null");
}
