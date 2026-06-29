using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeProvider() : IAllureTestingPlatformRuntimeProvider
{
    public AllureTestingPlatformRuntime Value { get; internal set; } = new();
}
