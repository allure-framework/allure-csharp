using Allure.Sdk.Configuration;

namespace Allure.TestingPlatform.Configuration;

public record class AllureTestingPlatformConfiguration : AllureConfiguration
{
    public bool IsEnabled { get; init; } = true;

    public bool IsProcessWatchdogEnabled { get; init; } = true;
}
