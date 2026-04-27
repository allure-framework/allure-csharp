using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.v3.Tests;

class TagTests
{
    [Test]
    public async Task AddTagsApiWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddTagsApiCalls);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var tags = results.TestResults[0]["labels"]
            .AsArray()
            .Cast<JsonObject>()
            .Where(static (l) => (string)l["name"] == "tag")
            .Select(static (l) => (string)l["value"])
            .ToArray();
        await Assert.That(tags).IsEquivalentTo(
            ["foo", "bar", "baz"],
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task TagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.TagAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var tags = results.TestResults[0]["labels"]
            .AsArray()
            .Cast<JsonObject>()
            .Where(static (l) => (string)l["name"] == "tag")
            .Select(static (l) => (string)l["value"]);
        await Assert.That(tags).IsEquivalentTo(
            ["foo", "bar", "baz", "qux", "qut"],
            TUnit.Assertions.Enums.CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task LegacyTagAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyTagAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var tags = results.TestResults[0]["labels"]
            .AsArray()
            .Cast<JsonObject>()
            .Where(static (l) => (string)l["name"] == "tag")
            .Select(static (l) => (string)l["value"]);
        await Assert.That(tags).IsEquivalentTo(["bar", "baz", "qux"]);
    }
}



