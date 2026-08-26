using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;

namespace Allure.Net.Sdk.Tests.Registration;

public class ConfigurationResolutionTests
{
    [Test]
    public async Task ShouldUseFirstLoadableConfigurationSource()
    {
        var skipped = new RecordingConfigurationSource(canLoad: false);
        var expected = new TestConfiguration { Value = "first" };
        var first = new RecordingConfigurationSource(expected);
        var later = new RecordingConfigurationSource(
            new TestConfiguration { Value = "later" }
        );
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfigurationSources(() => [skipped, first, later]);
        });

        using var runtime = plan.Build();

        await Assert.That(runtime.Runtime.Configuration).IsSameReferenceAs(expected);
        await Assert.That(skipped.LoadCount).IsEqualTo(0);
        await Assert.That(first.LoadCount).IsEqualTo(1);
        await Assert.That(later.LoadCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldCreateDefaultConfigurationWhenNoSourceCanLoad()
    {
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfigurationSources(
                () => [new RecordingConfigurationSource(canLoad: false)]
            );
        });

        using var runtime = plan.Build();

        await Assert.That(runtime.Runtime.Configuration).IsNotNull();
        await Assert.That(runtime.Runtime.Configuration.Value).IsNull();
    }

    [Test]
    public async Task ShouldEvaluateConfigurationSourceFactoryAtPrepareTime()
    {
        var factoryCalls = 0;
        var expected = new TestConfiguration();
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfigurationSources(() =>
            {
                factoryCalls++;
                return [new RecordingConfigurationSource(expected)];
            });
        });

        await Assert.That(factoryCalls).IsEqualTo(1);
        using var runtime = plan.Build();

        await Assert.That(factoryCalls).IsEqualTo(1);
        await Assert.That(runtime.Runtime.Configuration).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task ShouldUseExplicitConfigurationExtension()
    {
        var expected = new TestConfiguration();
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(expected);
        });

        using var runtime = plan.Build();

        await Assert.That(runtime.Runtime.Configuration).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task ShouldUseConfigurationSourceExtension()
    {
        var expected = new TestConfiguration();
        var sourceFactoryCalls = 0;
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfigurationSource(() =>
            {
                sourceFactoryCalls++;
                return new RecordingConfigurationSource(expected);
            });
        });

        using var runtime = plan.Build();

        await Assert.That(sourceFactoryCalls).IsEqualTo(1);
        await Assert.That(runtime.Runtime.Configuration).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task ShouldUseConfigurationFileExtension()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-config-{Guid.NewGuid():N}.json"
        );
        await File.WriteAllTextAsync(path, """{"value":"from-file"}""");

        try
        {
            var builder = CreateBuilder();
            var plan = builder.Prepare((ctx) =>
            {
                ctx.UseConfigurationFile(path);
            });

            using var runtime = plan.Build();

            await Assert.That(runtime.Runtime.Configuration.Value).IsEqualTo("from-file");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldUseConfigurationPathEnvironmentVariableExtension()
    {
        var variableName = $"ALLURE_SDK_TEST_CONFIG_{Guid.NewGuid():N}";
        var path = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-config-{Guid.NewGuid():N}.json"
        );
        await File.WriteAllTextAsync(path, """{"value":"from-environment"}""");
        Environment.SetEnvironmentVariable(variableName, path);

        try
        {
            var builder = CreateBuilder();
            var plan = builder.Prepare((ctx) =>
            {
                ctx.UseConfigurationPathEnvironmentVariable(variableName);
            });

            using var runtime = plan.Build();

            await Assert.That(runtime.Runtime.Configuration.Value)
                .IsEqualTo("from-environment");
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
            File.Delete(path);
        }
    }

    [Test]
    [NotInParallel]
    public async Task ShouldLoadLegacyDefaultConfigurationFile()
    {
        using var state = new DefaultConfigurationState();
        await state.WriteLegacyConfiguration("legacy");

        var configuration = ResolveDefaultConfiguration();

        await Assert.That(configuration.Value).IsEqualTo("legacy");
    }

    [Test]
    [NotInParallel]
    public async Task ShouldLoadDottedDefaultConfigurationFile()
    {
        using var state = new DefaultConfigurationState();
        await state.WriteDottedConfiguration("dotted");

        var configuration = ResolveDefaultConfiguration();

        await Assert.That(configuration.Value).IsEqualTo("dotted");
    }

    [Test]
    [NotInParallel]
    public async Task ShouldPreferLegacyDefaultConfigurationFile()
    {
        using var state = new DefaultConfigurationState();
        await state.WriteLegacyConfiguration("legacy");
        await state.WriteDottedConfiguration("dotted");

        var configuration = ResolveDefaultConfiguration();

        await Assert.That(configuration.Value).IsEqualTo("legacy");
    }

    [Test]
    [NotInParallel]
    public async Task ShouldPreferEnvironmentConfigurationFile()
    {
        using var state = new DefaultConfigurationState();
        await state.WriteLegacyConfiguration("legacy");
        await state.WriteDottedConfiguration("dotted");
        await state.WriteEnvironmentConfiguration("environment");

        var configuration = ResolveDefaultConfiguration();

        await Assert.That(configuration.Value).IsEqualTo("environment");
    }

    static TestConfiguration ResolveDefaultConfiguration()
    {
        var builder = CreateBuilder();
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseDestination(_ => new InMemoryResultsDestination());
        });

        using var runtime = plan.Build();
        return runtime.Runtime.Configuration;
    }

    static AllureRuntimeBuilder<TestConfiguration> CreateBuilder() =>
        new("configuration-tests");

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string? Value { get; init; }
    }

    sealed class DefaultConfigurationState : IDisposable
    {
        const string EnvironmentVariableName = "ALLURE_CONFIG";

        static readonly string LegacyConfigurationPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "allureConfig.json"
        );

        static readonly string DottedConfigurationPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "allure.config.json"
        );

        readonly string? previousEnvironmentConfigurationPath =
            Environment.GetEnvironmentVariable(EnvironmentVariableName);
        readonly byte[]? previousLegacyConfiguration = ReadIfExists(
            LegacyConfigurationPath
        );
        readonly byte[]? previousDottedConfiguration = ReadIfExists(
            DottedConfigurationPath
        );
        string? environmentConfigurationPath;

        public DefaultConfigurationState()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableName, null);
            File.Delete(LegacyConfigurationPath);
            File.Delete(DottedConfigurationPath);
        }

        public Task WriteLegacyConfiguration(string value) =>
            WriteConfiguration(LegacyConfigurationPath, value);

        public Task WriteDottedConfiguration(string value) =>
            WriteConfiguration(DottedConfigurationPath, value);

        public async Task WriteEnvironmentConfiguration(string value)
        {
            this.environmentConfigurationPath = Path.Combine(
                Path.GetTempPath(),
                $"allure-sdk-config-{Guid.NewGuid():N}.json"
            );
            await WriteConfiguration(this.environmentConfigurationPath, value);
            Environment.SetEnvironmentVariable(
                EnvironmentVariableName,
                this.environmentConfigurationPath
            );
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(
                EnvironmentVariableName,
                this.previousEnvironmentConfigurationPath
            );
            File.Delete(LegacyConfigurationPath);
            File.Delete(DottedConfigurationPath);
            Restore(LegacyConfigurationPath, this.previousLegacyConfiguration);
            Restore(DottedConfigurationPath, this.previousDottedConfiguration);

            if (this.environmentConfigurationPath is not null)
            {
                File.Delete(this.environmentConfigurationPath);
            }
        }

        static byte[]? ReadIfExists(string path) =>
            File.Exists(path) ? File.ReadAllBytes(path) : null;

        static Task WriteConfiguration(string path, string value) =>
            File.WriteAllTextAsync(path, $$"""{"value":"{{value}}"}""");

        static void Restore(string path, byte[]? contents)
        {
            if (contents is not null)
            {
                File.WriteAllBytes(path, contents);
            }
        }
    }

    sealed class RecordingConfigurationSource : IAllureConfigurationSource<TestConfiguration>
    {
        readonly TestConfiguration configuration = new();

        public RecordingConfigurationSource(bool canLoad)
        {
            this.CanLoad = canLoad;
        }

        public RecordingConfigurationSource(TestConfiguration configuration)
        {
            this.configuration = configuration;
            this.CanLoad = true;
        }

        public string Name => "recording";

        public bool CanLoad { get; }

        public int LoadCount { get; private set; }

        public TrackedConfiguration<TestConfiguration> LoadConfiguration()
        {
            this.LoadCount++;
            return TrackedConfiguration.WithAllPropertiesSet(this.Name, this.configuration);
        }
    }
}
