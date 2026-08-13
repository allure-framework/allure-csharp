using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an in-process endpoint for an Allure runtime.
/// </summary>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureInProcessEndpointRegistrationContext<out TRuntime> :
    IAllureEndpointRegistrationContext

    where TRuntime : IAllureRuntimeBase
{
    /// <summary>
    /// Configures the parameter serializer used by the endpoint.
    /// </summary>
    /// <param name="serializerFactory">
    /// A factory that creates the serializer from the constructed runtime.
    /// </param>
    void UseParameterSerializer(
        Func<TRuntime, IAllureParameterSerializer> serializerFactory
    );

    /// <summary>
    /// Configures parameter serialization rules used by the endpoint.
    /// </summary>
    /// <param name="registration">
    /// An action that configures the serialization rules using the constructed runtime.
    /// </param>
    void ConfigureSerialization(Action<TRuntime, IParameterSerializationRulesContext> registration);

    /// <summary>
    /// Configures endpoint availability using the constructed runtime.
    /// </summary>
    /// <param name="isAvailable">
    /// A function that determines whether the endpoint is available.
    /// </param>
    void SetAvailabilityPredicate(Func<TRuntime, bool> isAvailable);

    /// <summary>
    /// Configures suppressed route IDs using the constructed runtime.
    /// </summary>
    /// <param name="routeIdsFactory">
    /// A factory that returns the route IDs suppressed by this endpoint.
    /// </param>
    void SuppressRoutes(Func<TRuntime, IEnumerable<string>> routeIdsFactory);
}
