namespace Allure.TestingPlatform.Sdk.Runtime;

public interface IAllureTestingPlatformRuntimeOwner
{
    public IAllureTestingPlatformRuntimeProvider RuntimeProvider { get; }

    AllureTestingPlatformRuntime Configure();

    AllureTestingPlatformRuntime Build();
}
