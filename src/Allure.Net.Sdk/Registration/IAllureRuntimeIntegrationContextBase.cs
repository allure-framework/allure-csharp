using System;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;
using Allure.Sdk.TestPlan;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures the services and optional in-process endpoint of an Allure runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the integration.</typeparam>
public interface IAllureRuntimeIntegrationContextBase<TConfiguration, out TRuntime> :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
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
    /// Configures the test plan.
    /// </summary>
    /// <param name="testPlanFactory">
    /// A factory that resolves the test plan from the resolved configuration.
    /// </param>
    public void UseTestPlan(
        Func<TConfiguration, AllureTestPlan> testPlanFactory
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
