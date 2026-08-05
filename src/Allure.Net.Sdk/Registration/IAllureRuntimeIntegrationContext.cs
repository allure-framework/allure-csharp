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
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the integration.</typeparam>
public interface IAllureRuntimeIntegrationContext<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntime
> :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
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
        Func<TConfiguration, IAllureExecutionContext> contextFactory
    );

    /// <summary>
    /// Configures the lifecycle API service.
    /// </summary>
    void UseLifecycleApi(
        Func<TConfiguration, IAllureLifecycleApi> lifecycleApiFactory
    );

    /// <summary>
    /// Configures the model API service.
    /// </summary>
    void UseModelApi(
        Func<TConfiguration, IAllureModelApi> modelApiFactory
    );

    /// <summary>
    /// Registers an in-process endpoint for the constructed runtime.
    /// </summary>
    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<
            TRuntime,
            IAllureInProcessEndpointIntegrationContext<
                TConfiguration,
                TEndpointRegistrationContext,
                TEndpointHook,
                TRuntime
            >
        > endpointRegistration
    );
}

/// <summary>
/// Configures a standard Allure runtime with a custom configuration type,
/// custom registration hook types, and its in-process endpoint integration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
public interface IAllureRuntimeIntegrationContext<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook
> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureRuntime<TConfiguration>
    >

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>;

/// <summary>
/// Configures a standard Allure runtime and its in-process endpoint integration.
/// </summary>
public interface IAllureRuntimeIntegrationContext :
    IAllureRuntimeIntegrationContext<
        AllureConfiguration,
        IAllureRuntimeRegistrationContext,
        IAllureRuntimeRegistrationHook,
        IAllureInProcessEndpointRegistrationContext,
        IAllureInProcessEndpointRegistrationHook
    >,
    IAllureRuntimeRegistrationContext;
