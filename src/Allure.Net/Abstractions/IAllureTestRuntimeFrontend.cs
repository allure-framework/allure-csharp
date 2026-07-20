namespace Allure.Abstractions;

public interface IAllureTestRuntimeFrontend
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureFrontendTestApi TestApi { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
