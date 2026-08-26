using Allure.TestingPlatform.Sdk.Registration;
using Allure.Xunit.Configuration;
using Allure.Xunit.Runtime;
using Allure.Xunit.Registration;

namespace Allure.Xunit.Internal.Registration;

interface IAllureXunitIntegrationContext :
    IAllureTestingPlatformIntegrationContext<
        AllureXunitConfiguration,
        AllureXunitRuntime,
        IAllureXunitRegistrationContext
    >,
    IAllureXunitRegistrationContext;
