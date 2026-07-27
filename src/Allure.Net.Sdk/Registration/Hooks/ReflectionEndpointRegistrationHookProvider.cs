using System;

namespace Allure.Sdk.Registration.Hooks;

public class ReflectionEndpointRegistrationHookProvider<THook>(
    string? assemblyQualifiedTypeName
) :
    ReflectionRegistrationHookProvider<THook>(assemblyQualifiedTypeName),
    IAllureEndpointRegistrationHookProvider<THook>

    where THook : IAllureEndpointRegistrationHook
{
    public static ReflectionEndpointRegistrationHookProvider<THook> FromEnvironmentVariable(
        string environmentVariableName
    ) =>
        new(Environment.GetEnvironmentVariable(environmentVariableName));

    public static ReflectionEndpointRegistrationHookProvider<THook> FromEnvironmentVariable() =>
        FromEnvironmentVariable("ALLURE_REGISTRATION_HOOK");
}
