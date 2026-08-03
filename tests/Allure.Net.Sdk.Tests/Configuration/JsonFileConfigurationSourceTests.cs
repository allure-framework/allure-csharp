using System.Collections.Immutable;
using System.Text.Json;
using Allure.Sdk.Configuration;

namespace Allure.Net.Sdk.Tests.Configuration;

public class JsonFileConfigurationSourceTests
{
    [Test]
    public async Task ShouldLoadAllModernConfigurationProperties()
    {
        var path = await WriteConfiguration("""
            {
              "hostname": "build-agent",
              "resultsDirectory": "test-results",
              "linkTemplates": {
                "issue": {
                  "urlTemplate": "https://tracker/{0}",
                  "nameTemplate": "Issue {0}"
                }
              },
              "failExceptions": ["FatalException"],
              "indentOutput": true,
              "globalLabels": { "browser": "Chrome" },
              "runtimeRegistrationHook": "Hooks.Configure"
            }
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo("build-agent");
            await Assert.That(configuration.ResultsDirectory)
                .IsEqualTo(Path.GetFullPath("test-results"));
            await Assert.That(configuration.LinkTemplates["issue"])
                .IsEqualTo(new AllureLinkTemplate("https://tracker/{0}", "Issue {0}"));
            await Assert.That(configuration.FailExceptions)
                .IsEquivalentTo(ImmutableList.Create("FatalException"));
            await Assert.That(configuration.IndentOutput).IsTrue();
            await Assert.That(configuration.GlobalLabels["browser"]).IsEqualTo("Chrome");
            await Assert.That(configuration.RuntimeRegistrationHook).IsEqualTo("Hooks.Configure");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldLoadConfigurationFromAllureSection()
    {
        var path = await WriteConfiguration("""{"allure":{"hostname":"nested"}}""");

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo("nested");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldLoadDerivedConfiguration()
    {
        var path = await WriteConfiguration("""{"hostname":"agent","value":"custom"}""");

        try
        {
            var configuration = new JsonFileConfigurationSource<TestConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo("agent");
            await Assert.That(configuration.Value).IsEqualTo("custom");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldUseDefaultsForEmptyConfiguration()
    {
        var path = await WriteConfiguration("{}");

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo(Environment.MachineName);
            await Assert.That(configuration.ResultsDirectory)
                .IsEqualTo(Path.GetFullPath("allure-results"));
            await Assert.That(configuration.LinkTemplates).IsEmpty();
            await Assert.That(configuration.FailExceptions).IsEmpty();
            await Assert.That(configuration.IndentOutput).IsFalse();
            await Assert.That(configuration.GlobalLabels).IsEmpty();
            await Assert.That(configuration.RuntimeRegistrationHook).IsNull();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldMapLegacyProperties()
    {
        var path = await WriteConfiguration("""
            { "allure": {
              "title": "legacy-agent",
              "directory": "legacy-results",
              "links": [
                "https://tracker/{issue}",
                "https://tms/{tms}",
                "https://duplicate/{issue}"
              ]
            }}
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo("legacy-agent");
            await Assert.That(configuration.ResultsDirectory)
                .IsEqualTo(Path.GetFullPath("legacy-results"));
            await Assert.That(configuration.LinkTemplates)
                .IsEquivalentTo(
                    ImmutableDictionary<string, AllureLinkTemplate>.Empty
                        .Add("issue", new("https://duplicate/{0}", null))
                        .Add("tms", new("https://tms/{0}", null))
                );
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldIgnoreLegacyLinksWithoutAValidPlaceholder()
    {
        var path = await WriteConfiguration("""
            { "links": ["https://tracker/no-placeholder", "https://tracker/{}"] }
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.LinkTemplates).IsEmpty();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldSupportEscapingInLegacyLinks()
    {
        var path = await WriteConfiguration("""
            { "links": ["https://tracker/{{foo}}/{issue}"] }
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.LinkTemplates)
                .IsEquivalentTo(
                    ImmutableDictionary<string, AllureLinkTemplate>.Empty
                        .Add("issue", new("https://tracker/{{foo}}/{0}", null))
                );
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldPreserveModernPropertiesWhenLegacyPropertiesAreAlsoPresent()
    {
        var path = await WriteConfiguration("""
            {
              "hostname": "modern-agent", "title": "legacy-agent",
              "resultsDirectory": "modern-results", "directory": "legacy-results",
              "linkTemplates": { "issue": { "urlTemplate": "https://modern/{0}" } },
              "links": ["https://legacy/{issue}"]
            }
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsEqualTo("modern-agent");
            await Assert.That(configuration.ResultsDirectory)
                .IsEqualTo(Path.GetFullPath("modern-results"));
            await Assert.That(configuration.LinkTemplates["issue"].UrlTemplate)
                .IsEqualTo("https://modern/{0}");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldTreatConfigurationPropertyNamesAsCaseSensitive()
    {
        var path = await WriteConfiguration("""
            { "Hostname": "wrong", "ResultsDirectory": "wrong-results", "IndentOutput": true }
            """);

        try
        {
            var configuration = new JsonFileConfigurationSource<AllureConfiguration>(path)
                .LoadConfiguration();

            await Assert.That(configuration.Hostname).IsNotEqualTo("wrong");
            await Assert.That(configuration.ResultsDirectory)
                .IsNotEqualTo(Path.GetFullPath("wrong-results"));
            await Assert.That(configuration.IndentOutput).IsFalse();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldThrowForInvalidLegacyLinkItems()
    {
        var path = await WriteConfiguration("""{"links":[42]}""");

        try
        {
            var source = new JsonFileConfigurationSource<AllureConfiguration>(path);

            await Assert.That(source.LoadConfiguration).Throws<JsonException>();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    [Arguments("[]")]
    [Arguments("null")]
    [Arguments("\"not an object\"")]
    public async Task ShouldThrowWhenConfigurationRootIsNotAnObject(string json)
    {
        var path = await WriteConfiguration(json);

        try
        {
            var source = new JsonFileConfigurationSource<AllureConfiguration>(path);

            await Assert.That(source.LoadConfiguration).Throws<JsonException>();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldThrowWhenAllureSectionIsNotAnObject()
    {
        var path = await WriteConfiguration("""{"allure":false}""");

        try
        {
            var source = new JsonFileConfigurationSource<AllureConfiguration>(path);

            await Assert.That(source.LoadConfiguration).Throws<InvalidOperationException>();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldNotLoadAnEmptyEnvironmentPath()
    {
        var variableName = $"ALLURE_SDK_TEST_CONFIG_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(variableName, null);

        var source = JsonFileConfigurationSource
            .FromPathEnvironmentVariable<AllureConfiguration>(variableName);

        await Assert.That(source.CanLoad).IsFalse();
    }

    [Test]
    public async Task ShouldThrowWhenConfigurationFileDoesNotExist()
    {
        var path = Path.Combine(Path.GetTempPath(), $"missing-allure-config-{Guid.NewGuid():N}.json");
        var source = new JsonFileConfigurationSource<AllureConfiguration>(path);

        await Assert.That(source.LoadConfiguration).Throws<FileNotFoundException>();
    }

    static async Task<string> WriteConfiguration(string contents)
    {
        var path = Path.Combine(Path.GetTempPath(), $"allure-sdk-config-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(path, contents);
        return path;
    }

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string? Value { get; init; }
    }
}
