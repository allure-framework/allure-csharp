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
    /// <param name="hooksFactory">
    /// A factory that creates the registration hooks from the constructed runtime.
    /// </param>
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
    /// <param name="predicate">
    /// A function that uses the constructed runtime to determine whether the
    /// endpoint matches the current scope.
    /// </param>
    void UseCurrentScopePredicate(Func<TRuntime, bool> predicate);

    /// <summary>
    /// Configures the predicate used to match the global scope.
    /// </summary>
    /// <param name="predicate">
    /// A function that uses the constructed runtime to determine whether the
    /// endpoint matches the global scope.
    /// </param>
    void UseGlobalScopePredicate(Func<TRuntime, bool> predicate);

    /// <summary>
    /// Configures the operations exposed by the endpoint.
    /// </summary>
    /// <param name="operationsFactory">
    /// A factory that creates the endpoint operations from the constructed runtime.
    /// </param>
    void UseOperations(Func<TRuntime, AllureInProcessOperations> operationsFactory);
}
