namespace Allure.TestingPlatform.Sdk.Runtime;

public sealed record class SuppressedAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode
) : AllureTestingPlatformRuntime(
    AllureTestingPlatformRuntimeState.Suppressed,
    false
);
