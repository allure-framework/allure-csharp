namespace Allure.Sdk.Registration;

public interface ILateBoundReferenceView<out T>
{
    public bool IsBound { get; }

    public T Value { get; }
}
