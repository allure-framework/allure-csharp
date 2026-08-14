using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.Stubs;

class RuntimeCoordinatorSpy<TConfiguration, TRuntime>(
    IReadOnlyLateBoundReference<TConfiguration> configurationReference,
    IReadOnlyLateBoundReference<TRuntime> runtimeReference
) :
    IAllureTestingPlatformRuntimeControl<TConfiguration, TRuntime>

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    public int EnsureRuntimeStartedCalls { get; private set; } = 0;

    public IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference => configurationReference;

    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference => runtimeReference;

    public bool CanPublish => true;

    public ValueTask DisposeAsync() => default;

    public void EnsureRuntimeStarted()
    {
        this.EnsureRuntimeStartedCalls++;
    }

    public Task PublishAsync(IDataProducer dataProducer, IData data) =>
        Task.CompletedTask;
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
