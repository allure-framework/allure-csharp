using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class LabelTests
{
    [Test]
    public async Task ClassLevelCustomLabels()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AttributeLabelOnClass);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }

    [Test]
    public async Task MethodLevelCustomLabels()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AttributeLabelOnMethod);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }

    [Test]
    public async Task AddLabelFromTest()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddLabelFromTest);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }

    [Test]
    public async Task AddLabelFromSetUp()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AddLabelFromSetUp);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }
}
