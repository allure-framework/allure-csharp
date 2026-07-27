using System;

namespace Allure.Sdk.Serialization;

public class DelegateParameterSerializationRule<T>(Func<T, string> serialize) :
    TypedParameterSerializationRule<T>
{
    protected override string Serialize(T value) => serialize(value);
}

public static class DelegateParameterSerializationRule
{
    public static DelegateParameterSerializationRule<T> Create<T>(Func<T, string> serialize) =>
        new(serialize);
}
