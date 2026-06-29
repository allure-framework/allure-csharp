namespace Allure.TestingPlatform.Sdk.Runtime;

public record class AllureTestingPlatformRuntime(
    AllureTestingPlatformRuntimeState State,
    bool IsEnabled
)
{
    public AllureTestingPlatformRuntime() : this(AllureTestingPlatformRuntimeState.NotInitialized, false)
    {
    }
}
