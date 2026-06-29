using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

public sealed record class EnabledAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration
) : ConfiguredAllureTestingPlatformRuntime(
    Mode: Mode,
    State: AllureTestingPlatformRuntimeState.Configured,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: true
);
