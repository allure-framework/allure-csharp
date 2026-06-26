using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.Stubs;

public class AllureRuntimeProviderStub(AllureRuntimeStub runtime) : IAllureRuntimeProvider
{
    public IAllureRuntime Runtime => runtime;

    public bool IsAllureEnabled { get; set; } = true;

    public bool IsAllureAlive { get; set; } = true;
}
