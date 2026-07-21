namespace Allure.Abstractions;

public interface IAllureOperationContext
{
    IAllureParameterSerializer ParameterSerializer { get; }
}
