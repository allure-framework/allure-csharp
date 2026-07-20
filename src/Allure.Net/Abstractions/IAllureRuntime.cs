namespace Allure.Abstractions;

public interface IAllureRuntime
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureRuntimeOperations TestApi { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
