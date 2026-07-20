namespace Allure.Abstractions;

public interface IAllureApiClient
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureApiOperations Operations { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
