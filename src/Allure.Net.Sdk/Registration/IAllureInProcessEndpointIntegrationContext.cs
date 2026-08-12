using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an in-process Allure endpoint and its integration hooks.
/// </summary>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureInProcessEndpointIntegrationContext<out TRuntime> :
    IAllureInProcessEndpointRegistrationContext<TRuntime>

    where TRuntime : IAllureRuntimeBase
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<
            TRuntime,
            IEnumerable<
                IAllureRegistrationHook<
                    IAllureInProcessEndpointRegistrationContext<TRuntime>
                >?
            >
        > hooksFactory
    );

    /// <summary>
    /// Configures the predicate used to match the current test or fixture scope.
    /// </summary>
    void UseCurrentScopePredicate(Func<TRuntime, bool> predicate);

    /// <summary>
    /// Configures the predicate used to match the global scope.
    /// </summary>
    void UseGlobalScopePredicate(Func<TRuntime, bool> predicate);

    /// <summary>
    /// Configures the operations exposed by the endpoint.
    /// </summary>
    void UseOperations(Func<TRuntime, AllureInProcessOperations> operationsFactory);
}
