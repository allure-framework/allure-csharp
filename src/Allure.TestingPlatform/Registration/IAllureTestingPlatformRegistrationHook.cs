using Allure.TestingPlatform.Sdk.Registration;

namespace Allure.TestingPlatform.Registration;

/// <summary>
/// Represents a registration hook for the default Allure.TestingPlatform runtime.
/// </summary>
public interface IAllureTestingPlatformRegistrationHook :
    IAllureTestingPlatformRegistrationHook<
        IAllureTestingPlatformRegistrationContext
    >;
