namespace Allure.TestingPlatform.Sdk;

public interface IAllureRuntimeProvider
{
    bool IsAllureEnabled { get ;}

    bool IsAllureAlive { get ;}

    IAllureRuntime Runtime { get; }
}