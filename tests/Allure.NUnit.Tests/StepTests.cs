using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class StepTests
{
    record class ParameterExpectations(string Name, string Value)
    {
        public bool Check(JsonObject parameter)
            => (string)parameter["name"] == this.Name
                && (string)parameter["value"] == this.Value;
        public static bool CheckAll(
            List<ParameterExpectations> expectations,
            JsonArray parameters
        )
            => parameters.Count == expectations.Count
                && parameters
                    .Zip(expectations)
                    .All(static (p) => p.Second.Check(p.First.AsObject()));
    }

    record class StepExpectations(
        string Name,
        string Status,
        List<ParameterExpectations> Parameters,
        List<StepExpectations> Substeps
    )
    {
        public bool Check(JsonObject step)
            => (string)step["name"] == this.Name
                && (string)step["status"] == this.Status
                && (string)step["stage"] == "finished"
                && step["parameters"].AsArray().Count == this.Parameters.Count
                && ParameterExpectations.CheckAll(this.Parameters, step["parameters"].AsArray())
                && step["steps"]
                    .AsArray()
                    .Select(static (n) => n.AsObject())
                    .All(this.Check);

        public static bool CheckAll(
            List<StepExpectations> expectations,
            JsonArray steps
        )
            => steps.Count == expectations.Count
                && steps
                    .Zip(expectations)
                    .All(static (p) => p.Second.Check(p.First.AsObject()));
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetStepSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.StepAttributes,
            AllureSampleRegistry.LegacyStepAttributes,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetStepSamples))]
    public async Task CheckStepsFromAnnotatedMethodCalls(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyStepAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var steps = results.TestResults[0]["steps"].AsArray().Cast<JsonObject>().ToArray();
        await Assert.That(steps).Count().IsEqualTo(8);

        await Assert.That(steps[0]).Satisfies(static (step) =>
            new StepExpectations("Void", "passed", [], []).Check(step));

        await Assert.That(steps[1]).Satisfies(static (step) =>
            new StepExpectations("Return", "passed", [], []).Check(step));

        await Assert.That(steps[2]).Satisfies(static (step) =>
            new StepExpectations("Async", "passed", [], []).Check(step));

        await Assert.That(steps[3]).Satisfies(static (step) =>
            new StepExpectations("AsyncReturn", "passed", [], []).Check(step));

        await Assert.That(steps[4]).Satisfies(static (step) =>
            new StepExpectations("Renamed", "passed", [], []).Check(step));

        await Assert.That(steps[5]).Satisfies(static (step) =>
            new StepExpectations(
                "Parameters",
                "passed",
                [new("foo", "1"), new("bar", "\"baz\"")],
                []).Check(step));

        await Assert.That(steps[6]).Satisfies(static (step) =>
            new StepExpectations("SkippedParameter", "passed", [], []).Check(step));

        await Assert.That(steps[7]).Satisfies(static (step) =>
            new StepExpectations("RenamedParameter", "passed", [new("Bar", "3")], []).Check(step));
    }
}
