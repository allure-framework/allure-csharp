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
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TContext">The registration context type.</typeparam>
/// <typeparam name="THook">The endpoint registration hook type.</typeparam>
public interface IAllureInProcessEndpointIntegrationContext<TConfiguration, TContext, THook> :
    IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TConfiguration : AllureConfiguration
    where TContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TContext>
{
    /// <summary>
    /// Configures the hooks invoked during endpoint registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<THook?>> hooksFactory
    );

    /// <summary>
    /// Configures the predicate used to match the current test or fixture scope.
    /// </summary>
    void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    /// <summary>
    /// Configures the predicate used to match the global scope.
    /// </summary>
    void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    /// <summary>
    /// Configures the operations exposed by the endpoint.
    /// </summary>
    void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory);
}

/// <summary>
/// Configures an in-process endpoint and its integration hooks for an Allure
/// runtime that uses the standard <see cref="AllureConfiguration"/>.
/// </summary>
public interface IAllureInProcessEndpointIntegrationContext :
    IAllureInProcessEndpointIntegrationContext<
        AllureConfiguration,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >,
    IAllureInProcessEndpointRegistrationContext;
