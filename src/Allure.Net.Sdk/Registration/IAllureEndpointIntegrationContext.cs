using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an external Allure runtime endpoint and its integration hooks.
/// </summary>
/// <typeparam name="TContext">The registration context type.</typeparam>
/// <typeparam name="THook">The endpoint registration hook type.</typeparam>
public interface IAllureEndpointIntegrationContext<TContext, THook> : IAllureEndpointRegistrationContext
    where TContext : IAllureEndpointRegistrationContext
    where THook : IAllureEndpointRegistrationHook<TContext>
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<IEnumerable<THook?>> hooksFactory
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

/// <summary>
/// Configures an external Allure runtime endpoint and its integration hooks
/// through the standard endpoint registration context.
/// </summary>
public interface IAllureEndpointIntegrationContext :
    IAllureEndpointIntegrationContext<IAllureEndpointRegistrationContext, IAllureEndpointRegistrationHook>;
