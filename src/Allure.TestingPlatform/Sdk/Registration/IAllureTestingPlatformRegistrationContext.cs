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

    where TConfiguration : AllureTestingPlatformConfiguration;
