using System;

namespace Allure.Sdk.Serialization;

public class LambdaParameterSerializationRule<T>(Func<T, string> serialize) :
    TypedParameterSerializationRule<T>
{
    protected override string Serialize(T value) => serialize(value);
}

public static class LambdaParameterSerializationRule
{
    public static LambdaParameterSerializationRule<T> Create<T>(Func<T, string> serialize) =>
        new(serialize);
}
