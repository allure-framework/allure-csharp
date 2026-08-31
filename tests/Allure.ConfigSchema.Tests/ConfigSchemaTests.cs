using System.Text.Json;
using Json.Schema;

namespace Allure.ConfigSchema.Tests;

public class ConfigSchemaTests
{
    const string Sdk = "allure.net.sdk";
    const string TestingPlatform = "allure.testingplatform";
    const string XunitV3 = "allure.xunit.v3";

    static readonly string[] Packages = [Sdk, TestingPlatform, XunitV3];

    static readonly Lazy<Dictionary<string, JsonSchema>> Schemas = new(
        LoadSchemas
    );

    [Test]
    [Arguments(Sdk)]
    [Arguments(TestingPlatform)]
    [Arguments(XunitV3)]
    public async Task ShouldConformToDraft7MetaSchema(string packageName)
    {
        using var schema = JsonDocument.Parse(
            await File.ReadAllTextAsync(GetSchemaPath(packageName))
        );
        var result = MetaSchemas.Draft7.Evaluate(schema.RootElement);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    [Arguments(Sdk, "{}")]
    [Arguments(Sdk, """
        {
          "$schema": "https://example.test/allure.config.schema.json",
          "hostname": "test-host",
          "resultsDirectory": "allure-results",
          "linkTemplates": {
            "issue": {
              "urlTemplate": "https://issues.example.test/{0}",
              "nameTemplate": "Issue {0}"
            }
          },
          "failExceptions": ["Example.AssertionException"],
          "indentOutput": true,
          "globalLabels": {
            "owner": "allure"
          },
          "runtimeRegistrationHook": null
        }
        """)]
    [Arguments(TestingPlatform, """
        {
          "resultsDirectory": "allure-results",
          "isEnabled": true,
          "isProcessWatchdogEnabled": false
        }
        """)]
    [Arguments(XunitV3, """
        {
          "hostname": "test-host",
          "indentOutput": true,
          "isEnabled": true,
          "isProcessWatchdogEnabled": false
        }
        """)]
    [Arguments(XunitV3, """
        {
          "customFrameworkOption": "allowed"
        }
        """)]
    public async Task ShouldAcceptValidConfiguration(string packageName, string configuration)
    {
        var result = Evaluate(packageName, configuration);

        await Assert.That(result.IsValid).IsTrue();
    }

    [Test]
    [Arguments(Sdk, """
        {
          "indentOutput": "true"
        }
        """)]
    [Arguments(Sdk, """
        {
          "linkTemplates": {
            "issue": {}
          }
        }
        """)]
    [Arguments(Sdk, """
        {
          "linkTemplates": {
            "issue": {
              "urlTemplate": "https://issues.example.test/{0}",
              "unknown": true
            }
          }
        }
        """)]
    [Arguments(TestingPlatform, """
        {
          "isEnabled": "true"
        }
        """)]
    [Arguments(XunitV3, """
        {
          "resultsDirectory": ""
        }
        """)]
    [Arguments(XunitV3, """
        {
          "isProcessWatchdogEnabled": 1
        }
        """)]
    public async Task ShouldRejectInvalidConfiguration(string packageName, string configuration)
    {
        var result = Evaluate(packageName, configuration);

        await Assert.That(result.IsValid).IsFalse();
    }

    static EvaluationResults Evaluate(string packageName, string configuration)
    {
        using var instance = JsonDocument.Parse(configuration);

        return Schemas.Value[packageName].Evaluate(instance.RootElement);
    }

    static Dictionary<string, JsonSchema> LoadSchemas() => Packages.ToDictionary(
        packageName => packageName,
        packageName => JsonSchema.FromFile(
            GetSchemaPath(packageName)
        )
    );

    static string GetSchemaPath(string packageName) => Path.Combine(
        AppContext.BaseDirectory,
        "schemas",
        packageName,
        "allure.config.schema.json"
    );
}
