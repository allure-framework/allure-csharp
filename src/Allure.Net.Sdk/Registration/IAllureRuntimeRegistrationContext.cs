using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an Allure runtime before it is constructed.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntimeRegistrationContext<TConfiguration> :
    IAllureRegistrationContext
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Configures the ordered sources used to resolve runtime configuration.
    /// </summary>
    /// <param name="sourcesFactory">
    /// A factory that returns an ordered sequence of sources. The first
    /// source that can load configuration is used.
    /// </param>
    void UseConfigurationSources(
        Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory
    );

    /// <summary>
    /// Adds a transformation to apply to the loaded runtime configuration.
    /// </summary>
    /// <param name="transformation">
    /// A function that receives the current configuration and returns the
    /// configuration to pass to the next transformation or use to construct
    /// the runtime.
    /// </param>
    /// <remarks>
    /// Transformations are applied in registration order after configuration
    /// sources have been resolved.
    /// </remarks>
    void TransformConfiguration(
        Func<TrackedConfiguration<TConfiguration>, TrackedConfiguration<TConfiguration>> transformation
    );

    /// <summary>
    /// Configures parameter serialization rules using the resolved configuration.
    /// </summary>
    /// <param name="registration">An action that configures the serialization rules.</param>
    void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration);

    /// <summary>
    /// Replaces the rule-based parameter serializer with a custom serializer.
    /// </summary>
    /// <param name="serializerFactory">
    /// A factory that creates the serializer from the resolved configuration.
    /// </param>
    void UseParameterSerializer(
        Func<TConfiguration, IAllureParameterSerializer> serializerFactory
    );

    /// <summary>
    /// Configures the destination that receives generated Allure results.
    /// </summary>
    /// <param name="destinationFactory">
    /// A factory that creates the destination from the resolved configuration.
    /// </param>
    void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory);
}
