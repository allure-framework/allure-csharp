using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureInProcessEndpointRegistrationContext<TConfiguration> : IAllureEndpointRegistrationContext
    where TConfiguration : AllureConfiguration
{
    void UseParameterSerializer(
        Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> serializerFactory
    );

    void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration);

    void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable);

    void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> routeIdsFactory);
}
