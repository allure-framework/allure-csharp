using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace Allure.Sdk.Configuration;

/// <summary>
/// Loads Allure configuration from a JSON file.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
/// <param name="path">The path to the JSON configuration file.</param>
/// <param name="isOptional">
/// Whether the source should be skipped when the file does not exist.
/// </param>
/// <param name="serializerOptions">Serializer options to use for deserialization.</param>
public class JsonFileConfigurationSource<TConfiguration>(
    string path,
    bool isOptional,
    JsonSerializerOptions serializerOptions
) :
    IAllureConfigurationSource<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
{
    static readonly JsonSerializerOptions defaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
    };

    /// <inheritdoc/>
    public string Name => $"JSON from {path}";

    /// <inheritdoc/>
    public bool CanLoad => path is { Length: >0 }
        && (!isOptional || File.Exists(Path.GetFullPath(path)));

    /// <summary>
    /// Creates a mandatory configuration source that throws if the file does not exist.
    /// </summary>
    /// <param name="path">The path to the JSON configuration file.</param>
    public JsonFileConfigurationSource(string path) :
        this(path, false, defaultSerializerOptions) { }

    /// <summary>
    /// Creates a configuration source for the specified path.
    /// </summary>
    /// <param name="path">The path to the JSON configuration file.</param>
    /// <param name="isOptional">
    /// Whether the source should be skipped when the file does not exist.
    /// </param>
    public JsonFileConfigurationSource(string path, bool isOptional) :
        this(path, isOptional, defaultSerializerOptions) { }

    /// <inheritdoc/>
    /// <exception cref="FileNotFoundException">
    /// The configured file does not exist.
    /// </exception>
    /// <exception cref="JsonException">
    /// The file does not contain a supported JSON configuration object.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The top-level <c>allure</c> property is present but is not a JSON object.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The custom converter for <typeparamref name="TConfiguration"/> does not expose
    /// property-assignment metadata.
    /// </exception>
    public TrackedConfiguration<TConfiguration> LoadConfiguration()
    {
        var configFullPath = Path.GetFullPath(path);
        if (!File.Exists(configFullPath))
        {
            throw new FileNotFoundException(
                $"The configuration file '{configFullPath}' does not exist."
            );
        }

        using var stream = File.OpenRead(configFullPath);
        var root = JsonNode.Parse(stream) as JsonObject
            ?? throw new JsonException(
                "The Allure configuration file must contain a JSON object."
            );

        var configurationObject = GetConfigurationObject(root);
        NormalizeLegacyProperties(configurationObject);

        TConfiguration configuration =
            configurationObject.Deserialize<TConfiguration>(serializerOptions)
                ?? new();

        return new(this.Name, configuration, this.GetAssignedProperties(configurationObject));
    }

    static JsonObject GetConfigurationObject(JsonObject root) =>
        root.TryGetPropertyValue("allure", out var allureNode)
            ? (allureNode as JsonObject
                ?? throw new InvalidOperationException(
                    "The 'allure' property must contain a JSON object."))
            : root;

    void NormalizeLegacyProperties(JsonObject configuration)
    {
        if (!configuration.ContainsKey("hostname")
            && configuration.TryGetPropertyValue("title", out var titleNode)
            && titleNode?.GetValueKind() is JsonValueKind.String)
        {
            configuration["hostname"] = titleNode.DeepClone();
        }

        if (!configuration.ContainsKey("resultsDirectory"))
        {
            if (configuration.TryGetPropertyValue("directory", out var directoryNode)
                && directoryNode?.GetValueKind() is JsonValueKind.String)
            {
                configuration["resultsDirectory"] = directoryNode.DeepClone();
            }
        }

        if (!configuration.ContainsKey("linkTemplates")
            && configuration.TryGetPropertyValue("links", out var linksNode)
            && linksNode?.GetValueKind() is JsonValueKind.Array)
        {
            configuration["linkTemplates"] = ConvertLegacyLinks(linksNode.AsArray());
        }

        configuration.Remove("title");
        configuration.Remove("directory");
        configuration.Remove("links");
    }

    IEnumerable<string> GetAssignedProperties(JsonObject configurationObject)
    {
        var typeInfo = serializerOptions.GetTypeInfo(typeof(TConfiguration));

        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            return GetObjectContractAssignments(typeInfo, configurationObject);
        }

        var converter = serializerOptions.GetConverter(
            typeof(TConfiguration)
        );

        if (converter is IJsonConfigurationAssignmentTracker<TConfiguration> assignmentTracker)
        {
            return assignmentTracker.GetAssignedPropertyNames(
                configurationObject,
                serializerOptions
            );
        }

        throw new NotSupportedException(
            $"The JSON converter for configuration type "
                + $"{typeof(TConfiguration).Name} does not provide "
                + "property-assignment metadata."
        );
    }

    static IEnumerable<string> GetObjectContractAssignments(
        JsonTypeInfo typeInfo,
        JsonObject configurationObject
    ) =>
        typeInfo.Properties
            .Where((jsonProperty) =>
                configurationObject.ContainsKey(jsonProperty.Name)
                && (
                    jsonProperty.Set is not null
                    || jsonProperty.AssociatedParameter is not null
                )
            )
            .Select(static jsonProperty => jsonProperty.AttributeProvider)
            .OfType<PropertyInfo>()
            .Select(static property => property.Name);

    JsonNode? ConvertLegacyLinks(JsonArray jsonArray)
    {
        var result = new JsonObject();

        foreach (var item in jsonArray)
        {
            if (item is not JsonValue value
                || !value.TryGetValue<string>(out var link))
            {
                throw new JsonException(
                    "Every item in the legacy 'links' array must be a string."
                );
            }

            if (TryParseLegacyLink(link, out var type, out var urlTemplate))
            {
                result[type] = new JsonObject
                {
                    ["urlTemplate"] = urlTemplate,
                };
            }
        }

        return result;
    }

    bool TryParseLegacyLink(string link, out string type, out string urlTemplate)
    {
        for (var open = link.IndexOf('{'); open >= 0; open = link.IndexOf('{', open + 1))
        {
            if (open + 1 >= link.Length)
            {
                break;
            }

            if (link[open + 1] == '{')
            {
                open++;
                continue;
            }

            var close = link.IndexOf('}', open + 1);
            if (close < 0 || close == open + 1)
            {
                continue;
            }

            type = link.Substring(open + 1, close - open - 1);
            urlTemplate = $"{link.Substring(0, open)}{{0}}{link.Substring(close + 1)}";
            return true;
        }

        type = "";
        urlTemplate = "";
        return false;
    }
}

