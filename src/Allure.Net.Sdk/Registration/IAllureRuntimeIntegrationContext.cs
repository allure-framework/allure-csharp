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
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the integration.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntimeHook, TEndpointHook, TRuntime> :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
    where TRuntime : IAllureRuntime<TConfiguration>
{
    /// <summary>
    /// Configures the hooks invoked during runtime registration.
    /// </summary>
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<TRuntimeHook?>> hooksFactory
    );

    /// <summary>
    /// Configures the execution-context service.
    /// </summary>
    void UseContext(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> contextFactory
    );

    /// <summary>
    /// Configures the lifecycle API service.
    /// </summary>
    void UseLifecycleApi(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> lifecycleApiFactory
    );

    /// <summary>
    /// Configures the model API service.
    /// </summary>
    void UseModelApi(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> modelApiFactory
    );

    /// <summary>
    /// Registers an in-process endpoint for the constructed runtime.
    /// </summary>
    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<TRuntime, IAllureInProcessEndpointIntegrationContext<TConfiguration, TEndpointHook>> endpointRegistration
    );
}

/// <summary>
/// Configures a standard Allure runtime with a custom configuration type,
/// custom registration hook types, and its in-process endpoint integration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntimeHook, TEndpointHook> :
    IAllureRuntimeIntegrationContext<TConfiguration, TRuntimeHook, TEndpointHook, IAllureRuntime<TConfiguration>>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration>;

/// <summary>
/// Configures a standard Allure runtime and its in-process endpoint integration.
/// </summary>
public interface IAllureRuntimeIntegrationContext :
    IAllureRuntimeIntegrationContext<
        AllureConfiguration,
        IAllureRuntimeRegistrationHook,
        IAllureInProcessEndpointRegistrationHook,
        IAllureRuntime<AllureConfiguration>
    >;
