using System.Threading.Tasks;

namespace Allure.TestingPlatform.Sdk.Runtime;

public abstract class AllureTestingPlatformRuntimeResponsibleExtension(
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
    public override Task<bool> IsEnabledAsync()
    {
        if (runtimeController is { RuntimeReference.CurrentRuntime.Phase: AllureTestingPlatformRuntimePhase.NotInitialized })
        {
            runtimeController.Configure();
        }

        return base.IsEnabledAsync();
    }

    protected IAllureTestingPlatformRuntimeController Controller => runtimeController;

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
