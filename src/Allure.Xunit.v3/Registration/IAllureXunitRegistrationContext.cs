using Allure.TestingPlatform.Sdk.Registration;
using Allure.Xunit.Configuration;

namespace Allure.Xunit.Registration;

public interface IAllureXunitRegistrationContext :
    IAllureTestingPlatformRegistrationContext<AllureXunitConfiguration>;
