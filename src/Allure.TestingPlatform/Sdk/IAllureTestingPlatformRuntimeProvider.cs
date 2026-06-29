using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureTestingPlatformRuntimeProvider
{
    AllureTestingPlatformRuntime Value { get; }
}
