using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class StepTests
{
    [Test]
    public async Task CheckStepsFromAnnotatedMethodCalls()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.DefaultStepAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var steps = results.TestResults[0]["steps"].AsArray().Cast<JsonObject>().ToArray();
        await Assert.That(steps).Count().IsEqualTo(1);

        var step1 = steps[0];
        await Assert.That(step1).Satisfies(
            static (step) => (string)step["name"] == "Foo"
                && (string)step["status"] == "passed"
                && (string)step["stage"] == "finished"
                && step["steps"].AsArray().Count == 0
                && step["parameters"].AsArray().Count == 0
        );
    }
}
