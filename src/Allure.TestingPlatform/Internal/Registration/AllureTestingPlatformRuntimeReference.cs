using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeReference() : IAllureTestingPlatformRuntimeReference
{
    public AllureTestingPlatformRuntimeState CurrentRuntime { get; internal set; } = new();
}
