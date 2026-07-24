using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Allure.Sdk.Configuration;

public class JsonFileConfigurationSource<TConfiguration>(string path) :
    IAllureConfigurationSource<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
{
    static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public string Name => $"json from {path}";

    public bool CanLoad => path is { Length: >0 };

    public TConfiguration LoadConfiguration()
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

        return configurationObject.Deserialize<TConfiguration>(serializerOptions)
            ?? new();
    }

    static JsonObject GetConfigurationObject(JsonObject root) =>
        root.TryGetPropertyValue("allure", out var allureNode)
            ? (allureNode as JsonObject
                ?? throw new InvalidOperationException(
                    "The 'allure' property must contain a JSON object"))
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

public static class JsonFileConfigurationSource
{
    public static JsonFileConfigurationSource<TConfiguration> FromEnvironmentVariable<TConfiguration>(
        string environmentVariableName
    )
        where TConfiguration : AllureConfiguration, new()
    => new(
        Environment.GetEnvironmentVariable(environmentVariableName)
    );

    public static JsonFileConfigurationSource<TConfiguration> FromEnvironmentVariable<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        FromEnvironmentVariable<TConfiguration>("ALLURE_CONFIG");

    public static JsonFileConfigurationSource<TConfiguration> FromBaseDirectory<TConfiguration>()
        where TConfiguration : AllureConfiguration, new()
    =>
        new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allureConfig.json"));
}
