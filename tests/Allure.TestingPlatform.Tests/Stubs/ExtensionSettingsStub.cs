using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.Stubs;

public class ExtensionSettingsStub : IAllureExtensionSettings
{
    public bool IsEnabled { get; set; } = true;
}