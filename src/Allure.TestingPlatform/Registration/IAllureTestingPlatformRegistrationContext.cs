using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Registration;

namespace Allure.TestingPlatform.Registration;

/// <summary>
/// Provides configuration operations for the default Allure.TestingPlatform runtime.
/// </summary>
public interface IAllureTestingPlatformRegistrationContext :
    IAllureTestingPlatformRegistrationContext<AllureTestingPlatformConfiguration>;
