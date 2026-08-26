using System;
using System.IO;
using System.Text.Json.Nodes;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace Allure.Build.Tasks;

public class PrepareConfigSchema : Task
{
    [Required]
    public string InputPath { get; set; } = "";

    [Required]
    public string OutputDirectory { get; set; } = "";

    [Required]
    public string Version { get; set; } = "";

    [Required]
    public string PackageName { get; set; } = "";

    [Output]
    public string VersionedSchemaPath { get; set; } = "";

    [Output]
    public string LatestSchemaPath { get; set; } = "";

    public override bool Execute()
    {
        try
        {
            var packageName = this.PackageName.ToLower();
            var version = this.Version.ToLower();

            var fileName = Path.GetFileName(this.InputPath);

            var schema = JsonNode.Parse(
                File.ReadAllBytes(this.InputPath)
            ) as JsonObject
                ?? throw new InvalidOperationException(
                    $"Schema '{this.InputPath}' must contain a JSON object."
                );

            var sourceId = GetSchemaId(schema);
            var schemaRoot = GetSchemaRoot(sourceId, packageName, fileName);

            var latestSchema = schema.DeepClone();
            var versionedSchema = schema.DeepClone();

            versionedSchema["$id"] = AddVersion(sourceId, schemaRoot, version).AbsoluteUri;

            RewriteReferences(versionedSchema, sourceId, schemaRoot, version);

            this.LatestSchemaPath = Write(
                latestSchema,
                Path.Combine(this.OutputDirectory, "schemas", packageName, fileName)
            );

            this.VersionedSchemaPath = Write(
                versionedSchema,
                Path.Combine(this.OutputDirectory, "schemas", version, packageName, fileName)
            );

            return true;
        }
        catch (Exception error)
        {
            this.Log.LogErrorFromException(error);
            return false;
        }
    }

    static Uri GetSchemaId(JsonObject schema)
    {
        var value = schema["$id"]?.GetValue<string>()
            ?? throw new InvalidOperationException(
                "The configuration schema must define a root '$id'."
            );

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                $"Schema '$id' must be an absolute URI, but was '{value}'."
            );
        }

        return uri;
    }

    static Uri GetSchemaRoot(Uri schemaId, string packageName, string fileName)
    {
        var expectedSuffix = $"/{packageName}/{fileName}";

        if (!schemaId.AbsolutePath.EndsWith(expectedSuffix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Schema '$id' must end with '{expectedSuffix}', "
                    + $"but was '{schemaId}'."
            );
        }

        return new(schemaId, "../");
    }

    static Uri AddVersion(Uri schemaId, Uri schemaRoot, string version)
    {
        var relative = schemaRoot.MakeRelativeUri(schemaId);
        var versionPrefix = $"{Uri.EscapeDataString(version)}/";

        return new(schemaRoot, versionPrefix + relative.OriginalString);
    }

    static void RewriteReferences(
        JsonNode? node,
        Uri sourceId,
        Uri schemaRoot,
        string version
    )
    {
        switch (node)
        {
            case JsonObject obj:
                if (obj["$ref"] is JsonValue referenceNode
                    && referenceNode.TryGetValue<string>(out var reference)
                    && !reference.StartsWith('#'))
                {
                    var resolved = new Uri(sourceId, reference);

                    if (schemaRoot.IsBaseOf(resolved))
                    {
                        obj["$ref"] = AddVersion(
                            resolved,
                            schemaRoot,
                            version
                        ).AbsoluteUri;
                    }
                }

                foreach (var property in obj)
                {
                    RewriteReferences(
                        property.Value,
                        sourceId,
                        schemaRoot,
                        version
                    );
                }

                break;

            case JsonArray array:

                foreach (var item in array)
                {
                    RewriteReferences(
                        item,
                        sourceId,
                        schemaRoot,
                        version
                    );
                }

                break;
        }
    }

    static string Write(JsonNode schema, string path)
    {
        var source = GeneratedFileSource.FromJsonSourceObject(schema, path);

        if (source.ShouldWrite)
        {
            source.Write();
        }

        return source.Destination.FullName;
    }
}
