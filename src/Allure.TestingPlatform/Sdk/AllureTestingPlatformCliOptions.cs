using Microsoft.Testing.Platform.CommandLine;

namespace Allure.TestingPlatform.Sdk;

public static class AllureTestingPlatformCliOptions
{
    public static bool? GetAllureToggleValue(ICommandLineOptions options) =>
        options.TryGetOptionArgumentList("allure", out var values)
            ? values[0] == "on"
            : null;

    public static bool IsAllureEnabled(ICommandLineOptions options) =>
        !options.TryGetOptionArgumentList("allure", out var values)
            || values[0] == "on";
}
