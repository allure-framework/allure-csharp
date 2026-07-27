using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureInProcessEndpointRegistrationContext<TConfiguration> : IAllureEndpointRegistrationContext
    where TConfiguration : AllureConfiguration
{
    void UseParameterSerializer(
        Func<IAllureRuntime<TConfiguration>, IAllureParameterSerializer> serializerFactory
    );

    void SetAvailabilityPredicate(Func<IAllureRuntime<TConfiguration>, bool> isAvailable);

    void SuppressRoutes(Func<IAllureRuntime<TConfiguration>, IEnumerable<string>> routeIdsFactory);
}

public interface IAllureInProcessEndpointRegistrationContext<TConfiguration, THook> :
    IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TConfiguration : AllureConfiguration
    where THook : IAllureEndpointRegistrationHook
{
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<IAllureEndpointRegistrationHookProvider<THook>>> hookProvidersFactory
    );

    void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory);
}
