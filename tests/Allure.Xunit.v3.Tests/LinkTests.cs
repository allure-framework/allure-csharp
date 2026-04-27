using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.v3.Tests;

class LinkTests
{
    [Test]
    public async Task LinkRuntimeApiShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkRuntimeApi);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToList();
        await Assert.That(links.Count).IsEqualTo(7);
        await Assert.That(links[0]).Satisfies(static (l) =>
            (string)l["url"] == "url-1"
                && l["name"] is null
                && l["type"] is null);
        await Assert.That(links[1]).Satisfies(static (l) =>
            (string)l["url"] == "url-2"
                && (string)l["name"] == "name-2"
                && (string)l["type"] == "type-2");
        await Assert.That(links[2]).Satisfies(static (l) =>
            (string)l["url"] == "url-3"
                && l["name"] is null
                && (string)l["type"] == "issue");
        await Assert.That(links[3]).Satisfies(static (l) =>
            (string)l["url"] == "url-4"
                && l["name"] is null
                && (string)l["type"] == "tms");
        await Assert.That(links[4]).Satisfies(static (l) =>
            (string)l["url"] == "url-5"
                && (string)l["name"] == "name-5"
                && l["type"] is null);
        await Assert.That(links[5]).Satisfies(static (l) =>
            (string)l["url"] == "url-6"
                && (string)l["name"] == "name-6"
                && (string)l["type"] == "issue");
        await Assert.That(links[6]).Satisfies(static (l) =>
            (string)l["url"] == "url-7"
                && (string)l["name"] == "name-7"
                && (string)l["type"] == "tms");
    }

    [Test]
    public async Task LinkAttributesShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LinkAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToArray();
        await Assert.That(links.Count).IsEqualTo(12);
        await Assert.That(links)
            .Any(static (l) =>
                (string)l["url"] == "url-1"
                    && l["name"] is null
                    && l["type"] is null)
            .And.Any(static (l) =>
                (string)l["url"] == "url-2"
                    && l["name"] is null
                    && (string)l["type"] == "issue")
            .And.Any(static (l) =>
                (string)l["url"] == "url-3"
                    && l["name"] is null
                    && (string)l["type"] == "tms")
            .And.Any(static (l) =>
                (string)l["url"] == "url-4"
                    && (string)l["name"] == "name-4"
                    && l["type"] is null)
            .And.Any(static (l) =>
                (string)l["url"] == "url-5"
                    && (string)l["name"] == "name-5"
                    && (string)l["type"] == "issue")
            .And.Any(static (l) =>
                (string)l["url"] == "url-6"
                    && (string)l["name"] == "name-6"
                    && (string)l["type"] == "tms")
            .And.Any(static (l) =>
                (string)l["url"] == "url-7"
                    && l["name"] is null
                    && (string)l["type"] == "type-7")
            .And.Any(static (l) =>
                (string)l["url"] == "url-8"
                    && l["name"] is null
                    && (string)l["type"] == "issue")
            .And.Any(static (l) =>
                (string)l["url"] == "url-9"
                    && l["name"] is null
                    && (string)l["type"] == "tms")
            .And.Any(static (l) =>
                (string)l["url"] == "url-10"
                    && (string)l["name"] == "name-10"
                    && (string)l["type"] == "type-10")
            .And.Any(static (l) =>
                (string)l["url"] == "url-11"
                    && l["name"] is null
                    && (string)l["type"] == "issue")
            .And.Any(static (l) =>
                (string)l["url"] == "url-12"
                    && l["name"] is null
                    && (string)l["type"] == "tms");
    }

    [Test]
    public async Task LegacyLinkAttributesShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyLinkAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>().ToArray();
        await Assert.That(links.Count).IsEqualTo(4);
        await Assert.That(links)
            .Any(static (l) =>
                (string)l["url"] == "url-3"
                    && (string)l["name"] == "name-3"
                    && (string)l["type"] == "link")
            .And.Any(static (l) =>
                (string)l["url"] == "url-4"
                    && (string)l["name"] == "name-4"
                    && (string)l["type"] == "issue")
            .And.Any(static (l) =>
                (string)l["url"] == "url-5"
                    && (string)l["name"] == "url-5"
                    && (string)l["type"] == "link")
            .And.Any(static (l) =>
                (string)l["url"] == "url-6"
                    && (string)l["name"] == "url-6"
                    && (string)l["type"] == "issue");
    }
}



