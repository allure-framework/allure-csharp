using System.Reflection;
using Allure.Model;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests;

public class IntegrationTests
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

    static TestNodeUpdateMessage TestStartMessage(SessionUid session) => new(
        session,
        new()
        {
            Uid = "1",
            DisplayName = "Foo",
            Properties = new(new InProgressTestNodeStateProperty())
        }
    );

    static void TargetMethod(string foo) { }

    static AllureTestUpdateMessage TestUpdateMessage(SessionUid session) =>
        new(new(session.Value), new("1"))
        {
            Properties =
            [
                new AllureTestMethodProperty(
                    typeof(IntegrationTests).GetMethod(
                        nameof(TargetMethod),
                        BindingFlags.Static | BindingFlags.NonPublic
                    )
                )
                {
                    Arguments = ["Lorem Ipsum"]
                },
                new AllureLinksProperty(
                    [new Link { Url = "https://example.org/1", Name = "bar", Type = "issue" }]
                ),
            ]
        };

    static TestNodeUpdateMessage TestStopMessage(SessionUid session) => new(
        session,
        new()
        {
            Uid = "1",
            DisplayName = "Foo",
            Properties = new(new PassedTestNodeStateProperty())
        }
    );

    [Test]
    public async Task ShouldRunDataConsumerWithAllureInitializedInProcess()
    {
        AllureTestingPlatformConfiguration config = new()
        {
            IsProcessWatchdogEnabled = false,
        };
        var correlation = new SessionUidCorrelationStrategy();
        var writer = new InMemoryResultsDestination();
        IServiceProvider registrationServiceProvider = null;

        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);
        var runtimeHandle = builder.AddEmbeddedAllure(
            "integration-test",
            (context, serviceProvider) =>
            {
                registrationServiceProvider = serviceProvider;
                context.UseConfiguration(config);
                context.UseCorrelationStrategy(_ => correlation);
                context.UseDestination(_ => writer);
            }
        );

        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new TestFrameworkStub(TestStartMessage, TestUpdateMessage, TestStopMessage)
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(0);
        await Assert.That(runtimeHandle.ConfigurationReference.Value).IsSameReferenceAs(config);
        await Assert.That(runtimeHandle.RuntimeReference.Value.Configuration).IsSameReferenceAs(config);
        await Assert.That(runtimeHandle.RuntimeReference.Value.ResultsDestination).IsSameReferenceAs(writer);
        await Assert.That(runtimeHandle.RuntimeReference.Value.CorrelationStrategy).IsSameReferenceAs(correlation);

        var testResult = await Assert.That(writer.TestResults).HasSingleItem();
        var link = await Assert.That(testResult.Links).HasSingleItem();
        await Assert.That(link.Url).IsEqualTo("https://example.org/1");
        await Assert.That(link.Name).IsEqualTo("bar");
        await Assert.That(link.Type).IsEqualTo("issue");

        var parameter = await Assert.That(testResult.Parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("foo");
        await Assert.That(parameter.Value).IsEqualTo("\"Lorem Ipsum\"");
    }

    [Test]
    public async Task ShouldDisableDataConsumerIfCliOptionSetToOff()
    {
        IServiceProvider serviceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync(
            [.. DefaultArgs, "--allure", "off"]
        );
        builder.AddEmbeddedAllure(
            "integration-test",
            (context, _) => context.DisableHostProcessWatchdog()
        );
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, provider) =>
            {
                serviceProvider = provider;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8);
        await Assert.That(serviceProvider.GetService<AllureDataConsumer>()).IsNull();
    }

    [Test]
    public async Task ShouldBeAbleToDisableDataConsumerThroughBuilder()
    {
        IServiceProvider serviceProvider = null;
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);
        builder.AddEmbeddedAllure(
            "integration-test",
            (context, _) =>
            {
                context.DisableHostProcessWatchdog();
                context.Disable();
            }
        );
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, provider) =>
            {
                serviceProvider = provider;
                return new TestFrameworkStub();
            }
        );

        using var app = await builder.BuildAsync();
        var code = await app.RunAsync();

        await Assert.That(code).IsEqualTo(8);
        await Assert.That(serviceProvider.GetService<AllureDataConsumer>()).IsNull();
    }
}
