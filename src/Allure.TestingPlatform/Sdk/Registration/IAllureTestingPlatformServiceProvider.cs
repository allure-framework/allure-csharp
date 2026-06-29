using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformServiceProvider
{
    AllureTestingPlatform Value { get; }
}
