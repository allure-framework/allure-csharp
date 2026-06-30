namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Represents a runtime registration that was suppressed
/// via <c>--allure off</c>.
/// </summary>
/// <param name="Mode">The registration mode.</param>
public sealed record class SuppressedAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode
) : AllureTestingPlatformRuntimeState(
    Phase: AllureTestingPlatformRuntimePhase.Suppressed,
    IsEnabled: false
);
