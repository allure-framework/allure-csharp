using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Configurations;

namespace Allure.TestingPlatform.Tests;

public class ConfigurationDiscoveryTests
{
    [Test]
    public async Task ShouldReadConfigurationFromAllureObjectInExplicitFile()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Configured title",
                "directory": "configured-results",
                "links": [ "https://example.org/{issue}" ],
                "indentOutput": true
              }
            }
            """
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            new ServiceProviderStub(),
            configPath
        );

        await Assert.That(config.Title).IsEqualTo("Configured title");
        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("configured-results"));
        await Assert.That(config.Links).Contains("https://example.org/{issue}");
        await Assert.That(config.IndentOutput).IsTrue();
    }

    [Test]
    public async Task ShouldReadConfigurationFromRootObjectInExplicitFile()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "title": "Root title",
              "directory": "root-results",
              "links": [ "https://example.org/{tms}" ]
            }
            """
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            new ServiceProviderStub(),
            configPath
        );

        await Assert.That(config.Title).IsEqualTo("Root title");
        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("root-results"));
        await Assert.That(config.Links).Contains("https://example.org/{tms}");
    }

    [Test]
    public async Task ShouldReadCustomConfigurationTypeFromExplicitFile()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Custom title",
                "customValue": "Custom value"
              }
            }
            """
        );

        var config = ConfigurationFunctions.ReadConfiguration<CustomAllureConfiguration>(
            _ => null,
            new ServiceProviderStub(),
            configPath
        );

        var customConfig = await Assert.That(config).IsTypeOf<CustomAllureConfiguration>();
        await Assert.That(customConfig.CustomValue).IsEqualTo("Custom value");
        await Assert.That(customConfig.Title).IsEqualTo("Custom title");
    }

    [Test]
    public async Task ShouldReadPathFromEnvironmentVariableIfNotProvidedExplicitly()
    {
        string actualEnvVarName = null;
        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            name =>
            {
                actualEnvVarName = name;
                return null;
            },
            new ServiceProviderStub()
        );

        await Assert.That(actualEnvVarName).IsEqualTo("ALLURE_CONFIG");
    }

    [Test]
    public async Task ShouldReadConfigurationFromEnvironmentVariable()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Environment title",
                "directory": "environment-results"
              }
            }
            """
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => configPath,
            new ServiceProviderStub()
        );

        await Assert.That(config.Title).IsEqualTo("Environment title");
        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("environment-results"));
    }

    [Test]
    public async Task ShouldPreferExplicitFileOverEnvironmentVariable()
    {
        using var temp = TempDirectory.Create();
        var envConfigPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Environment title"
              }
            }
            """,
            "env-allureConfig.json"
        );
        var explicitConfigPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Explicit title"
              }
            }
            """,
            "explicit-allureConfig.json"
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => envConfigPath,
            new ServiceProviderStub(),
            explicitConfigPath
        );

        await Assert.That(config.Title).IsEqualTo("Explicit title");
    }

    [Test]
    public async Task ShouldThrowIfExplicitConfigurationFileDoesNotExist()
    {
        var missingConfigPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString(),
            "missing-allureConfig.json"
        );

        await Assert.That(() => ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            new ServiceProviderStub(),
            missingConfigPath
        )).Throws<FileNotFoundException>().WithMessageContaining(missingConfigPath);
    }

    [Test]
    public async Task ShouldThrowIfEnvVarFileDoesNotExist()
    {
        var missingConfigPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString(),
            "missing-allureConfig.json"
        );

        await Assert.That(() => ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => missingConfigPath,
            new ServiceProviderStub()
        )).Throws<FileNotFoundException>().WithMessageContaining(missingConfigPath);
    }

    [Test]
    public async Task ShouldUseMtpResultDirectoryAsDefaultAllureResultsDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Configured title"
              }
            }
            """
        );
        var mtpResults = Path.Combine(temp.Path, "mtp-results");
        var serviceProvider = new ServiceProviderStub(
            ("platformOptions:resultDirectory", mtpResults)
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Directory).IsEqualTo(
            Path.GetFullPath(Path.Combine(mtpResults, "allure-results"))
        );
    }

    [Test]
    public async Task ShouldNotOverwriteConfiguredDirectoryWithMtpResultDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "directory": "configured-results"
              }
            }
            """
        );
        var mtpResults = Path.Combine(temp.Path, "mtp-results");
        var serviceProvider = new ServiceProviderStub(
            ("platformOptions:resultDirectory", mtpResults)
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("configured-results"));
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverAllureObjectDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "directory": "configured-results"
              }
            }
            """
        );
        var serviceProvider = new ServiceProviderStub();
        serviceProvider.CommandLineOptions.ResultsDirectory = "cli-results";

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("cli-results"));
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverRootObjectDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "directory": "root-results"
            }
            """
        );
        var serviceProvider = new ServiceProviderStub();
        serviceProvider.CommandLineOptions.ResultsDirectory = "cli-results";

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("cli-results"));
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverEnvironmentConfigurationDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "directory": "environment-results"
              }
            }
            """
        );
        var serviceProvider = new ServiceProviderStub();
        serviceProvider.CommandLineOptions.ResultsDirectory = "cli-results";

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => configPath,
            serviceProvider
        );

        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("cli-results"));
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverMtpResultDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "allure": {
                "title": "Configured title"
              }
            }
            """
        );
        var mtpResults = Path.Combine(temp.Path, "mtp-results");
        var serviceProvider = new ServiceProviderStub(
            ("platformOptions:resultDirectory", mtpResults)
        );
        serviceProvider.CommandLineOptions.ResultsDirectory = "cli-results";

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Directory).IsEqualTo(Path.GetFullPath("cli-results"));
    }

    [Test]
    public async Task ShouldPutDefaultDirectoryInRootObjectIfAllureConfigDefinedAtRootLevel()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            """
            {
              "title": "Configured title"
            }
            """
        );
        var mtpResults = Path.Combine(temp.Path, "mtp-results");
        var serviceProvider = new ServiceProviderStub(
            ("platformOptions:resultDirectory", mtpResults)
        );

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider,
            configPath
        );

        await Assert.That(config.Title).IsEqualTo("Configured title");
        await Assert.That(config.Directory).IsEqualTo(
            Path.GetFullPath(Path.Combine(mtpResults, "allure-results"))
        );
    }

    [Test]
    public async Task ShouldFallbackToAllureResultsInCurrentDirectoryIfNoMtpResultsDirConfigured()
    {
        var serviceProvider = new ServiceProviderStub();

        var config = ConfigurationFunctions.ReadConfiguration<AllureConfiguration>(
            _ => null,
            serviceProvider
        );

        await Assert.That(config.Directory).IsEqualTo(
            Path.Combine(Environment.CurrentDirectory, "allure-results")
        );
    }

    sealed class ServiceProviderStub(params (string Key, string Value)[] configuration) : IServiceProvider
    {
        readonly ConfigurationStub configuration = new(configuration);

        public CommandLineOptionsStub CommandLineOptions { get; } = new();

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IConfiguration))
            {
                return this.configuration;
            }
            else if (serviceType == typeof(ICommandLineOptions))
            {
                return this.CommandLineOptions;
            }

            throw new NotImplementedException();
        }
    }

    sealed class CustomAllureConfiguration : AllureConfiguration
    {
        public string CustomValue { get; set; }
    }

    sealed class ConfigurationStub(params (string Key, string Value)[] values) : IConfiguration
    {
        readonly Dictionary<string, string> values = values.ToDictionary(
            value => value.Key,
            value => value.Value
        );

        public string this[string key]
        {
            get => this.values.TryGetValue(key, out var value) ? value : null;
            set => this.values[key] = value;
        }
    }

    sealed class TempDirectory : IDisposable
    {
        public string Path { get; }

        TempDirectory(string path)
        {
            this.Path = path;
        }

        public static TempDirectory Create()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"allure-testing-platform-{Guid.NewGuid():N}"
            );
            Directory.CreateDirectory(path);
            return new(path);
        }

        public string WriteConfig(string json, string fileName = "allureConfig.json")
        {
            var path = System.IO.Path.Combine(this.Path, fileName);
            File.WriteAllText(path, json);
            return path;
        }

        public void Dispose()
        {
            if (Directory.Exists(this.Path))
            {
                Directory.Delete(this.Path, true);
            }
        }
    }
}
