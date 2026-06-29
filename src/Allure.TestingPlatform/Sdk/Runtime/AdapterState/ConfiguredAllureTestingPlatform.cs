using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime.AdapterState;

public record class ConfiguredAllureTestingPlatform(
    AllureTestingPlatformRegistrationMode Mode,
    AllureTestingPlatformState State,
    ILogger Logger,
    AllureConfiguration Configuration,
    bool IsEnabled
) : AllureTestingPlatform(
    State: State,
    IsEnabled: IsEnabled
);
