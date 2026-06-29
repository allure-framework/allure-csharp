using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.Stubs;

public record class AllureStateProviderStub(
    AllureTestingPlatformRuntime Value
): IAllureTestingPlatformRuntimeProvider;
