using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable);

    void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory);

    void UseParameterSerializer(
        Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> serializerFactory
    );

    void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> routeIdsFactory);
}
