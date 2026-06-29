using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Internal.Registration;

public interface IAllureTestingPlatformBuilder
{
    public IAllureTestingPlatformServiceProvider StateProvider { get; }

    AllureTestingPlatform Configure();

    AllureTestingPlatform Build();
}
