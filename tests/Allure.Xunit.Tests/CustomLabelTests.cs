using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests;

class CustomLabelTests
{
    [Test]
    public async Task CheckCustomLabelIsAdded()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddLabelApi);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes)
            .Any(static (l) =>
                (string)l["name"] == "test"
                    && (string)l["value"] == "foo")
            .And.Any(static (l) =>
                (string)l["name"] == "dispose"
                && (string)l["value"] == "bar");
    }

    [Test]
    public async Task LabelAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LabelAttribute);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var labels = results.TestResults[0]["labels"].AsArray().ToArray();
        await Assert.That(labels)
            .Any(static (l) =>
                (string)l["name"] == "interface"
                    && (string)l["value"] == "foo")
            .And.Any(static (l) =>
                (string)l["name"] == "baseClass"
                    && (string)l["value"] == "bar")
            .And.Any(static (l) =>
                (string)l["name"] == "class"
                    && (string)l["value"] == "baz")
            .And.Any(static (l) =>
                (string)l["name"] == "method"
                    && (string)l["value"] == "qux");
    }

    [Test]
    public async Task LegacyLabelAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyLabelAttribute);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var labels = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(labels)
            .Any(static (l) =>
                (string)l["name"] == "class"
                    && (string)l["value"] == "bar")
            .And.Any(static (l) =>
                (string)l["name"] == "method"
                    && (string)l["value"] == "baz");
    }
}
