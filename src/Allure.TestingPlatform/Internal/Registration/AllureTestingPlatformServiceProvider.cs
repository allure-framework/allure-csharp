using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformServiceProvider() : IAllureTestingPlatformServiceProvider
{
    public AllureTestingPlatform Value { get; internal set; } = new();
}
