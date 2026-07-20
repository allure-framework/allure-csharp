namespace Allure.Abstractions;

public interface IAllureParameterSerializer
{
    string Serialize(object? value);
}
