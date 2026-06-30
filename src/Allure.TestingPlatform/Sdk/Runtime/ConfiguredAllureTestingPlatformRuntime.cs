using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Represents a runtime with resolved configuration and logger dependencies.
/// </summary>
/// <param name="Mode">The registration mode.</param>
/// <param name="Phase">The current runtime phase.</param>
/// <param name="Logger">The resolved logger.</param>
/// <param name="Configuration">The resolved Allure configuration.</param>
/// <param name="IsEnabled">Whether the runtime is enabled.</param>
public record class ConfiguredAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    AllureTestingPlatformRuntimePhase Phase,
    ILogger Logger,
    AllureConfiguration Configuration,
    bool IsEnabled
) : AllureTestingPlatformRuntimeState(
    Phase: Phase,
    IsEnabled: IsEnabled
);
