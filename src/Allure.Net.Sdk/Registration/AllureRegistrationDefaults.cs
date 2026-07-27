using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Extensions;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public static class AllureRegistrationDefaults
{
    public static Func<IAllureParameterSerializer> ParameterSerializer =>
        static () => new DefaultParameterSerializerBuilder().Build();
}

public static class AllureRegistrationDefaults<TConfiguration>
    where TConfiguration : AllureConfiguration, new()
{
    public static Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> ConfigurationSources =>
        static () => [
            JsonFileConfigurationSource.FromEnvironmentVariable<TConfiguration>(),
            JsonFileConfigurationSource.FromBaseDirectory<TConfiguration>(),
        ];

    public static Func<TConfiguration, IAllureResultsDestination> Destination =>
        static (configuration) => new FileSystemResultsDestination(
            configuration.ResultsDirectory,
            configuration.IndentOutput
        );

    public static Func<TConfiguration, IAllureParameterSerializer> Serializer =>
        static (configuration) => new DefaultParameterSerializerBuilder().Build();

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureRuntimeContext> Context =>
        static (runtime) => new AsyncLocalRuntimeContext(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> LifecycleApi =>
        static (runtime) => new RuntimeBoundLifecycleApi(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> ModelApi =>
        static (runtime) => new RuntimeBoundModelApi(runtime.RuntimeReference);
}

public static class AllureRegistrationDefaults<TConfiguration, THook>
    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    public static Func<TConfiguration, IEnumerable<IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>>> HookProviders =>
        static (configuration) => [
            ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook>.FromEnvironmentVariable(),
            ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook>.FromConfiguration(configuration),
        ];
}
