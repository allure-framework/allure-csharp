using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Registration;

namespace Allure.TestingPlatform.Registration;

public interface IAllureTestingPlatformRuntimeRegistrationHook :
    IAllureTestingPlatformRuntimeRegistrationHook<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntimeRegistrationContext
    >;
