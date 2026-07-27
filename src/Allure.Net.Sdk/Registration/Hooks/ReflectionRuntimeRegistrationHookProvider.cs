using System;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

public class ReflectionRuntimeRegistrationHookProvider<TConfiguration, THook>(
    string? assemblyQualifiedTypeName
) :
    ReflectionRegistrationHookProvider<THook>(assemblyQualifiedTypeName),
    IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>

    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    public static ReflectionRuntimeRegistrationHookProvider<TConfiguration, THook> FromConfiguration(
        TConfiguration configuration
    ) =>
        new(configuration.RegistrationHook);

    public static ReflectionRuntimeRegistrationHookProvider<TConfiguration, THook> FromEnvironmentVariable(
        string environmentVariableName
    ) =>
        new(Environment.GetEnvironmentVariable(environmentVariableName));

    public static ReflectionRuntimeRegistrationHookProvider<TConfiguration, THook> FromEnvironmentVariable() =>
        FromEnvironmentVariable("ALLURE_REGISTRATION_HOOK");
}
