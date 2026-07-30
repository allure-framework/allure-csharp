using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an in-process endpoint for an Allure runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime> :
    IAllureEndpointRegistrationContext

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    /// <summary>
    /// Configures the parameter serializer used by the endpoint.
    /// </summary>
    void UseParameterSerializer(
        Func<TRuntime, IAllureParameterSerializer> serializerFactory
    );

    /// <summary>
    /// Configures parameter serialization rules using the resolved runtime configuration.
    /// </summary>
    void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration);

    /// <summary>
    /// Configures endpoint availability using the constructed runtime.
    /// </summary>
    void SetAvailabilityPredicate(Func<TRuntime, bool> isAvailable);

    /// <summary>
    /// Configures suppressed route IDs using the constructed runtime.
    /// </summary>
    void SuppressRoutes(Func<TRuntime, IEnumerable<string>> routeIdsFactory);
}

/// <summary>
/// Configures an in-process endpoint for a standard Allure runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureInProcessEndpointRegistrationContext<TConfiguration> :
    IAllureInProcessEndpointRegistrationContext<TConfiguration, IAllureRuntime<TConfiguration>>

    where TConfiguration : AllureConfiguration;

/// <summary>
/// Configures an in-process endpoint for an Allure runtime that uses the
/// standard <see cref="AllureConfiguration"/>.
/// </summary>
public interface IAllureInProcessEndpointRegistrationContext :
    IAllureInProcessEndpointRegistrationContext<AllureConfiguration>;
