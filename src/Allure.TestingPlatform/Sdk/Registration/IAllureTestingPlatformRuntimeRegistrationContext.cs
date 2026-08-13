using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration> :
    IAllureTestingPlatformRuntimeRegistrationContextBase,
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
