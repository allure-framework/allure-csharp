using System;
using System.Collections.Generic;
using Allure.Abstractions;

namespace Allure.Sdk.Registration;

public interface IAllureEndpointRegistrationContext : IAllureRegistrationContext
{
    void SetAvailabilityPredicate(Func<bool> isAvailable);

    void UseCurrentScopePredicate(Func<bool> predicate);

    void UseGlobalScopePredicate(Func<bool> predicate);

    void UseOperations(Func<AllureOperations> operationsFactory);

    void SuppressRoutes(Func<IEnumerable<string>> routeIdsFactory);
}
