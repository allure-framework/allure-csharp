using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Configurations;

#pragma warning disable TPEXP // IConfigurationSource is the only direct MTP configuration injection API.

namespace Allure.TestingPlatform.Tests;

public class ConfigurationDiscoveryTests
{
    static readonly string[] DefaultArgs =
    [
        "--no-progress",
        "--no-ansi",
        "--output",
        "Normal",
        "--show-stdout",
        "None",
        "--show-stderr",
        "None",
    ];

    [Test]
    public async Task ShouldUseMtpResultDirectoryAsDefaultAllureResultsDirectory()
    {
        using var temp = TempDirectory.Create();
        var mtpResults = Path.Combine(temp.Path, "mtp-results");

        var config = await ResolveConfiguration([], mtpResultsDirectory: mtpResults);

        await Assert.That(config.ResultsDirectory).IsEqualTo(
            Path.GetFullPath(Path.Combine(mtpResults, "allure-results"))
        );
    }

    [Test]
    public async Task ShouldNotOverwriteConfiguredDirectoryWithMtpResultDirectory()
    {
        using var temp = TempDirectory.Create();
        var configuredResults = Path.Combine(temp.Path, "configured-results");
        var configPath = temp.WriteConfig(
            $$"""
            {
              "resultsDirectory": "{{configuredResults}}"
            }
            """
        );

        var config = await ResolveConfiguration(
            [],
            configPath,
            Path.Combine(temp.Path, "mtp-results")
        );

        await Assert.That(config.ResultsDirectory).IsEqualTo(
            Path.GetFullPath(configuredResults)
        );
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverConfiguredDirectory()
    {
        using var temp = TempDirectory.Create();
        var configPath = temp.WriteConfig(
            $$"""
            {
              "resultsDirectory": "{{Path.Combine(temp.Path, "configured-results")}}"
            }
            """
        );
        var cliResults = Path.Combine(temp.Path, "cli-results");

        var config = await ResolveConfiguration(
            ["--allure-results-directory", cliResults],
            configPath
        );

        await Assert.That(config.ResultsDirectory).IsEqualTo(Path.GetFullPath(cliResults));
    }

    [Test]
    public async Task ShouldPreferCliResultsDirectoryOverMtpResultDirectory()
    {
        using var temp = TempDirectory.Create();
        var cliResults = Path.Combine(temp.Path, "cli-results");

        var config = await ResolveConfiguration(
            [
                "--allure-results-directory",
                cliResults,
            ],
            mtpResultsDirectory: Path.Combine(temp.Path, "mtp-results")
        );

        await Assert.That(config.ResultsDirectory).IsEqualTo(Path.GetFullPath(cliResults));
    }

    static async Task<AllureTestingPlatformConfiguration> ResolveConfiguration(
        string[] args,
        string configPath = null,
        string mtpResultsDirectory = null
    )
    {
        var builder = await TestApplication.CreateBuilderAsync([.. DefaultArgs, .. args]);
        if (mtpResultsDirectory is not null)
        {
            builder.Configuration.AddConfigurationSource(
                () => new ConfigurationSourceStub(
                    "platformOptions:resultDirectory",
                    mtpResultsDirectory
                )
            );
        }
        var runtimeReference = builder.AddEmbeddedAllure(
            "configuration-test",
            (context, _) =>
            {
                context.DisableHostProcessWatchdog();
                context.UseDestination(_ => new InMemoryResultsDestination());
                if (configPath is not null)
                {
                    context.UseConfigurationFile(configPath);
                }
            }
        );
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new TestFrameworkStub()
        );

        using var app = await builder.BuildAsync();
        await app.RunAsync();
        return runtimeReference.Value.Configuration;
    }

    sealed class ConfigurationSourceStub(string key, string value) : IConfigurationSource
    {
        public string Uid => "configuration-source-stub";

        public string Version => "1.0.0";

        public string DisplayName => "Configuration source stub";

        public string Description => "Provides Microsoft Testing Platform configuration for tests.";

        public int Order => 0;

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        public Task<IConfigurationProvider> BuildAsync(
            CommandLineParseResult commandLineParseResult
        ) =>
            Task.FromResult<IConfigurationProvider>(new ConfigurationProviderStub(key, value));
    }

    sealed class ConfigurationProviderStub(string key, string value) : IConfigurationProvider
    {
        public Task LoadAsync() => Task.CompletedTask;

        public bool TryGet(string requestedKey, out string result)
        {
            if (requestedKey == key)
            {
                result = value;
                return true;
            }

            result = null;
            return false;
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

        public string WriteConfig(string json)
        {
            var path = System.IO.Path.Combine(this.Path, "allureConfig.json");
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

#pragma warning restore TPEXP
