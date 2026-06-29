using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk;

public interface IAllureTestingPlatformRuntimeReference
{
    AllureTestingPlatformRuntimeState CurrentRuntime { get; }
}
