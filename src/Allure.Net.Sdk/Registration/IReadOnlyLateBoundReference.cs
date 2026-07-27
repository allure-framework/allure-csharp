namespace Allure.Sdk.Registration;

public interface IReadOnlyLateBoundReference<out T>
{
    public bool IsBound { get; }

    public T Value { get; }
}
