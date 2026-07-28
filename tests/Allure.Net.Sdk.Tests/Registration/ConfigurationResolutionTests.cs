using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Net.Sdk.Tests.Infrastructure;

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

        builder.UseConfigurationSources(() => [skipped, first, later]);
        var runtime = builder.Build();

        await Assert.That(runtime.Configuration).IsSameReferenceAs(expected);
        await Assert.That(skipped.LoadCount).IsEqualTo(0);
        await Assert.That(first.LoadCount).IsEqualTo(1);
        await Assert.That(later.LoadCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldCreateDefaultConfigurationWhenNoSourceCanLoad()
    {
        var builder = CreateBuilder();
        builder.UseConfigurationSources(
            () => [new RecordingConfigurationSource(canLoad: false)]
        );

        var runtime = builder.Build();

        await Assert.That(runtime.Configuration).IsNotNull();
        await Assert.That(runtime.Configuration.Value).IsNull();
    }

    [Test]
    public async Task ShouldEvaluateConfigurationSourceFactoryAtBuildTime()
    {
        var factoryCalls = 0;
        var expected = new TestConfiguration();
        var builder = CreateBuilder();
        builder.UseConfigurationSources(() =>
        {
            factoryCalls++;
            return [new RecordingConfigurationSource(expected)];
        });

        await Assert.That(factoryCalls).IsEqualTo(0);
        var runtime = builder.Build();

        await Assert.That(factoryCalls).IsEqualTo(1);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task ShouldUseExplicitConfigurationExtension()
    {
        var expected = new TestConfiguration();
        var builder = CreateBuilder();

        builder.UseConfiguration(expected);
        var runtime = builder.Build();

        await Assert.That(runtime.Configuration).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task ShouldUseConfigurationSourceExtension()
    {
        var expected = new TestConfiguration();
        var sourceFactoryCalls = 0;
        var builder = CreateBuilder();

        builder.UseConfigurationSource(() =>
        {
            sourceFactoryCalls++;
            return new RecordingConfigurationSource(expected);
        });
        var runtime = builder.Build();

        await Assert.That(sourceFactoryCalls).IsEqualTo(1);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(expected);
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
            builder.UseConfigurationFile(path);

            var runtime = builder.Build();

            await Assert.That(runtime.Configuration.Value).IsEqualTo("from-file");
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
            builder.UseConfigurationPathEnvironmentVariable(variableName);

            var runtime = builder.Build();

            await Assert.That(runtime.Configuration.Value)
                .IsEqualTo("from-environment");
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
            File.Delete(path);
        }
    }

    static AllureRuntimeBuilder<
        TestConfiguration,
        RecordingRuntimeHook<TestConfiguration>,
        RecordingEndpointHook<TestConfiguration>
    > CreateBuilder() =>
        new("configuration-tests");

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string? Value { get; init; }
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

        public TestConfiguration LoadConfiguration()
        {
            this.LoadCount++;
            return this.configuration;
        }
    }
}
