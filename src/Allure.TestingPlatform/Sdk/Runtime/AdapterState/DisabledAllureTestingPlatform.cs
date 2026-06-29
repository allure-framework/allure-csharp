using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime.AdapterState;

public sealed record class DisabledAllureTestingPlatform(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration
) : ConfiguredAllureTestingPlatform(
    Mode: Mode,
    State: AllureTestingPlatformState.Disabled,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: false
);
