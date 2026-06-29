using Allure.Net.Commons.Configuration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

public record class ConfiguredAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    AllureTestingPlatformRuntimeState State,
    ILogger Logger,
    AllureConfiguration Configuration,
    bool IsEnabled
) : AllureTestingPlatformRuntime(
    State: State,
    IsEnabled: IsEnabled
);
