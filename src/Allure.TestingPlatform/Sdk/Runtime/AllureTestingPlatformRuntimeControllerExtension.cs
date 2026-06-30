using System.Threading.Tasks;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Base class for extensions that configure or start the Allure.TestingPlatform runtime.
/// It is expected to have exactly one such extension per process.
/// The runtime of the test host process is managed by
/// <see cref="TestingPlatformExtensions.AllureTestingPlatformInProcessRuntimeController"/>
/// by default.
/// </summary>
public abstract class AllureTestingPlatformRuntimeControllerExtension(
    string uid,
    string displayName,
    string description,
    IAllureTestingPlatformRuntimeController runtimeController
) :
    AllureTestingPlatformExtension(
        uid,
        displayName,
        description,
        runtimeController.RuntimeReference
    )
{
    /// <inheritdoc />
    public override Task<bool> IsEnabledAsync()
    {
        if (runtimeController is { RuntimeReference.CurrentRuntime.Phase: AllureTestingPlatformRuntimePhase.NotInitialized })
        {
            runtimeController.Configure();
        }

        return base.IsEnabledAsync();
    }

    /// <summary>
    /// Gets the runtime controller used by this extension.
    /// </summary>
    protected IAllureTestingPlatformRuntimeController Controller => runtimeController;

    /// <summary>
    /// If the runtime is configured, starts it. Otherwise, does nothing.
    /// </summary>
    /// <returns>The current runtime state.</returns>
    protected AllureTestingPlatformRuntimeState EnsureRuntimeStarted()
    {
        var runtime = this.Controller.RuntimeReference.CurrentRuntime;
        if (runtime is { Phase: AllureTestingPlatformRuntimePhase.Configured })
        {
            runtime = runtimeController.Start();
        }
        return runtime;
    }
}
