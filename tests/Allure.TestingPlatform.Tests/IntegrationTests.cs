using System.Collections.Immutable;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;
using TUnit.Assertions.Enums;

namespace Allure.TestingPlatform.Tests;

public class IntegrationTests
{
    static TestNodeUpdateMessage TestStartMessage(SessionUid session) => new (
        session,
        new()
        {
            Uid = "1",
            DisplayName = "Foo",
            Properties = new(
                new InProgressTestNodeStateProperty()
            )
        }
    );

    static void TargetMethod(string foo) { }

    static AllureTestUpdateMessage TestUpdateMessage(SessionUid session) =>
        new (new(session.Value), new("1"))
        {
            Properties = [
                new AllureTestMethodProperty(
                    typeof(IntegrationTests)
                        .GetMethod(
                            nameof(TargetMethod),
                            BindingFlags.Static | BindingFlags.NonPublic
                        )
                )
                {
                    Arguments = ["Lorem Ipsum"]
                },
                new AllureLinksProperty([new(){ url = "1", name = "bar", type = "issue" }]),
            ]
        };

    static TestNodeUpdateMessage TestStopMessage(SessionUid session) => new (
        session,
        new()
        {
            Uid = "1",
            DisplayName = "Foo",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        }
    );

    [Test]
    public async Task ShouldRunDataConsumerWithAllureInitializedInProcess()
    {
        AllureConfiguration config = new()
        {
            Links = ["foo/{issue}/bar"],
        };
        var correlation = new SessionUidCorrelation();
        var writer = new InMemoryResultsWriter();
        Dictionary<Type, ITypeFormatter> typeFormatters = new()
        {
            { typeof(string), new TypeFormatterStub<string>("stub") },
        };
        var lifecycle = new AllureLifecycle(config, writer, typeFormatters);

        IServiceProvider useConfigurationServiceProvider = null;

        IServiceProvider setIsEnabledServiceProvider = null;
        AllureConfiguration setIsEnabledConfiguration = null;

        IServiceProvider useCorrelationServiceProvider = null;
        AllureConfiguration useCorrelationConfiguration = null;

        IServiceProvider useWriterServiceProvider = null;
        AllureConfiguration useWriterConfig = null;

        IServiceProvider useTypeFormattersServiceProvider = null;
        AllureConfiguration useTypeFormattersConfig = null;

        IServiceProvider useLifecycleServiceProvider = null;
        AllureConfiguration useLifecycleConfig = null;
        IAllureResultsWriter useLifecycleWriter = null;
        ImmutableDictionary<Type, ITypeFormatter> useLifecycleTypeFormatters = null;

        var builder = await TestApplication.CreateBuilderAsync([
            "--no-progress",
            "--no-ansi",
            "--output",
            "Normal",
            "--show-stdout",
            "None",
            "--show-stderr",
            "None",
        ]);
        builder.AddEmbeddedAllure(ctx =>
        {
            // Can't use watchdog with a nested MTP application
            ctx.DisableHostProcessWatchdog();

            ctx.UseConfiguration((sp) =>
            {
                useConfigurationServiceProvider = sp;
                return config;
            });
            ctx.SetIsEnabled((sp, cfg) =>
            {
                setIsEnabledServiceProvider = sp;
                setIsEnabledConfiguration = cfg;
                return true;
            });
            ctx.UseCorrelation((sp, cfg) =>
            {
                useCorrelationServiceProvider = sp;
                useCorrelationConfiguration = cfg;
                return correlation;
            });
            ctx.UseWriter((sp, cfg) =>
            {
                useWriterServiceProvider = sp;
                useWriterConfig = cfg;
                return writer;
            });
            ctx.UseTypeFormatters((sp, cfg) =>
            {
                useTypeFormattersServiceProvider = sp;
                useTypeFormattersConfig = cfg;
                return typeFormatters;
            });
            ctx.UseLifecycle((sp, deps) =>
            {
                useLifecycleServiceProvider = sp;
                useLifecycleConfig = deps.Config;
                useLifecycleWriter = deps.Writer;
                useLifecycleTypeFormatters = deps.TypeFormatters;
                return lifecycle;
            });
        });

        builder.RegisterTestFramework(
            serviceProvider => new TestFrameworkCapabilities(),
            (capabilities, serviceProvider) => new TestFrameworkStub(
                TestStartMessage,
                TestUpdateMessage,
                TestStopMessage
            )
        );

        using var app = await builder.BuildAsync();

        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(0);

        // Check if registration callbacks received the same service provider used for data consumer registration.
        var dataConsumer =
            await Assert.That(useConfigurationServiceProvider.GetRequiredService<AllureDataConsumer>())
                .IsNotNull();
        await Assert.That(setIsEnabledServiceProvider.GetRequiredService<AllureDataConsumer>())
            .IsSameReferenceAs(dataConsumer);
        await Assert.That(useCorrelationServiceProvider.GetRequiredService<AllureDataConsumer>())
            .IsSameReferenceAs(dataConsumer);
        await Assert.That(useWriterServiceProvider.GetRequiredService<AllureDataConsumer>())
            .IsSameReferenceAs(dataConsumer);
        await Assert.That(useTypeFormattersServiceProvider.GetRequiredService<AllureDataConsumer>())
            .IsSameReferenceAs(dataConsumer);
        await Assert.That(useLifecycleServiceProvider.GetRequiredService<AllureDataConsumer>())
            .IsSameReferenceAs(dataConsumer);

        // Check if registration callbacks received the created objects.
        await Assert.That(setIsEnabledConfiguration).IsSameReferenceAs(config);
        await Assert.That(useCorrelationConfiguration).IsSameReferenceAs(config);
        await Assert.That(useWriterConfig).IsSameReferenceAs(config);
        await Assert.That(useTypeFormattersConfig).IsSameReferenceAs(config);
        await Assert.That(useLifecycleConfig).IsSameReferenceAs(config);
        await Assert.That(useLifecycleWriter).IsSameReferenceAs(writer);
        await Assert.That(useLifecycleTypeFormatters).IsEquivalentTo(
            typeFormatters,
            CollectionOrdering.Matching
        );

        await Assert.That(dataConsumer.IsEnabledAsync()).IsTrue();

        // Check if the writer was actually used
        var testResult = await Assert.That(writer.TestResults).HasSingleItem();

        // Check if the config (Links property) was actually used.
        var link = await Assert.That(testResult.links).HasSingleItem();
        await Assert.That(link.url).IsEqualTo("foo/1/bar");
        await Assert.That(link.name).IsEqualTo("bar");
        await Assert.That(link.type).IsEqualTo("issue");

        // Check if type formatter was actually used.
        var parameter = await Assert.That(testResult.parameters).HasSingleItem();
        await Assert.That(parameter.name).IsEqualTo("foo");
        await Assert.That(parameter.value).IsEqualTo("stub");
    }

