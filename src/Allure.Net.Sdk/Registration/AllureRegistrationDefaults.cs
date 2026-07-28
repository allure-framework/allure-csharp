using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Internal.Runtime;
using Allure.Sdk.Registration.Hooks;
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

    public static Func<TConfiguration, IAllureParameterSerializer> ParameterSerializer<TConfiguration>(
        IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> registrations
    ) =>
        (configuration) =>
        {
            var builder = new RuleBasedParameterSerializerBuilder();
            foreach (var registration in registrations)
            {
                registration(configuration, builder);
            }
            return builder.Build();
        };

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> Context<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new AsyncLocalExecutionContext(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> LifecycleApi<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new RuntimeLifecycleApi(runtime.RuntimeReference);

    public static Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> ModelApi<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (runtime) => new RuntimeModelApi(runtime.RuntimeReference);

    public static Func<TConfiguration, IEnumerable<THook?>> RuntimeHookProviders<TConfiguration, THook>()
        where TConfiguration : AllureConfiguration, new()
        where THook : IAllureRuntimeRegistrationHook<TConfiguration>
    =>
        static (configuration) => [
            ReflectionHooks.FromEnvironmentVariable<THook>("ALLURE_RUNTIME_REGISTRATION_HOOK"),
            ReflectionHooks.FromConfiguration<TConfiguration, THook>(configuration),
        ];
}
