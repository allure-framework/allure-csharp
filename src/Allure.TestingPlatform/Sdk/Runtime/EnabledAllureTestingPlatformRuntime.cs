using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Represents a configured runtime that is ready to start.
/// </summary>
/// <param name="Mode">The registration mode.</param>
/// <param name="Logger">The resolved logger.</param>
/// <param name="Configuration">The resolved Allure configuration.</param>
public sealed record class EnabledAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration
) : ConfiguredAllureTestingPlatformRuntime(
    Mode: Mode,
    Phase: AllureTestingPlatformRuntimePhase.Configured,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: true
);
