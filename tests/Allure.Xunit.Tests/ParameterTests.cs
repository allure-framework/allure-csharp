using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests;

class ParameterTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetParameterSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.AddTestParameter,
            AllureSampleRegistry.ParameterAttributesOnTheoryParameters,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetParameterSamples))]
    public async Task ParametersApiWorks(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);

        var parameters = results.TestResults[0]["parameters"].AsArray().Cast<JsonObject>().ToList();

        await Assert.That(parameters.Count).IsEqualTo(5);
        await Assert.That(parameters[0]).Satisfies(
            static (p) => (string)p["name"] == "name1"
                && (string)p["value"] == "\"value-1\""
                && p["mode"] is null
                && (bool)p["excluded"] is false
        );
        await Assert.That(parameters[1]).Satisfies(
            static (p) => (string)p["name"] == "name2"
                && (string)p["value"] == "\"value-2\""
                && (string)p["mode"] == "masked"
                && (bool)p["excluded"] is false
        );
        await Assert.That(parameters[2]).Satisfies(
            static (p) => (string)p["name"] == "name3"
                && (string)p["value"] == "\"value-3\""
                && (string)p["mode"] == "hidden"
                && (bool)p["excluded"] is false
        );
        await Assert.That(parameters[3]).Satisfies(
            static (p) => (string)p["name"] == "name4"
                && (string)p["value"] == "\"value-4\""
                && p["mode"] is null
                && (bool)p["excluded"] is true
        );
        await Assert.That(parameters[4]).Satisfies(
            static (p) => (string)p["name"] == "name5"
                && (string)p["value"] == "\"value-5\""
                && (string)p["mode"] == "masked"
                && (bool)p["excluded"] is true
        );
    }
}