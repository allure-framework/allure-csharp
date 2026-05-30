using System;
using System.Text.Json;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using TUnit.Assertions.Core;

namespace Allure.Testing.Internal;

public abstract class JsonFunctions
{
    public static bool TypeMatchesValue(JsonType jsonType, JsonValueKind jsonValueKind) =>
        jsonType switch
        {
            JsonType.Null => jsonValueKind is JsonValueKind.Null,
            JsonType.Boolean => jsonValueKind is JsonValueKind.True or JsonValueKind.False,
            JsonType.Number => jsonValueKind is JsonValueKind.Number,
            JsonType.String => jsonValueKind is JsonValueKind.String,
            JsonType.Array => jsonValueKind is JsonValueKind.Array,
            JsonType.Object => jsonValueKind is JsonValueKind.Object,
            _ => throw new NotImplementedException(),
        };

    public static string GetJsonTypeString<V, P>()
        where P : IAllureModelObject<P>, IAllureProperty<V, P>
    =>
        P.JsonType switch
        {
            JsonType.Null => "null",
            JsonType.Boolean => "boolean",
            JsonType.Number => "number",
            JsonType.String => "string",
            JsonType.Array => "array",
            JsonType.Object => "object",
            _ => throw new NotImplementedException(),
        };

    public static string GetJsonKindTypeString(JsonValueKind kind) =>
        kind switch
        {
            JsonValueKind.Undefined => "undefined",
            JsonValueKind.Null => "null",
            JsonValueKind.False => "boolean",
            JsonValueKind.True => "boolean",
            JsonValueKind.Number => "number",
            JsonValueKind.String => "string",
            JsonValueKind.Array => "array",
            JsonValueKind.Object => "object",
            _ => throw new NotImplementedException(),
        };

    public static AssertionResult<string> GetStringProperty(JsonElement obj, string propertyName) =>
        obj.TryGetProperty(propertyName, out var propertyValue)
            ? propertyValue.ValueKind is JsonValueKind.String
                ? AssertionResult<string>.Passed(propertyValue.GetString()!)
                : AssertionResult.Failed(
                    $"{propertyName} was {GetJsonKindTypeString(propertyValue.ValueKind)} instead of string")
            : AssertionResult.Failed($"{propertyName} did not exist");
}