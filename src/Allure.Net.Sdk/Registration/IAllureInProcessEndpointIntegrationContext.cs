using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an in-process Allure endpoint and its integration hooks.
/// </summary>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
/// <typeparam name="TContext">The endpoint registration context type.</typeparam>
public interface IAllureInProcessEndpointIntegrationContext<out TRuntime, out TContext> :
    IAllureInProcessEndpointRegistrationContext<TRuntime>

    where TRuntime : IAllureRuntime
    where TContext : IAllureInProcessEndpointRegistrationContext<TRuntime>
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<TRuntime, IEnumerable<IAllureRegistrationHook<TContext>?>> hooksFactory
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

public interface IAllureInProcessEndpointIntegrationContext<TRuntime> :
    IAllureInProcessEndpointIntegrationContext<
        TRuntime,
        IAllureInProcessEndpointRegistrationContext<TRuntime>
    >

    where TRuntime : IAllureRuntime;

public interface IAllureInProcessEndpointIntegrationContext :
    IAllureInProcessEndpointIntegrationContext<IAllureRuntime<AllureConfiguration>>;
