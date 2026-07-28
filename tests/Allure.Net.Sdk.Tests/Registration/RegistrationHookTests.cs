using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Registration;

public class RegistrationHookTests
{
    [Test]
    public async Task ShouldRunHooksInOrderWithInitiallyResolvedConfiguration()
    {
        var configuration = new TestConfiguration();
        var calls = new List<string>();
        var first = new RecordingRuntimeHook<TestConfiguration>(
            _ => calls.Add("first")
        );
        var second = new RecordingRuntimeHook<TestConfiguration>(
            _ => calls.Add("second")
        );
        var builder = CreateBuilder();
        builder.UseConfiguration(configuration);
        builder.UseRegistrationHooks(received =>
        {
            calls.Add(
                ReferenceEquals(received, configuration)
                    ? "configuration"
                    : "unexpected"
            );
            return [first, null, second];
        });

        var runtime = builder.Build();

        await Assert.That(calls)
            .IsEquivalentTo(["configuration", "first", "second"]);
        await Assert.That(first.CallCount).IsEqualTo(1);
        await Assert.That(second.CallCount).IsEqualTo(1);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(configuration);
    }

    [Test]
    public async Task ShouldResolveConfigurationAgainWhenHookChangesSources()
    {
        var original = new TestConfiguration { Value = "original" };
        var replacement = new TestConfiguration { Value = "replacement" };
        var originalLoads = 0;
        var replacementLoads = 0;
        var hook = new RecordingRuntimeHook<TestConfiguration>(context =>
            context.UseConfigurationSources(() =>
            {
                replacementLoads++;
                return [DelegateConfigurationSource.Create("replacement", () => replacement)];
            })
        );
        var builder = CreateBuilder();
        builder.UseConfigurationSources(() =>
        {
            originalLoads++;
            return [DelegateConfigurationSource.Create("original", () => original)];
        });
        builder.UseRegistrationHooks(_ => [hook]);

        var runtime = builder.Build();

        await Assert.That(originalLoads).IsEqualTo(1);
        await Assert.That(replacementLoads).IsEqualTo(1);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(replacement);
    }

    [Test]
    public async Task ShouldNotReloadConfigurationForUnrelatedHookChanges()
    {
        var sourceFactoryCalls = 0;
        var destination = new InMemoryResultsDestination();
        var hook = new RecordingRuntimeHook<TestConfiguration>(
            context => context.UseDestination(_ => destination)
        );
        var builder = CreateBuilder();
        builder.UseConfigurationSources(() =>
        {
            sourceFactoryCalls++;
            return [
                DelegateConfigurationSource.Create(
                    "configuration",
                    () => new TestConfiguration()
                )
            ];
        });
        builder.UseRegistrationHooks(_ => [hook]);

        var runtime = builder.Build();

        await Assert.That(sourceFactoryCalls).IsEqualTo(1);
        await Assert.That(runtime.ResultsDestination)
            .IsSameReferenceAs(destination);
    }

    static AllureRuntimeBuilder<
        TestConfiguration,
        RecordingRuntimeHook<TestConfiguration>,
        RecordingEndpointHook<TestConfiguration>
    > CreateBuilder() =>
        new("hook-tests");

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string? Value { get; init; }
    }
}
