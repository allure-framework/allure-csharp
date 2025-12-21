namespace Allure.Net.Commons;

public interface ITypeFormatter
{
    string Format(object value);
}

public abstract class TypeFormatter<T> : ITypeFormatter
{
    public abstract string Format(T value);

    string ITypeFormatter.Format(object value)
    {
        return Format((T)value);
    }
}