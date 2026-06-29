using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

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
