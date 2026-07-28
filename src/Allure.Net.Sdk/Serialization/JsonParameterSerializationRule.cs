using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Allure.Sdk.Serialization;

public class JsonParameterSerializationRule(JsonSerializerOptions serializerOptions) :
    IParameterSerializationRule
{
    public JsonParameterSerializationRule() : this(CreateDefaultJsonOptions()) { }

    public JsonSerializerOptions SerializerOptions => serializerOptions;

    public bool TrySerialize(object value, [NotNullWhen(true)] out string? text)
    {
        text = JsonSerializer.Serialize(value, serializerOptions);
        return true;
    }

    public static JsonSerializerOptions CreateDefaultJsonOptions() => new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}
