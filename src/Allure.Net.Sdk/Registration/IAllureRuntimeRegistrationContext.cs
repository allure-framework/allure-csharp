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
    void UseConfigurationSources(
        Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory
    );

    /// <summary>
    /// Configures parameter serialization rules using the resolved configuration.
    /// </summary>
    void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration);

    /// <summary>
    /// Replaces the rule-based parameter serializer with a custom serializer.
    /// </summary>
    void UseParameterSerializer(
        Func<TConfiguration, IAllureParameterSerializer> serializerFactory
    );

    /// <summary>
    /// Configures the destination that receives generated Allure results.
    /// </summary>
    void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory);
}

/// <summary>
/// Configures an Allure runtime that uses the standard
/// <see cref="AllureConfiguration"/> before it is constructed.
/// </summary>
public interface IAllureRuntimeRegistrationContext :
    IAllureRuntimeRegistrationContext<AllureConfiguration>;
