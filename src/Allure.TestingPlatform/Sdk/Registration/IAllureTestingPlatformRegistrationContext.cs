using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Provides configuration operations for an Allure Microsoft Testing Platform runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureTestingPlatformRegistrationContext<TConfiguration> :
    IAllureTestingPlatformRegistrationContextBase,
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
{
    /// <summary>
    /// Disables Allure.
    /// </summary>
    void Disable();

    /// <summary>
    /// Disables the process watchdog that writes a global error when the test host crashes.
    /// </summary>
    void DisableHostProcessWatchdog();
}
