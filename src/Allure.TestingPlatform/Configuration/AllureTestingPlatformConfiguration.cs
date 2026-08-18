using Allure.Sdk.Configuration;

namespace Allure.TestingPlatform.Configuration;

/// <summary>
/// Defines configuration options for the Allure Microsoft Testing Platform integration.
/// </summary>
public record class AllureTestingPlatformConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets a value indicating whether the Allure integration is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether an Allure global error is written when the test host
    /// process exits unexpectedly.
    /// </summary>
    public bool IsProcessWatchdogEnabled { get; init; } = true;
}
