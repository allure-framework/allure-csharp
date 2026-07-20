namespace Allure.Abstractions;

public interface IAllureApiClient
{
    string Name { get; }

    bool IsAvailable { get; }

    AllureApiOperations Operations { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
