using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

public sealed record class DisabledAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration
) : ConfiguredAllureTestingPlatformRuntime(
    Mode: Mode,
    Phase: AllureTestingPlatformRuntimePhase.Disabled,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: false
);
