using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Lifecycle;
using Allure.TestingPlatform.Internal.Registration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.Stubs;

class RuntimeCoordinatorSpy<TConfiguration, TRuntime>(
    IReadOnlyLateBoundReference<TConfiguration> configurationReference,
    IReadOnlyLateBoundReference<TRuntime> runtimeReference
) :
    IAllureTestingPlatformRegistrationControl<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    public int EnsureRuntimeStartedCalls { get; private set; } = 0;

    public IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference => configurationReference;

    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference => runtimeReference;

    public IAllureTestingPlatformMessageChannel MessageChannel => IAllureTestingPlatformMessageChannel.Mock();

    public void ConfigureEndpoint(IAllureEndpointRegistrationContext context)
    {
    }

    public ITestExecutionCoordinator CreateTestExecutionCoordinator()
    {
        return DirectTestExecutionCoordinator.Instance;
    }

    public ValueTask DisposeAsync() => default;

    public void EnsureRuntimeStarted()
    {
        this.EnsureRuntimeStartedCalls++;
    }
}

class RuntimeCoordinatorSpy(
    IReadOnlyLateBoundReference<AllureTestingPlatformConfiguration> configurationReference,
    IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > runtimeReference
) :
    RuntimeCoordinatorSpy<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >(configurationReference, runtimeReference)
{
}
