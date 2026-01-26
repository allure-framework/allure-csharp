using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class ParameterTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSingleParameterSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.AddParameterFromSetUp,
            AllureSampleRegistry.AddParameterFromTest,
            AllureSampleRegistry.AddParameterFromTearDown,
            AllureSampleRegistry.OneNUnitTestCaseWithOneParameter,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSingleParameterSamples))]
    public async Task CheckParameterIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["parameters"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).ContainsOnly(
            p => (string)p["name"] == "foo" && (string)p["value"] == "\"bar\""
        );
    }

    [Test]
    public async Task CheckMaskedParameter()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.MaskedParameter);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);

        var parameters = results.TestResults[0]["parameters"].AsArray().Cast<JsonObject>();
        await Assert.That(parameters).HasSingleItem().And.All(
            static (p) => (string)p["mode"] == "masked"
        );
    }

    [Test]
    public async Task CheckHiddenParameter()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.HiddenParameter);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);

        var parameters = results.TestResults[0]["parameters"].AsArray().Cast<JsonObject>();
        await Assert.That(parameters).HasSingleItem().And.All(
            static (p) => (string)p["mode"] == "hidden"
        );
    }

    [Test]
    public async Task CheckExcludedParameter()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.ExcludedParameter);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(2);

        var testResult1 = results.TestResults[0];
        var testResult2 = results.TestResults[1];

        var testResult1Parameters = testResult1["parameters"].AsArray().Cast<JsonObject>();
        var testResult2Parameters = testResult2["parameters"].AsArray().Cast<JsonObject>();
        await Assert.That(testResult1Parameters).Any(
            static (p) => (string)p["name"] == "timestamp" && (bool)p["excluded"] == true
        );
        await Assert.That(testResult2Parameters).Any(
            static (p) => (string)p["name"] == "timestamp" && (bool)p["excluded"] == true
        );

        // It must not affect historyId
        await Assert.That((string)testResult1["historyId"]).IsEqualTo((string)testResult2["historyId"]);
    }
}