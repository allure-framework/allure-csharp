using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

public interface IAllureEndpointRegistrationContext : IAllureRegistrationContext
{
    void SuppressRoutes(Func<IEnumerable<string>> routeIdsFactory);

    void SetAvailabilityPredicate(Func<bool> isAvailable);
}

public interface IAllureEndpointRegistrationContext<THook> : IAllureEndpointRegistrationContext
    where THook : IAllureEndpointRegistrationHook
{
    void UseRegistrationHooks(
        Func<IEnumerable<THook?>> hooksFactory
    );

    void UseCurrentScopePredicate(Func<bool> predicate);

    void UseGlobalScopePredicate(Func<bool> predicate);

    void UseOperations(Func<AllureOperations> operationsFactory);
}
