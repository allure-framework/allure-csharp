namespace Allure.TestingPlatform.Sdk.Runtime;

public sealed record class SuppressedAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode
) : AllureTestingPlatformRuntimeState(
    Phase: AllureTestingPlatformRuntimePhase.Suppressed,
    IsEnabled: false
);
