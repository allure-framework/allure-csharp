using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class LinkTests
{
    [Test]
    public async Task CheckLinksRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddLinksAtRuntime);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(4);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && l["type"] is null);
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && l["type"] is null);
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "name-3"
                && (string)l["type"] == "type-3");
        await Assert.That(links[3]).Satisfies(static (l) =>
            (string)l["url"] == "url-4"
                && (string)l["name"] == "name-4"
                && (string)l["type"] == "type-4");
    }

    [Test]
    public async Task CheckLegacyLinkAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyLinkAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "name-3"
                && (string)l["type"] == "link");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "link");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && (string)l["name"] == "url-1"
                && (string)l["type"] == "link");
    }

    [Test]
    public async Task CheckLinkAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(4);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && l["type"] is null);
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && l["type"] is null);
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && l["name"] is null
                && (string)l["type"] == "type-3");
        await Assert.That(links[3]).Satisfies(static (l) =>
            (string)l["url"] == "url-4"
                && (string)l["name"] == "name-4"
                && (string)l["type"] == "type-4");
    }

    [Test]
    public async Task CheckIssuesRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddIssuesAtRuntime);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && (string)l["type"] == "issue");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "issue");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "name-3"
                && (string)l["type"] == "issue");
    }

    [Test]
    public async Task CheckLegacyIssueAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyIssueAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "url-3"
                && (string)l["type"] == "issue");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "issue");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && (string)l["name"] == "url-1"
                && (string)l["type"] == "issue");
    }

    [Test]
    public async Task CheckIssueAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.IssueAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && (string)l["type"] == "issue");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "issue");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && l["name"] is null
                && (string)l["type"] == "issue");
    }

    [Test]
    public async Task CheckTmsLinksRuntimeApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddTmsItemsAtRuntime);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && (string)l["type"] == "tms");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "tms");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "name-3"
                && (string)l["type"] == "tms");
    }

    [Test]
    public async Task CheckLegacyTmsAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyTmsAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && (string)l["name"] == "url-3"
                && (string)l["type"] == "tms");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "tms");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && (string)l["name"] == "url-1"
                && (string)l["type"] == "tms");
    }

    [Test]
    public async Task CheckTmsItemAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TmsItemAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(3);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && (string)l["type"] == "tms");
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "tms");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && l["name"] is null
                && (string)l["type"] == "tms");
    }
}
