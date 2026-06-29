namespace Allure.TestingPlatform.Sdk.Runtime;

public record class AllureTestingPlatformRuntimeState(
    AllureTestingPlatformRuntimePhase Phase,
    bool IsEnabled
)
{
    public AllureTestingPlatformRuntimeState() : this(AllureTestingPlatformRuntimePhase.NotInitialized, false)
    {
    }
}
