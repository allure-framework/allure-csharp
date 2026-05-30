using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureProperty<TValue, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureProperty<TValue, TSelf>
{
    abstract static string PropertyName { get; }

    abstract static JsonType JsonType { get; }

    static virtual AssertionResult<TValue> GetValue(TSelf? obj) =>
        obj switch
        {
            { Json.ValueKind: JsonValueKind.Null } => AssertionResult.Failed("the object was null"),

            { Json.ValueKind: not JsonValueKind.Object } =>
                AssertionResult.Failed(
                    $"a JSON {JsonTypeString} can't have properties"),

            { Json: var json } =>
                json.TryGetProperty(TSelf.PropertyName, out var propertyElement)
                    ? propertyElement.ValueKind switch
                    {
                        var propertyValueKind when JsonFunctions.TypeMatchesValue(TSelf.JsonType, propertyValueKind) =>
                            TSelf.TryGetPropertyValue(propertyElement),

                        JsonValueKind.Null => AssertionResult.Failed($"\"{TSelf.PropertyName}\" was null"),

                        var propertyValueKind => AssertionResult.Failed(
                            $"the value of \"{TSelf.PropertyName}\" was a JSON"
                                + JsonFunctions.GetJsonKindTypeString(propertyValueKind)),
                    }
                    : AssertionResult.Failed($"\"{TSelf.PropertyName}\" was missing"),

            _ => AssertionResult.Failed($"the target was null"),
        };

    static string JsonTypeString { get; } = JsonFunctions.GetJsonTypeString<TValue, TSelf>();

    protected abstract static AssertionResult<TValue> TryGetPropertyValue(JsonElement json);
}
