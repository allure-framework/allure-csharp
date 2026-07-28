using System.Collections.Immutable;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Links;

public class AddLinkTests
{
    [Test]
    public async Task AddLinkAppliesTemplateAndAddsLinkToCurrentTest()
    {
        var environment = CreateEnvironment();
        var test = NewTest();
        var link = new Link
        {
            Type = "issue",
            Url = "123",
            Name = "bug",
        };

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddLink(link);
        });

        await Assert.That(test.Links.Single()).IsSameReferenceAs(link);
        await Assert.That(link.Url).IsEqualTo("https://issues/123");
        await Assert.That(link.Name).IsEqualTo("Issue bug");
    }

    [Test]
    public async Task AddLinkAsyncAppliesTemplateAndAddsLinkToCurrentTest()
    {
        var environment = CreateEnvironment();
        var test = NewTest();
        var link = new Link
        {
            Type = "issue",
            Url = "123",
            Name = "bug",
        };

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddLinkAsync(
                link,
                CancellationToken.None
            );
        });

        await Assert.That(test.Links.Single()).IsSameReferenceAs(link);
        await Assert.That(link.Url).IsEqualTo("https://issues/123");
        await Assert.That(link.Name).IsEqualTo("Issue bug");
    }

    [Test]
    public async Task AddLinkDoesNotTemplateAbsoluteUrl()
    {
        var environment = CreateEnvironment();
        var test = NewTest();
        var link = new Link
        {
            Type = "issue",
            Url = "https://elsewhere/123",
            Name = "bug",
        };

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddLink(link);
        });

        await Assert.That(link.Url).IsEqualTo("https://elsewhere/123");
        await Assert.That(link.Name).IsEqualTo("bug");
    }

    [Test]
    public async Task AddLinkThrowsIfNoTestRunning()
    {
        var environment = CreateEnvironment();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddLink(new Link { Url = "123" })
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddLinkAsyncThrowsIfNoTestRunning()
    {
        var environment = CreateEnvironment();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddLinkAsync(
                new Link { Url = "123" },
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static AllureApiTestEnvironment CreateEnvironment() =>
        AllureApiTestEnvironment.Create(new()
        {
            LinkTemplates = ImmutableDictionary<
                string,
                AllureLinkTemplate
            >.Empty.Add(
                "issue",
                new("https://issues/{0}", "Issue {0}")
            ),
        });

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
