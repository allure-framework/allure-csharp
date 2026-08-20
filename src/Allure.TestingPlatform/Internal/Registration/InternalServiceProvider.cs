using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Internal.Lifecycle;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class InternalServiceProvider<TConfiguration>
{
    Func<TConfiguration, ITestExecutionCoordinator> currentTestExecutionCoordinatorFactory =
        (_) => DirectTestExecutionCoordinator.Instance;

    Action<TConfiguration, IAllureEndpointRegistrationContext>? currentEndpointConfigurationCallback = null;

    public void UseTestExecutionCoordinator(
        Func<TConfiguration, ITestExecutionCoordinator> testExecutionCoordinatorFactory
    ) =>
        this.currentTestExecutionCoordinatorFactory = testExecutionCoordinatorFactory;

    public void UseEndpointConfigurationCallback(
        Action<TConfiguration, IAllureEndpointRegistrationContext> endpointConfigurationCallback
    ) =>
        this.currentEndpointConfigurationCallback = endpointConfigurationCallback;

    public ITestExecutionCoordinator CreateTestExecutionCoordinator(
        TConfiguration configuration
    ) =>
        this.currentTestExecutionCoordinatorFactory(configuration);

    public void ConfigureEndpoint(
        TConfiguration configuration,
        IAllureEndpointRegistrationContext context
    ) =>
        this.currentEndpointConfigurationCallback?.Invoke(configuration, context);
}
