using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Runtime;

public interface IAllureTestingPlatformServiceProvider
{
    AllureTestingPlatform Value { get; }
}
