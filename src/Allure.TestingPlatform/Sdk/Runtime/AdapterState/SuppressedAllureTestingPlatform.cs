namespace Allure.TestingPlatform.Sdk.Runtime.AdapterState;

public sealed record class SuppressedAllureTestingPlatform(
    AllureTestingPlatformRegistrationMode Mode
) : AllureTestingPlatform(
    AllureTestingPlatformState.Suppressed,
    false
);
