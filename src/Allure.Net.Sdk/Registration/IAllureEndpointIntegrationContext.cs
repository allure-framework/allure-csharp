using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Registration.Hooks;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an external Allure runtime endpoint and its integration hooks.
/// </summary>
/// <typeparam name="THook">The endpoint registration hook type.</typeparam>
public interface IAllureEndpointIntegrationContext<THook> : IAllureEndpointRegistrationContext
    where THook : IAllureEndpointRegistrationHook
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
