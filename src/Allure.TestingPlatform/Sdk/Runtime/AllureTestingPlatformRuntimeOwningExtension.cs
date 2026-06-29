using System.Threading.Tasks;

namespace Allure.TestingPlatform.Sdk.Runtime;

public abstract class AllureTestingPlatformRuntimeOwningExtension(
    string uid,
    string displayName,
    string description,
    IAllureTestingPlatformRuntimeOwner runtimeOwner
) :
    AllureTestingPlatformExtension(
        uid,
        displayName,
        description,
        runtimeOwner.RuntimeProvider
    )
{
    public override Task<bool> IsEnabledAsync()
    {
        if (runtimeOwner is { RuntimeProvider.Value.State: AllureTestingPlatformRuntimeState.NotInitialized })
        {
            runtimeOwner.Configure();
        }

        return base.IsEnabledAsync();
    }

    protected AllureTestingPlatformRuntime EnsureBuilt()
    {
        var state = runtimeOwner.RuntimeProvider.Value;
        if (state is { State: AllureTestingPlatformRuntimeState.Configured })
        {
            state = runtimeOwner.Build();
        }
        return state;
    }
}
