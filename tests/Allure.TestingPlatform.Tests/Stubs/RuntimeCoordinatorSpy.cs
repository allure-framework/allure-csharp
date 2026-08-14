using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.Stubs;

class RuntimeCoordinatorSpy<TConfiguration, TRuntime>(
    AllureTestingPlatformConfiguration configuration,
    IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > runtimeReference
) :
    IAllureTestingPlatformRuntimeHandle
{
    public int EnsureRuntimeStartedCalls { get; private set; } = 0;

    public IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > RuntimeReference => runtimeReference;

    public AllureTestingPlatformConfiguration Configuration => configuration;

    public ValueTask DisposeAsync() => default;

    public void EnsureRuntimeStarted()
    {
        this.EnsureRuntimeStartedCalls++;
    }
}

class RuntimeCoordinatorSpy(
    AllureTestingPlatformConfiguration configuration,
    IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > runtimeReference
) :
    RuntimeCoordinatorSpy<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >(configuration, runtimeReference)
{
}
