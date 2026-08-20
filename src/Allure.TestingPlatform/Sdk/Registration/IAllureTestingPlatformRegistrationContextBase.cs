using Allure.Sdk.Registration;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Represents the common registration context for an Allure Microsoft Testing Platform runtime.
/// </summary>
public interface IAllureTestingPlatformRegistrationContextBase :
    IAllureRegistrationContext
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
