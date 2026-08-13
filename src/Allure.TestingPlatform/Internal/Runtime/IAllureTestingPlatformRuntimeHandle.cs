using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Runtime;

public interface IAllureTestingPlatformRuntimeHandle
{
    AllureTestingPlatformConfiguration Configuration { get; }

    IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > RuntimeReference { get; }

    void EnsureRuntimeStarted();
}
