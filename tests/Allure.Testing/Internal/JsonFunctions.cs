using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;

namespace Allure.Testing.Internal;

public abstract class JsonFunctions
{
    public static V AssertedGetPropertyValue<V, P>(P? item, string propertyName)
        where P : IAllureProperty<V, P>
            =>
                item is { Json: var json }
                    ? json.TryGetProperty(propertyName, out var propertyValue)
                        ? propertyValue.ValueKind == JsonValueKind.Null
                            ? throw new InvalidOperationException(
                                $"the value of \"{propertyName}\" was null"
                            )
                            : propertyValue.ValueKind switch
                            {
                                _ when TypeMatchesValue(P.JsonType, propertyValue.ValueKind) =>
                                    TryConvertJsonProperty<V, P>(propertyValue, out var value)
                                        ? value
                                        : throw new InvalidOperationException(
                                            $"the value of \"{propertyName}\" had an invalid format"
                                        ),

                                JsonValueKind.Null => throw new InvalidOperationException(
                                    $"the value of \"{propertyName}\" was null"
                                ),

                                _ => throw new InvalidOperationException(
                                    $"the value of \"{propertyName}\" was not a JSON {GetJsonTypeString<V, P>()}"
                                ),

                            }
                        : throw new InvalidOperationException(
                            $"the object didn't have \"{propertyName}\""
                        )
                    : throw new InvalidOperationException("the object was null");

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
        where P : IAllureProperty<V, P>
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

    public static bool TryConvertJsonProperty<V, P>(JsonElement json, [NotNullWhen(true)] out V? value)
        where P : IAllureProperty<V, P>
    {
        var rawValue = P.GetValue(json);
        if (rawValue is not null)
        {
            value = rawValue;
            return true;
        }

        value = default;
        return false;
    }

    public static string? GetStringProperty(JsonElement obj, string propertyName) =>
        obj.TryGetProperty(propertyName, out var propertyValue) && propertyValue.ValueKind is JsonValueKind.String
            ? propertyValue.GetString()
            : null;
}