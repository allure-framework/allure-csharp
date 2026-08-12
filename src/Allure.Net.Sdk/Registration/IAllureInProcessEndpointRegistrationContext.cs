using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an in-process endpoint for an Allure runtime.
/// </summary>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureInProcessEndpointRegistrationContext<out TRuntime> :
    IAllureEndpointRegistrationContext

    where TRuntime : IAllureRuntime
{
    /// <summary>
    /// Configures the parameter serializer used by the endpoint.
    /// </summary>
    void UseParameterSerializer(
        Func<TRuntime, IAllureParameterSerializer> serializerFactory
    );

    /// <summary>
    /// Configures endpoint availability using the constructed runtime.
    /// </summary>
    void ConfigureSerialization(Action<TRuntime, IParameterSerializationRulesContext> registration);

    /// <summary>
    /// Configures endpoint availability using the constructed runtime.
    /// </summary>
    void SetAvailabilityPredicate(Func<TRuntime, bool> isAvailable);

    /// <summary>
    /// Configures suppressed route IDs using the constructed runtime.
    /// </summary>
    void SuppressRoutes(Func<TRuntime, IEnumerable<string>> routeIdsFactory);
}
