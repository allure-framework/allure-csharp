using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformServiceProvider() : IAllureTestingPlatformServiceProvider
{
    public AllureTestingPlatform Value { get; internal set; } = new();
}
