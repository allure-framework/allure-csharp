using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Allure.Sdk.Serialization;

/// <summary>
/// Serializes parameter values to JSON.
/// </summary>
/// <param name="serializerOptions">The JSON serializer options.</param>
public class JsonParameterSerializationRule(JsonSerializerOptions serializerOptions) :
    IParameterSerializationRule
{
    /// <summary>
    /// Initializes a rule with the default JSON serializer options.
    /// </summary>
    public JsonParameterSerializationRule() : this(CreateDefaultJsonOptions()) { }

    /// <summary>
    /// Gets the JSON serializer options.
    /// </summary>
    public JsonSerializerOptions SerializerOptions => serializerOptions;

    /// <inheritdoc/>
    public bool TrySerialize(object value, [NotNullWhen(true)] out string? text)
    {
        try
        {
            text = JsonSerializer.Serialize(value, serializerOptions);
            return true;
        }
        catch (Exception)
        {
        }

        text = null;
        return false;
    }

    /// <summary>
    /// Creates the default JSON serializer options used for Allure parameters.
    /// </summary>
    public static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        JsonSerializerOptions options = new ()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { IgnoreDelegatePropertiesModifier },
            },
        };

        options.Converters.Add(new ObjectToStringConverterFactory(options));
        return options;
    }

    static void IgnoreDelegatePropertiesModifier(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
        {
            var property = typeInfo.Properties[i];

            if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
            {
                typeInfo.Properties.RemoveAt(i);
            }
        }
    }

    class ObjectToStringConverterFactory(JsonSerializerOptions contractOptions) : JsonConverterFactory
    {
        readonly JsonSerializerOptions contractOptions = new(contractOptions);

        public override bool CanConvert(Type typeToConvert)
        {
            var typeInfo = this.contractOptions.GetTypeInfo(typeToConvert);
            if (typeInfo.Kind != JsonTypeInfoKind.Object)
            {
                return false;
            }

            var toString = typeToConvert.GetMethod(
                nameof(ToString),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null
            );

            return toString?.DeclaringType != typeof(object);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type converterType =
                typeof(ObjectToStringConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }

    private sealed class ObjectToStringConverter<T> : JsonConverter<T>
    {
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }

        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotSupportedException("This converter does not support deserialization.");
        }
    }
}
