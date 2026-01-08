using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class LabelTests
{
    [Test]
    public async Task AttributeLabelPassedToResult()
    {
        var runResult = await SampleRunner.RunAsync(AllureSampleRegistry.AttributeLabel);

        await Assert.That(runResult.ExitCode).IsZero();
        var nodes = runResult.AllureResults.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }
}
