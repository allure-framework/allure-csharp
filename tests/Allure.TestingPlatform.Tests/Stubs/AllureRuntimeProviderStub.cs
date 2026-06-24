using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureRuntimeProviderStub(AllureRuntimeStub runtime) : IAllureRuntimeProvider
{
    public IAllureRuntime Runtime => runtime;
}