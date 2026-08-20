using System;
using Allure.TestingPlatform.Internal.Lifecycle;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class TestExecutionCoordinatorProvider<TConfiguration>
{
    Func<TConfiguration, ITestExecutionCoordinator> factory =
        (_) => DirectTestExecutionCoordinator.Instance;

    public void Use(
        Func<TConfiguration, ITestExecutionCoordinator> factory
    ) =>
        this.factory = factory;

    public ITestExecutionCoordinator Create(
        TConfiguration configuration
    ) =>
        this.factory(configuration);
}
