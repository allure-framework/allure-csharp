using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Implementation;

public record class AllureExtensionSettings(
    bool IsEnabled
) : IAllureExtensionSettings;
