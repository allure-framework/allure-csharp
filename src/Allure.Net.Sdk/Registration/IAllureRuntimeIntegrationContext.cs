using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an Allure runtime and its in-process endpoint integration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the integration.</typeparam>
/// <typeparam name="TContext">The type of runtime integration context.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, out TContext> :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TContext : IAllureRuntimeRegistrationContext<TConfiguration>
{
    /// <summary>
    /// Configures the hooks invoked during runtime registration.
    /// </summary>
    /// <param name="hooksFactory">
    /// A factory that creates the hooks from the initially resolved configuration.
    /// </param>
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<IAllureRegistrationHook<TContext>?>> hooksFactory
    );

    /// <summary>
    /// Configures the execution-context service.
    /// </summary>
    /// <param name="contextFactory">
    /// A factory that creates the service from the resolved configuration.
    /// </param>
    void UseContext(
        Func<TConfiguration, IAllureExecutionContext> contextFactory
    );

    /// <summary>
    /// Configures the lifecycle API service.
    /// </summary>
    /// <param name="lifecycleApiFactory">
    /// A factory that creates the service from the resolved configuration.
    /// </param>
    void UseLifecycleApi(
        Func<TConfiguration, IAllureLifecycleApi> lifecycleApiFactory
    );

    /// <summary>
    /// Configures the model API service.
    /// </summary>
    /// <param name="modelApiFactory">
    /// A factory that creates the service from the resolved configuration.
    /// </param>
    void UseModelApi(
        Func<TConfiguration, IAllureModelApi> modelApiFactory
    );

    /// <summary>
    /// Registers an in-process endpoint for the constructed runtime.
    /// </summary>
    /// <param name="endpointId">The route identifier of the endpoint.</param>
    /// <param name="endpointRegistration">
    /// An action that configures the endpoint after the runtime is constructed.
    /// </param>
    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TRuntime>> endpointRegistration
    );
}

public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntime> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        IAllureRuntimeRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>;

/// <summary>
/// Configures a standard Allure runtime with a custom configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        IAllureRuntime<TConfiguration>
    >

    where TConfiguration : AllureConfiguration, new();

/// <summary>
/// Configures a standard Allure runtime and its in-process endpoint integration.
/// </summary>
public interface IAllureRuntimeIntegrationContext :
    IAllureRuntimeIntegrationContext<AllureConfiguration>;
