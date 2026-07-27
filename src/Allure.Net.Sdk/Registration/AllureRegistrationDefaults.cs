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
    public static Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> ConfigurationSources<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        static () => [
            JsonFileConfigurationSource.FromPathEnvironmentVariable<TConfiguration>(),
            JsonFileConfigurationSource.FromBaseDirectory<TConfiguration>(),
        ];

    public static Func<TConfiguration, IAllureResultsDestination> Destination<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (configuration) => new FileSystemResultsDestination(
            configuration.ResultsDirectory,
            configuration.IndentOutput
        );

    public static Func<TConfiguration, IAllureParameterSerializer> ParameterSerializer<TConfiguration>()=>
        static (_) => new DefaultParameterSerializerBuilder().Build();

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureRuntimeContext> Context<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new AsyncLocalRuntimeContext(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> LifecycleApi<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new RuntimeBoundLifecycleApi(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> ModelApi<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new RuntimeBoundModelApi(runtime.RuntimeReference);

    public static Func<TConfiguration, IEnumerable<IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>>> HookProviders<TConfiguration, THook>()
        where TConfiguration : AllureConfiguration, new()
        where THook : IAllureRuntimeRegistrationHook<TConfiguration>
    =>
        static (configuration) => [
            ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook>.FromEnvironmentVariable(),
            ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook>.FromConfiguration(configuration),
        ];
}
