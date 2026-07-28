using System;
using System.Collections.Generic;

namespace Allure.Sdk.Registration;

public interface IAllureEndpointRegistrationContext : IAllureRegistrationContext
{
    void SuppressRoutes(Func<IEnumerable<string>> routeIdsFactory);

    void SetAvailabilityPredicate(Func<bool> isAvailable);
}
