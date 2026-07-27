using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

public interface IAllureEndpointRegistrationContext<THook> : IAllureRegistrationContext
    where THook : IAllureEndpointRegistrationHook
{
    void UseRegistrationHooks(
        Func<IEnumerable<IAllureEndpointRegistrationHookProvider<THook>>> hookProvidersFactory
    );

    void SetAvailabilityPredicate(Func<bool> isAvailable);

    void UseCurrentScopePredicate(Func<bool> predicate);

    void UseGlobalScopePredicate(Func<bool> predicate);

    void UseOperations(Func<AllureOperations> operationsFactory);

    void SuppressRoutes(Func<IEnumerable<string>> routeIdsFactory);
}
