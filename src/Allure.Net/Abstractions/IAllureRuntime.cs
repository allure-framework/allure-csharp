namespace Allure.Abstractions;

public interface IAllureRuntime
{
    string Name { get; }

    bool IsAvailable { get; }

    AllureRuntimeOperations Operations { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
