namespace Allure.Abstractions;

public interface IAllureTestRuntimeBackend
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureBackendTestApi TestApi { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
