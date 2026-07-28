using System.Collections.Immutable;
using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Links;

public class AddLinksTests
{
    [Test]
    public async Task AddLinksAppliesTemplatesAndAddsLinksToCurrentTest()
    {
        var environment = CreateEnvironment();
        var test = NewTest();
        Link[] links =
        [
            new() { Type = "issue", Url = "123" },
            new() { Type = "issue", Url = "456", Name = "second" },
        ];

        environment.Run(current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            AllureApi.AddLinks(links);
        });

        await Assert.That(test.Links).IsEquivalentTo(links);
        await Assert.That(links.Select(link => link.Url))
            .IsEquivalentTo(["https://issues/123", "https://issues/456"]);
        await Assert.That(links.Select(link => link.Name!))
            .IsEquivalentTo(["Issue 123", "Issue second"]);
    }

    [Test]
    public async Task AddLinksAsyncAppliesTemplatesAndAddsLinksToCurrentTest()
    {
        var environment = CreateEnvironment();
        var test = NewTest();
        Link[] links =
        [
            new() { Type = "issue", Url = "123" },
            new() { Type = "issue", Url = "456", Name = "second" },
        ];

        await environment.RunAsync(async current =>
        {
            current.Runtime.LifecycleApi.StartTest(test);
            await AllureApi.AddLinksAsync(
                links,
                CancellationToken.None
            );
        });

        await Assert.That(test.Links).IsEquivalentTo(links);
        await Assert.That(links.Select(link => link.Url))
            .IsEquivalentTo(["https://issues/123", "https://issues/456"]);
        await Assert.That(links.Select(link => link.Name!))
            .IsEquivalentTo(["Issue 123", "Issue second"]);
    }

    [Test]
    public async Task AddLinksThrowsIfNoTestRunning()
    {
        var environment = CreateEnvironment();

        await Assert.That(() => environment.Run(
            _ => AllureApi.AddLinks(new Link { Url = "123" })
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddLinksAsyncThrowsIfNoTestRunning()
    {
        var environment = CreateEnvironment();

        await Assert.That(() => environment.RunAsync(
            _ => AllureApi.AddLinksAsync(
                [new Link { Url = "123" }],
                CancellationToken.None
            )
        )).Throws<InvalidOperationException>();
    }

    static AllureApiTestEnvironment CreateEnvironment() =>
        AllureApiTestEnvironment.Create(new()
        {
            LinkTemplates = new Dictionary<string, AllureLinkTemplate>
            {
                ["issue"] = new("https://issues/{0}", "Issue {0}"),
            }.ToImmutableDictionary(),
        });

    static AllureTestResult NewTest() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };
}
