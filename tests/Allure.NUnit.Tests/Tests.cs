using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class LabelTests
{
    [Test]
    public async Task AttributeLabelPassedToResult()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AttributeLabel);

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
