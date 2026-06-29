namespace Allure.TestingPlatform.Sdk.Runtime;

public interface IAllureTestingPlatformRuntimeController
{
    public IAllureTestingPlatformRuntimeReference RuntimeReference { get; }

    AllureTestingPlatformRuntimeState Configure();

    AllureTestingPlatformRuntimeState Start();
}
