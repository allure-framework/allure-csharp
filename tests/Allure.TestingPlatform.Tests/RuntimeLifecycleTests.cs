using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Services;
using TUnit.Assertions.Enums;

namespace Allure.TestingPlatform.Tests;

public class RuntimeLifecycleTests
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
    public async Task ShouldConfigureAndStartRuntimeInOrder()
    {
        var calls = new List<string>();
        var events = new List<AllureTestingPlatformRuntimeState>();

        AllureConfiguration config = new();
        var logger = new LoggerSpy();
        var writer = new InMemoryResultsWriter();
        var correlation = new TestingPlatformSessionUidCorrelationStrategy();
        Dictionary<Type, ITypeFormatter> typeFormatters = new()
        {
            { typeof(string), new TypeFormatterStub<string>("stub") },
        };
        var lifecycle = new AllureLifecycle(config, writer, typeFormatters);

        IServiceProvider serviceProvider = null;
        AllureConfiguration loggerConfig = null;
        AllureConfiguration isEnabledConfig = null;
        AllureConfiguration writerConfig = null;
        AllureConfiguration typeFormattersConfig = null;
        AllureConfiguration correlationConfig = null;
        AllureLifecycleFactoryContext lifecycleDeps = null;
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);
        var runtimeReferences = builder.AddEmbeddedAllure(ctx =>
        {
            ctx.DisableHostProcessWatchdog();

            ctx.UseConfiguration(sp =>
            {
                serviceProvider = sp;
                calls.Add("configuration");
                return config;
            });
            ctx.UseLogger((_, cfg) =>
            {
                calls.Add("logger");
                loggerConfig = cfg;
                return logger;
            });
            ctx.SetIsEnabled((_, cfg) =>
            {
                calls.Add("isEnabled");
                isEnabledConfig = cfg;
                return true;
            });
            ctx.UseWriter((_, cfg) =>
            {
                calls.Add("writer");
                writerConfig = cfg;
                return writer;
            });
            ctx.UseTypeFormatters((_, cfg) =>
            {
                calls.Add("typeFormatters");
                typeFormattersConfig = cfg;
                return typeFormatters;
            });
            ctx.UseCorrelation((_, cfg) =>
            {
                calls.Add("correlation");
                correlationConfig = cfg;
                return correlation;
            });
            ctx.UseLifecycle((_, deps) =>
            {
                calls.Add("lifecycle");
                lifecycleDeps = deps;
                return lifecycle;
            });
            ctx.SetSdkEventHandlers(sdkEvents =>
            {
                sdkEvents.OnConfigured += runtime =>
                {
                    calls.Add("configured");
                    events.Add(runtime);
                };
                sdkEvents.OnLive += runtime =>
                {
                    calls.Add("live");
                    events.Add(runtime);
                };
            });
        });

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new TestFrameworkStub()
        );

        using var app = await builder.BuildAsync();

        var runtimeReference = runtimeReferences.GetRuntimeReference(serviceProvider);
        await Assert.That(runtimeReference.CurrentRuntime).IsTypeOf<EnabledAllureTestingPlatformRuntime>();
        await Assert.That(runtimeReference.CurrentRuntime.Phase)
            .IsEqualTo(AllureTestingPlatformRuntimePhase.Configured);
        await Assert.That(calls).IsEquivalentTo(
            ["configuration", "logger", "isEnabled", "configured"],
            CollectionOrdering.Matching
        );

        var applicationLifetime = serviceProvider.GetRequiredService<AllureTestingPlatformInProcessRuntimeController>();
        await Assert.That(applicationLifetime.IsEnabledAsync()).IsTrue();
        await Assert.That(calls).IsEquivalentTo(
            ["configuration", "logger", "isEnabled", "configured"],
            CollectionOrdering.Matching
        );

        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        var liveRuntime =
            await Assert.That(runtimeReference.CurrentRuntime).IsTypeOf<LiveAllureTestingPlatformRuntime>();
        await Assert.That(calls).IsEquivalentTo(
            [
                "configuration",
                "logger",
                "isEnabled",
                "configured",
                "writer",
                "typeFormatters",
                "correlation",
                "lifecycle",
                "live"
            ],
            CollectionOrdering.Matching
        );
        await Assert.That(events).Count().IsEqualTo(2);

        await Assert.That(loggerConfig).IsSameReferenceAs(config);
        await Assert.That(isEnabledConfig).IsSameReferenceAs(config);
        await Assert.That(writerConfig).IsSameReferenceAs(config);
        await Assert.That(typeFormattersConfig).IsSameReferenceAs(config);
        await Assert.That(correlationConfig).IsSameReferenceAs(config);
        await Assert.That(lifecycleDeps.Config).IsSameReferenceAs(config);
        await Assert.That(lifecycleDeps.Writer).IsSameReferenceAs(writer);
        await Assert.That(lifecycleDeps.TypeFormatters).IsEquivalentTo(
            typeFormatters,
            CollectionOrdering.Matching
        );

        var configuredRuntime = await Assert.That(events[0]).IsTypeOf<EnabledAllureTestingPlatformRuntime>();
        await Assert.That(configuredRuntime.Configuration).IsSameReferenceAs(config);
        await Assert.That(configuredRuntime.Logger).IsSameReferenceAs(logger);

        await Assert.That(events[1]).IsSameReferenceAs(liveRuntime);
        await Assert.That(liveRuntime.Configuration).IsSameReferenceAs(config);
        await Assert.That(liveRuntime.Logger).IsSameReferenceAs(logger);
        await Assert.That(liveRuntime.Writer).IsSameReferenceAs(writer);
        await Assert.That(liveRuntime.CorrelationStrategy).IsSameReferenceAs(correlation);
        await Assert.That(liveRuntime.Lifecycle).IsSameReferenceAs(lifecycle);
        await Assert.That(liveRuntime.TypeFormatters).IsEquivalentTo(
            typeFormatters.ToImmutableDictionary(),
            CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task ShouldConfigureDisabledRuntimeWithoutStartingIt()
    {
        var calls = new List<string>();
        var events = new List<AllureTestingPlatformRuntimeState>();

        AllureConfiguration config = new();
        var logger = new LoggerSpy();

        IServiceProvider serviceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);
        var runtimeReferences = builder.AddEmbeddedAllure(ctx =>
        {
            ctx.DisableHostProcessWatchdog();

            ctx.UseConfiguration(sp =>
            {
                serviceProvider = sp;
                calls.Add("configuration");
                return config;
            });
            ctx.UseLogger((_, cfg) =>
            {
                calls.Add("logger");
                return logger;
            });
            ctx.SetIsEnabled((_, cfg) =>
            {
                calls.Add("isEnabled");
                return false;
            });
            ctx.UseCorrelation((_, _) =>
            {
                calls.Add("correlation");
                throw new NotImplementedException();
            });
            ctx.UseWriter((_, _) =>
            {
                calls.Add("writer");
                throw new NotImplementedException();
            });
            ctx.UseTypeFormatters((_, _) =>
            {
                calls.Add("formatters");
                throw new NotImplementedException();
            });
            ctx.UseLifecycle((_, _) =>
            {
                calls.Add("lifecycle");
                throw new NotImplementedException();
            });
            ctx.SetSdkEventHandlers(sdkEvents =>
            {
                sdkEvents.OnConfigured += runtime =>
                {
                    calls.Add("configured");
                    events.Add(runtime);
                };
                sdkEvents.OnLive += runtime =>
                {
                    calls.Add("live");
                    events.Add(runtime);
                };
            });
        });

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new TestFrameworkStub()
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        var runtimeReference = runtimeReferences.GetRuntimeReference(serviceProvider);
        var disabledRuntime =
            await Assert.That(runtimeReference.CurrentRuntime).IsTypeOf<DisabledAllureTestingPlatformRuntime>();
        await Assert.That(disabledRuntime.IsEnabled).IsFalse();
        await Assert.That(disabledRuntime.Phase).IsEqualTo(AllureTestingPlatformRuntimePhase.Disabled);
        await Assert.That(disabledRuntime.Configuration).IsSameReferenceAs(config);
        await Assert.That(disabledRuntime.Logger).IsSameReferenceAs(logger);
        await Assert.That(events).HasSingleItem();
        await Assert.That(events[0]).IsSameReferenceAs(disabledRuntime);
        await Assert.That(calls).IsEquivalentTo(
            ["configuration", "logger", "isEnabled", "configured"],
            CollectionOrdering.Matching
        );
        await Assert.That(serviceProvider.GetService<AllureTestingPlatformInProcessRuntimeController>()).IsNull();
        await Assert.That(serviceProvider.GetService<AllureDataConsumer>()).IsNull();
    }

    [Test]
    public async Task ShouldSuppressRuntimeWhenCliOptionIsOff()
    {
        var calls = new List<string>();
        var args = DefaultArgs.Concat(["--allure", "off"]).ToArray();

        IServiceProvider serviceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync(args);
        var runtimeReferences = builder.AddEmbeddedAllure(ctx =>
        {
            ctx.DisableHostProcessWatchdog();

            ctx.UseConfiguration(_ =>
            {
                calls.Add("configuration");
                return new();
            });
            ctx.SetIsEnabled((_, _) =>
            {
                calls.Add("isEnabled");
                return true;
            });
            ctx.SetSdkEventHandlers(sdkEvents =>
            {
                sdkEvents.OnConfigured += _ => calls.Add("configured");
                sdkEvents.OnLive += _ => calls.Add("live");
            });

            ctx.UseCorrelation((_, _) =>
            {
                calls.Add("correlation");
                throw new NotImplementedException();
            });
            ctx.UseWriter((_, _) =>
            {
                calls.Add("writer");
                throw new NotImplementedException();
            });
            ctx.UseTypeFormatters((_, _) =>
            {
                calls.Add("formatters");
                throw new NotImplementedException();
            });
            ctx.UseLifecycle((_, _) =>
            {
                calls.Add("lifecycle");
                throw new NotImplementedException();
            });
        });

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, sp) =>
            {
                serviceProvider = sp;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        var runtimeReference = runtimeReferences.GetRuntimeReference(serviceProvider);
        await Assert.That(runtimeReference.CurrentRuntime).IsTypeOf<SuppressedAllureTestingPlatformRuntime>();
        await Assert.That(runtimeReference.CurrentRuntime.IsEnabled).IsFalse();
        await Assert.That(runtimeReference.CurrentRuntime.Phase)
            .IsEqualTo(AllureTestingPlatformRuntimePhase.Suppressed);
        await Assert.That(calls).IsEmpty();
        await Assert.That(serviceProvider.GetService<AllureTestingPlatformInProcessRuntimeController>()).IsNull();
        await Assert.That(serviceProvider.GetService<AllureDataConsumer>()).IsNull();
    }

    [Test]
    public async Task ShouldThrowWhenLiveRuntimeIsStartedAgain()
    {
        IServiceProvider serviceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);
        var runtimeReferences = builder.AddEmbeddedAllure(ctx => ctx
            .DisableHostProcessWatchdog()
            .UseWriter((_, _) => new InMemoryResultsWriter()));

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, sp) =>
            {
                serviceProvider = sp;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        var runtimeReference = runtimeReferences.GetRuntimeReference(serviceProvider);
        await Assert.That(runtimeReference.CurrentRuntime).IsTypeOf<LiveAllureTestingPlatformRuntime>();

        var applicationLifetime = serviceProvider.GetRequiredService<AllureTestingPlatformInProcessRuntimeController>();
        await Assert.That(async () =>
            await applicationLifetime.BeforeRunAsync(CancellationToken.None)
        ).Throws<InvalidOperationException>().WithMessage(
            "Cannot start Allure.TestingPlatform runtime: the runtime is already live."
        );
    }
}