/// <summary>
/// Creates JSON-file configuration sources using conventional paths.
/// </summary>
public static class JsonFileConfigurationSource
{
    /// <summary>
    /// Creates a source whose path is read from the specified environment variable.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <param name="environmentVariableName">
    /// The name of the environment variable containing the file path.
    /// </param>
    /// <returns>The configuration source.</returns>
    public static JsonFileConfigurationSource<TConfiguration> FromPathEnvironmentVariable<TConfiguration>(
        string environmentVariableName
    )
        where TConfiguration : AllureConfiguration, new()
    => new(
        Environment.GetEnvironmentVariable(environmentVariableName)
    );

    /// <summary>
    /// Creates a source whose path is read from the <c>ALLURE_CONFIG</c>
    /// environment variable.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <returns>The configuration source.</returns>
    public static JsonFileConfigurationSource<TConfiguration> FromPathEnvironmentVariable<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        FromPathEnvironmentVariable<TConfiguration>("ALLURE_CONFIG");

    /// <summary>
    /// Creates a source for <c>allureConfig.json</c> in the application base directory.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <param name="isOptional">
    /// Whether the source should be skipped when the file does not exist.
    /// </param>
    /// <returns>The configuration source.</returns>
    public static JsonFileConfigurationSource<TConfiguration> FromBaseDirectory<TConfiguration>(bool isOptional)
        where TConfiguration : AllureConfiguration, new()
    =>
        new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allureConfig.json"), isOptional);
}
