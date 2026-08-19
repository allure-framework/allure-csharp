using Allure.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.Xunit.Configuration;
using Allure.Xunit.Runtime;
using Allure.Xunit.Registration;

namespace Allure.Xunit.Internal.Registration;

class AllureXunitRegistrationSession :
    AllureTestingPlatformRegistrationSession<
        AllureXunitConfiguration,
        AllureXunitRuntime,
        IAllureXunitRegistrationContext,
        IAllureXunitIntegrationContext
    >,
    IAllureXunitIntegrationContext
{
    protected override IAllureXunitIntegrationContext IntegrationContext => this;

    protected override IAllureXunitRegistrationContext RegistrationContext => this;

    protected override AllureXunitRuntime CreateRuntime(
        RuntimeCreationArguments<AllureXunitConfiguration> commonArgs,
        AllureTestingPlatformRuntimeArguments testingPlatformArgs
    ) =>
        new(
            commonArgs,
            testingPlatformArgs,
            AllureRunnerReporter.MessageHandlerReference
        );
}
