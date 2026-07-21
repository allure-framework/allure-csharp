namespace Allure.Abstractions;

public interface IAllureApiEndpoint
{
    AllureApiOperations Operations { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}
