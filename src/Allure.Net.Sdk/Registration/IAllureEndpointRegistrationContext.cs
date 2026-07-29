using System;
using System.Collections.Generic;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an Allure runtime endpoint.
/// </summary>
public interface IAllureEndpointRegistrationContext : IAllureRegistrationContext
{
    /// <summary>
    /// Configures the route IDs suppressed by this endpoint.
    /// </summary>
    void SuppressRoutes(Func<IEnumerable<string>> routeIdsFactory);

    /// <summary>
    /// Configures the predicate that determines whether the endpoint is available.
    /// </summary>
    void SetAvailabilityPredicate(Func<bool> isAvailable);
}
