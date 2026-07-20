namespace Allure.Abstractions;

public interface IAllureApiClient
{
    string Name { get; }

    bool IsAllureAvailable { get; }

    AllureApiOperations TestApi { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
