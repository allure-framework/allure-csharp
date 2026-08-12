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
    /// Creates the default factory for an ordered sequence of configuration sources.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <returns>A factory that creates the default configuration sources.</returns>
    public static Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> ConfigurationSources<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        static () => [
            JsonFileConfigurationSource.FromPathEnvironmentVariable<TConfiguration>(),
            JsonFileConfigurationSource.FromBaseDirectory<TConfiguration>(true),
        ];

    /// <summary>
    /// Creates the default file-system results destination factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <returns>
    /// A factory that creates a results destination from the resolved configuration.
    /// </returns>
    public static Func<TConfiguration, IAllureResultsDestination> Destination<TConfiguration>()
        where TConfiguration : AllureConfiguration
    =>
        static (configuration) => new FileSystemResultsDestination(
            configuration.ResultsDirectory,
            configuration.IndentOutput
        );

    /// <summary>
    /// Creates a rule-based parameter serializer factory for an in-process endpoint.
    /// </summary>
    /// <typeparam name="TRuntime">The runtime type.</typeparam>
    /// <param name="registrations">
    /// The actions that configure serialization rules using the constructed runtime.
    /// </param>
    /// <returns>
    /// A factory that creates a parameter serializer from the constructed runtime.
    /// </returns>
    public static Func<TRuntime, IAllureParameterSerializer> EndpointParameterSerializer<TRuntime>(
        IEnumerable<Action<TRuntime, IParameterSerializationRulesContext>> registrations
    )
        where TRuntime : IAllureRuntimeBase
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
    /// Creates a rule-based parameter serializer factory for a runtime.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <param name="registrations">
    /// The actions that configure serialization rules using the resolved configuration.
    /// </param>
    /// <returns>
    /// A factory that creates a parameter serializer from the resolved configuration.
    /// </returns>
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
    /// Creates the default runtime registration hook factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <typeparam name="TContext">The runtime registration context type.</typeparam>
    /// <returns>
    /// A factory that discovers registration hooks from the environment and the
    /// resolved configuration.
    /// </returns>
    public static Func<TConfiguration, IEnumerable<IAllureRegistrationHook<TContext>?>> RuntimeHookProviders<TConfiguration, TContext>()
        where TConfiguration : AllureConfiguration, new()
        where TContext : IAllureRuntimeRegistrationContext<TConfiguration>
    =>
        static (configuration) => [
            ReflectionHooks.FromEnvironmentVariable<IAllureRegistrationHook<TContext>>("ALLURE_RUNTIME_REGISTRATION_HOOK"),
            ReflectionHooks.FromConfiguration<TConfiguration, IAllureRegistrationHook<TContext>>(configuration),
        ];
}