    [Test]
    public async Task ShouldDisableDataConsumerIfCliOptionSetToOff()
    {
        IServiceProvider capturedServiceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync([
            "--no-progress",
            "--no-ansi",
            "--output",
            "Normal",
            "--show-stdout",
            "None",
            "--show-stderr",
            "None",
            "--allure",
            "off"
        ]);
        builder.AddEmbeddedAllure((ctx) => ctx
            .DisableHostProcessWatchdog()
            .UseWriter((_, _) => new InMemoryResultsWriter()));

        builder.RegisterTestFramework(
            serviceProvider => new TestFrameworkCapabilities(),
            (capabilities, serviceProvider) =>
            {
                capturedServiceProvider = serviceProvider;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();
        var dataConsumer = capturedServiceProvider.GetService<AllureDataConsumer>();
        var applicationLifetime = capturedServiceProvider.GetService<AllureTestingPlatformInProcessOwner>();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        await Assert.That(dataConsumer).IsNull(); // disabled extensions aren't registered
        await Assert.That(applicationLifetime).IsNull();
    }

    [Test]
    public async Task ShouldBeAbleToDisableDataConsumerThroughBuilder()
    {
        IServiceProvider capturedServiceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync([
            "--no-progress",
            "--no-ansi",
            "--output",
            "Normal",
            "--show-stdout",
            "None",
            "--show-stderr",
            "None"
        ]);
        builder.AddEmbeddedAllure((ctx) => ctx
            .DisableHostProcessWatchdog()
            .UseWriter((_, _) => new InMemoryResultsWriter())
            .SetIsEnabled((_, _) => false));

        builder.RegisterTestFramework(
            serviceProvider => new TestFrameworkCapabilities(),
            (capabilities, serviceProvider) =>
            {
                capturedServiceProvider = serviceProvider;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();
        var dataConsumer = capturedServiceProvider.GetService<AllureDataConsumer>();

        await Assert.That(code).IsEqualTo(8); // test session run zero tests
        await Assert.That(dataConsumer).IsNull();
    }
}