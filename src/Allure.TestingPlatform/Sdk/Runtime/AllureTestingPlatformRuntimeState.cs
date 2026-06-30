namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Represents the current Allure.TestingPlatform runtime state.
/// </summary>
/// <param name="Phase">The current runtime phase.</param>
/// <param name="IsEnabled">Whether the runtime is enabled.</param>
public record class AllureTestingPlatformRuntimeState(
    AllureTestingPlatformRuntimePhase Phase,
    bool IsEnabled
)
{
    /// <summary>
    /// Creates the default not-initialized runtime state.
    /// </summary>
    public AllureTestingPlatformRuntimeState() :
        this(AllureTestingPlatformRuntimePhase.NotInitialized, false)
    {
    }
}
