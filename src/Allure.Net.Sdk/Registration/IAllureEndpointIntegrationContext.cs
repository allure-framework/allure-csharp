using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an external Allure runtime endpoint.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureEndpointIntegrationContext<out TContext> : IAllureEndpointRegistrationContext
    where TContext : IAllureEndpointRegistrationContext
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<IEnumerable<IAllureRegistrationHook<TContext>?>> hooksFactory
    );

    /// <summary>
    /// Configures the predicate used to match the current test or fixture scope.
    /// </summary>
    void UseCurrentScopePredicate(Func<bool> predicate);

    /// <summary>
    /// Configures the predicate used to match the global scope.
    /// </summary>
    void UseGlobalScopePredicate(Func<bool> predicate);

    /// <summary>
    /// Configures the operations exposed by the endpoint.
    /// </summary>
    void UseOperations(Func<AllureOperations> operationsFactory);
}

public interface IAllureEndpointIntegrationContext :
    IAllureEndpointIntegrationContext<IAllureEndpointRegistrationContext>;
