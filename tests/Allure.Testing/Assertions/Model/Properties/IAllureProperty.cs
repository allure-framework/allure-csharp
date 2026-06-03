using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureProperty<TValue, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureProperty<TValue, TSelf>
{
    abstract static JsonType JsonType { get; }

    static virtual AssertionResult<TValue> GetValue(TSelf? obj, string propertyName) =>
        obj switch
        {
            { Json: var json } =>
                GetValue(json, propertyName),

            _ => AssertionResult.Failed($"the target was null"),
        };

    static AssertionResult<TValue> GetValue(JsonElement json, string propertyName) =>
        json switch
        {
            { ValueKind: JsonValueKind.Null } => AssertionResult.Failed("object was null"),

            { ValueKind: not JsonValueKind.Object } =>
                AssertionResult.Failed(
                    $"was a JSON {JsonTypeString}. Expected an object"),

            _ =>
                json.TryGetProperty(propertyName, out var propertyElement)
                    ? propertyElement.ValueKind switch
                    {
                        var propertyValueKind when JsonFunctions.TypeMatchesValue(TSelf.JsonType, propertyValueKind) =>
                            TSelf.TryConvertToPropertyValue(propertyElement) switch
                            {
                                { IsPassed: true } result => result,

                                { Message: var error } => AssertionResult.Failed($"\"{propertyName}\" {error}"),
                            },

                        JsonValueKind.Null => AssertionResult.Failed($"\"{propertyName}\" was null"),

                        var propertyValueKind => AssertionResult.Failed(
                            $"the value of \"{propertyName}\" was a JSON"
                                + JsonFunctions.GetJsonKindTypeString(propertyValueKind)),
                    }
                    : AssertionResult.Failed($"\"{propertyName}\" was missing"),
        };

    static string JsonTypeString { get; } = JsonFunctions.GetJsonTypeString<TValue, TSelf>();

    protected abstract static AssertionResult<TValue> TryConvertToPropertyValue(JsonElement json);
}
