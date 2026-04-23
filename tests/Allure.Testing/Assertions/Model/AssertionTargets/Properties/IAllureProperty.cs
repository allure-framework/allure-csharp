using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Allure.Testing.Internal;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureProperty<TValue, TFinal> : IAllureJsonObject
    where TFinal : IAllureProperty<TValue, TFinal>
{
    public abstract static string PropertyName { get; }

    abstract static JsonType JsonType { get; }

    abstract static TValue? GetValue(JsonElement json);

    virtual static string JsonTypeString { get; } = JsonFunctions.GetJsonTypeString<TValue, TFinal>();
}
