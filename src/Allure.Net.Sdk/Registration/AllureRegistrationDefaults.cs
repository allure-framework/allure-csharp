using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides default factories for Allure runtime components.
/// </summary>
public static class AllureRegistrationDefaults
{
    /// <summary>
    /// Creates the default ordered configuration-source factory.
    /// </summary>
    public static Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> ConfigurationSources<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        static () => [
            JsonFileConfigurationSource.FromPathEnvironmentVariable<TConfiguration>(),
            JsonFileConfigurationSource.FromBaseDirectory<TConfiguration>(true),
        ];

    /// <summary>
    /// Creates the default file-system results-destination factory.
    /// </summary>
    public static Func<TConfiguration, IAllureResultsDestination> Destination<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (configuration) => new FileSystemResultsDestination(
            configuration.ResultsDirectory,
            configuration.IndentOutput
        );

    /// <summary>
    /// Creates an endpoint rule-based parameter-serializer factory from rule registrations.
    /// </summary>
    public static Func<TRuntime, IAllureParameterSerializer> EndpointParameterSerializer<TRuntime>(
        IEnumerable<Action<TRuntime, IParameterSerializationRulesContext>> registrations
    )
        where TRuntime : IAllureRuntime
    =>
        (configuration) =>
        {
            var builder = new RuleBasedParameterSerializerBuilder();
            foreach (var registration in registrations)
            {
                registration(configuration, builder);
            }
            return builder.Build();
        };

    /// <summary>
    /// Creates a runtime rule-based parameter-serializer factory from rule registrations.
    /// </summary>
    public static Func<TConfiguration, IAllureParameterSerializer> RuntimeParameterSerializer<TConfiguration>(
        IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> registrations
    )
        where TConfiguration : AllureConfiguration
    =>
        (configuration) =>
        {
            var builder = new RuleBasedParameterSerializerBuilder();
            foreach (var registration in registrations)
            {
                registration(configuration, builder);
            }
            return builder.Build();
        };

    /// <summary>
    /// Creates the default runtime-hook provider factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <typeparam name="TContext">The runtime registration context type.</typeparam>
    /// <typeparam name="THook">The runtime registration hook type.</typeparam>
    public static Func<TConfiguration, IEnumerable<THook?>> RuntimeHookProviders<TConfiguration, TContext, THook>()
        where TConfiguration : AllureConfiguration, new()
        where TContext : IAllureRuntimeRegistrationContext<TConfiguration>
        where THook : IAllureRegistrationHook<TContext>
    =>
        static (configuration) => [
            ReflectionHooks.FromEnvironmentVariable<THook>("ALLURE_RUNTIME_REGISTRATION_HOOK"),
            ReflectionHooks.FromConfiguration<TConfiguration, THook>(configuration),
        ];
}
