using Allure.Net.Commons.Configuration;

namespace Allure.TestingPlatform.Sdk;

public record class AllureRegistrationResult(
    IAllureExtensionSettings ExtensionSettings,
    AllureConfiguration Configuration
);