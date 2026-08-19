using Allure.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.Xunit.Configuration;
using Allure.Xunit.Internal;

namespace Allure.Xunit.Runtime;

public class AllureXunitRuntime : AllureTestingPlatformRuntime<AllureXunitConfiguration>
{
    readonly IReadOnlyLateBoundReference<AllureMessageHandler> messageHandlerReference;

    internal AllureXunitRuntime(
        RuntimeCreationArguments<AllureXunitConfiguration> commonArgs,
        AllureTestingPlatformRuntimeArguments testingPlatformArgs,
        IReadOnlyLateBoundReference<AllureMessageHandler> xunitMessageHandlerReference
    ) : base(commonArgs, testingPlatformArgs)
    {
        this.messageHandlerReference = xunitMessageHandlerReference;
    }

    internal AllureMessageHandler XunitMessageHandler => this.messageHandlerReference.Value;
}
