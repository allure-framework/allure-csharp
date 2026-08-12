using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an external Allure runtime endpoint and its registration hooks.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureEndpointIntegrationContext<out TContext> : IAllureEndpointRegistrationContext
    where TContext : IAllureEndpointRegistrationContext
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    /// <param name="hooksFactory">A factory that creates the registration hooks.</param>
    void UseRegistrationHooks(
        Func<IEnumerable<IAllureRegistrationHook<TContext>?>> hooksFactory
    );

    /// <summary>
    /// Configures the predicate used to match the current test or fixture scope.
    /// </summary>
    /// <param name="predicate">
    /// A function that returns <see langword="true"/> when the endpoint matches
    /// the current scope.
    /// </param>
    void UseCurrentScopePredicate(Func<bool> predicate);

    /// <summary>
    /// Configures the predicate used to match the global scope.
    /// </summary>
    /// <param name="predicate">
    /// A function that returns <see langword="true"/> when the endpoint matches
    /// the global scope.
    /// </param>
    void UseGlobalScopePredicate(Func<bool> predicate);

    /// <summary>
    /// Configures the operations exposed by the endpoint.
    /// </summary>
    /// <param name="operationsFactory">A factory that creates the endpoint operations.</param>
    void UseOperations(Func<AllureOperations> operationsFactory);
}

/// <summary>
/// Configures an external Allure runtime endpoint using the standard endpoint
/// registration context.
/// </summary>
public interface IAllureEndpointIntegrationContext :
    IAllureEndpointIntegrationContext<IAllureEndpointRegistrationContext>;
