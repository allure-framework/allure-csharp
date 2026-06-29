namespace Allure.TestingPlatform.Sdk.Runtime.AdapterState;

public record class AllureTestingPlatform(
    AllureTestingPlatformState State,
    bool IsEnabled
)
{
    public AllureTestingPlatform() : this(AllureTestingPlatformState.NotInitialized, false)
    {
    }
}
