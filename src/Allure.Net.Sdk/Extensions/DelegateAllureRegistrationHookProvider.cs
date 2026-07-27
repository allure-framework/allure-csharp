using System;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Extensions;

public class DelegateAllureRegistrationHookProvider<TConfiguration, THook>(
    Func<THook?> hookFactory
) :
    IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>

    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    readonly Lazy<THook?> hook = new(hookFactory);

    public bool HasHook => this.hook.Value is not null;

    public THook GetHook() => this.hook.Value
        ?? throw new InvalidOperationException("The hook instance is null");
}
