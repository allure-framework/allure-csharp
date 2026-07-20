namespace Allure.Abstractions;

public interface IAllureRuntime
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureRuntimeOperations Operations { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
