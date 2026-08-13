using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Base class for extensions that configure or start the Allure.TestingPlatform runtime.
/// It is expected to have exactly one such extension per process.
/// The runtime of the test host process is managed by
/// <see cref="TestingPlatformExtensions.AllureTestingPlatformTestHostRuntimeController{TConfiguration, TRuntime}"/>
/// by default.
/// </summary>
public abstract class AllureTestingPlatformRuntimeControllerExtension<TConfiguration, TRuntime>(
    string uid,
    string displayName,
    string description,
    IAllureRuntimeRegistrationPlan<
        TConfiguration,
        TRuntime
    > runtimeRegistrationPlan
) :
    AllureTestingPlatformExtension<TConfiguration, TRuntime>(
        uid,
        displayName,
        description,
        runtimeRegistrationPlan.RuntimeReference
    )

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
{
    IAllureRuntimeRegistration<TRuntime>? registration = null;

    protected bool IsStarted => this.registration is not null;

    protected IAllureRuntimeRegistration<TRuntime> Registration => registration ??
        this.EnsureRuntimeStarted();

    new protected AllureTestingPlatformConfiguration Configuration =>
        runtimeRegistrationPlan.Configuration;

    /// <inheritdoc />
    public override Task<bool> IsEnabledAsync() =>
        Task.FromResult(runtimeRegistrationPlan.Configuration.IsEnabled);

    protected IAllureRuntimeRegistration<TRuntime> EnsureRuntimeStarted()
    {
        return this.registration ??= runtimeRegistrationPlan.Build();
    }
}
