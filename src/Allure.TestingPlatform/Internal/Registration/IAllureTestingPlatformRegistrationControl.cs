using System;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Registration;

interface IAllureTestingPlatformRegistrationControl<out TConfiguration, out TRuntime> :
    IAllureTestingPlatformRegistration<TConfiguration, TRuntime>,
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    void EnsureRuntimeStarted();

    ITestExecutionCoordinator CreateTestExecutionCoordinator();
}
