using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.Stubs;

public record class AllureRuntimeProviderStub(
    AllureTestingPlatformRuntime Value
): IAllureTestingPlatformRuntimeProvider;
