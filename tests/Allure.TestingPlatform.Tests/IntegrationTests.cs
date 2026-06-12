using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Messages;
using Allure.TestingPlatform.Properties;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

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

    static AllureTestUpdateMessage TestUpdateMessage(SessionUid session) => new (session, "1")
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
        var writer = new InMemoryResultsWriter();
        Dictionary<Type, ITypeFormatter> typeFormatters = new()
        {
            { typeof(string), new TypeFormatterStub<string>("stub") },
        };
        var lifecycle = new AllureLifecycle(config, writer, typeFormatters);

        AllureConfiguration useWriterConfig = null;

        AllureConfiguration useTypeFormattersConfig = null;

        AllureConfiguration useLifecycleConfig = null;
        IAllureResultsWriter useLifecycleWriter = null;
        Dictionary<Type, ITypeFormatter> useLifecycleTypeFormatters = null;

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
        builder.AddAllure(ctx =>
        {
            ctx.UseConfiguration(() => config);
            ctx.UseWriter((cfg) =>
            {
                useWriterConfig = cfg;
                return writer;
            });
            ctx.UseTypeFormatters((cfg) =>
            {
                useTypeFormattersConfig = cfg;
                return typeFormatters;
            });
            ctx.UseLifecycle((deps) =>
            {
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

        // Check if registration callbacks received the created objects.
        await Assert.That(useWriterConfig).IsSameReferenceAs(config);
        await Assert.That(useTypeFormattersConfig).IsSameReferenceAs(config);
        await Assert.That(useLifecycleConfig).IsSameReferenceAs(config);
        await Assert.That(useLifecycleWriter).IsSameReferenceAs(writer);
        await Assert.That(useLifecycleTypeFormatters).IsSameReferenceAs(typeFormatters);

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

    static void TargetMethod(string foo) { }
}