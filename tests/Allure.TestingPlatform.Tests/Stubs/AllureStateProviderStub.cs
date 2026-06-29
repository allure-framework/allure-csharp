using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Tests.Stubs;

public record class AllureStateProviderStub(
    AllureTestingPlatform Value
): IAllureTestingPlatformServiceProvider;
