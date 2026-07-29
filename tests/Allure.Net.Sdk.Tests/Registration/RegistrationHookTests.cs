using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
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

    [Test]
    public async Task ShouldPassOriginalConfigurationToHookFactoryWhenSourcesChange()
    {
        var original = new TestConfiguration { Value = "original" };
        var replacement = new TestConfiguration { Value = "replacement" };
        TestConfiguration? received = null;
        var builder = CreateBuilder();
        builder.UseConfiguration(original);
        builder.UseRegistrationHooks(configuration =>
        {
            received = configuration;
            return
            [
                new RecordingRuntimeHook<TestConfiguration>(context =>
                    context.UseConfiguration(replacement)
                ),
            ];
        });

        var runtime = builder.Build();

        await Assert.That(received).IsSameReferenceAs(original);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(replacement);
    }

    [Test]
    public async Task ShouldUseConfigurationFromLastHookThatChangesSources()
    {
        var first = new TestConfiguration { Value = "first" };
        var second = new TestConfiguration { Value = "second" };
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseRegistrationHooks(_ =>
        [
            new RecordingRuntimeHook<TestConfiguration>(context => context.UseConfiguration(first)),
            new RecordingRuntimeHook<TestConfiguration>(context => context.UseConfiguration(second)),
        ]);

        var runtime = builder.Build();

        await Assert.That(runtime.Configuration).IsSameReferenceAs(second);
    }

    [Test]
    public async Task ShouldPropagateRuntimeHookExceptions()
    {
        var laterHook = new RecordingRuntimeHook<TestConfiguration>();
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseRegistrationHooks(_ =>
        [
            new RecordingRuntimeHook<TestConfiguration>(_ => throw new InvalidOperationException("hook failed")),
            laterHook,
        ]);

        await Assert.That(builder.Build)
            .Throws<InvalidOperationException>()
            .WithMessageContaining("hook failed");
        await Assert.That(laterHook.CallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldPropagateRuntimeHookFactoryExceptions()
    {
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseRegistrationHooks(_ => throw new InvalidOperationException("factory failed"));

        await Assert.That(builder.Build)
            .Throws<InvalidOperationException>()
            .WithMessageContaining("factory failed");
    }

    [Test]
    public async Task ShouldUseComponentsRegisteredByRuntimeHook()
    {
        var serializer = new RecordingParameterSerializer(_ => "hook serializer");
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseRegistrationHooks(_ =>
        [
            new RecordingRuntimeHook<TestConfiguration>(context =>
                context.UseParameterSerializer(_ => serializer)
            ),
        ]);

        var runtime = builder.Build();

        await Assert.That(runtime.ParameterSerializer).IsSameReferenceAs(serializer);
    }

    [Test]
    public async Task ShouldRunEndpointHooksInOrderWithFinalConfiguration()
    {
        var original = new TestConfiguration { Value = "original" };
        var final = new TestConfiguration { Value = "final" };
        var calls = new List<string>();
        var first = new RecordingEndpointHook<TestConfiguration>(_ => calls.Add("first"));
        var second = new RecordingEndpointHook<TestConfiguration>(_ => calls.Add("second"));
        var builder = CreateBuilder();
        builder.UseConfiguration(original);
        builder.UseRegistrationHooks(_ =>
        [
            new RecordingRuntimeHook<TestConfiguration>(context => context.UseConfiguration(final)),
        ]);
        builder.RegisterInProcessEndpoint(
            $"endpoint-hooks-{Guid.NewGuid():N}",
            (_, context) =>
            {
                context.UseCurrentScopePredicate(_ => false);
                context.UseGlobalScopePredicate(_ => false);
                context.UseRegistrationHooks(received =>
                {
                    calls.Add(ReferenceEquals(received, final) ? "configuration" : "unexpected");
                    return [first, null, second];
                });
            }
        );

        var runtime = builder.Build();

        await Assert.That(calls.Count).IsEqualTo(3);
        await Assert.That(calls[0]).IsEqualTo("configuration");
        await Assert.That(calls[1]).IsEqualTo("first");
        await Assert.That(calls[2]).IsEqualTo("second");
        await Assert.That(first.CallCount).IsEqualTo(1);
        await Assert.That(second.CallCount).IsEqualTo(1);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(final);
    }

    [Test]
    public async Task ShouldPropagateEndpointHookExceptions()
    {
        var laterHook = new RecordingEndpointHook<TestConfiguration>();
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.RegisterInProcessEndpoint(
            $"endpoint-hook-failure-{Guid.NewGuid():N}",
            (_, context) => context.UseRegistrationHooks(_ =>
            [
                new RecordingEndpointHook<TestConfiguration>(_ =>
                    throw new InvalidOperationException("endpoint hook failed")
                ),
                laterHook,
            ])
        );

        await Assert.That(builder.Build)
            .Throws<InvalidOperationException>()
            .WithMessageContaining("endpoint hook failed");
        await Assert.That(laterHook.CallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldUseRuntimeRegistrationHookFromConfiguration()
    {
        ReflectionRuntimeHook.Calls.Clear();
        var builder = CreateReflectionBuilder();
        builder.UseConfiguration(new AllureConfiguration
        {
            RegistrationHook = typeof(ReflectionRuntimeHook).AssemblyQualifiedName,
        });

        builder.Build();

        var calls = ReflectionRuntimeHook.Calls.ToArray();
        await Assert.That(calls.Length).IsEqualTo(1);
        await Assert.That(calls[0]).IsEqualTo("configuration");
        ReflectionRuntimeHook.Calls.Clear();
    }

    [Test]
    [NotInParallel]
    public async Task ShouldRunEnvironmentRegistrationHookBeforeConfiguredHook()
    {
        const string variableName = "ALLURE_RUNTIME_REGISTRATION_HOOK";
        var previous = Environment.GetEnvironmentVariable(variableName);
        ReflectionRuntimeHook.Calls.Clear();
        Environment.SetEnvironmentVariable(
            variableName,
            typeof(EnvironmentReflectionRuntimeHook).AssemblyQualifiedName
        );

        try
        {
            var builder = CreateReflectionBuilder();
            builder.UseConfiguration(new AllureConfiguration
            {
                RegistrationHook = typeof(ReflectionRuntimeHook).AssemblyQualifiedName,
            });

            builder.Build();

            var calls = ReflectionRuntimeHook.Calls.ToArray();
            await Assert.That(calls.Length).IsEqualTo(2);
            await Assert.That(calls[0]).IsEqualTo("environment");
            await Assert.That(calls[1]).IsEqualTo("configuration");
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, previous);
            ReflectionRuntimeHook.Calls.Clear();
        }
    }

    [Test]
    public async Task ShouldResolveReflectionHooksAndIgnoreMissingValues()
    {
        var typeName = typeof(ReflectionRuntimeHook).AssemblyQualifiedName!;
        var fromConfiguration = ReflectionHooks
            .FromConfiguration<AllureConfiguration, IAllureRuntimeRegistrationHook>(
                new() { RegistrationHook = typeName }
            );
        var variableName = $"ALLURE_SDK_TEST_HOOK_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(variableName, null);

        var fromEnvironment = ReflectionHooks
            .FromEnvironmentVariable<IAllureRuntimeRegistrationHook>(variableName);

        await Assert.That(fromConfiguration).IsTypeOf<ReflectionRuntimeHook>();
        await Assert.That(fromEnvironment).IsNull();
        await Assert.That(
            ReflectionHooks.FromConfiguration<AllureConfiguration, IAllureRuntimeRegistrationHook>(new())
        ).IsNull();
    }

    [Test]
    public async Task ShouldRejectInvalidReflectionHooks()
    {
        await Assert.That(() => ReflectionHooks.FromConfiguration<AllureConfiguration, IAllureRuntimeRegistrationHook>(
            new() { RegistrationHook = typeof(NotARegistrationHook).AssemblyQualifiedName }
        )).Throws<InvalidOperationException>().WithMessageContaining("must implement");

        await Assert.That(() => ReflectionHooks.FromConfiguration<AllureConfiguration, IAllureRuntimeRegistrationHook>(
            new() { RegistrationHook = typeof(PrivateConstructorRuntimeHook).AssemblyQualifiedName }
        )).Throws<InvalidOperationException>().WithMessageContaining("public parameterless constructor");

        await Assert.That(() => ReflectionHooks.FromConfiguration<AllureConfiguration, IAllureRuntimeRegistrationHook>(
            new() { RegistrationHook = "No.Such.Hook, No.Such.Assembly" }
        )).Throws<FileNotFoundException>();
    }

    static TestRuntimeBuilder<TestConfiguration> CreateBuilder() =>
        new("hook-tests");

    static AllureRuntimeBuilder CreateReflectionBuilder() => new("reflection-hook-tests");

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string? Value { get; init; }
    }

    public class ReflectionRuntimeHook : IAllureRuntimeRegistrationHook
    {
        public static System.Collections.Concurrent.ConcurrentQueue<string> Calls { get; } = [];

        public void SetUp(IAllureRuntimeRegistrationContext context)
            => Calls.Enqueue("configuration");
    }

    public sealed class EnvironmentReflectionRuntimeHook : IAllureRuntimeRegistrationHook
    {
        public void SetUp(IAllureRuntimeRegistrationContext context) =>
            ReflectionRuntimeHook.Calls.Enqueue("environment");
    }

    public sealed class NotARegistrationHook;

    public sealed class PrivateConstructorRuntimeHook : IAllureRuntimeRegistrationHook
    {
        PrivateConstructorRuntimeHook() { }

        public void SetUp(IAllureRuntimeRegistrationContext context) { }
    }
}
