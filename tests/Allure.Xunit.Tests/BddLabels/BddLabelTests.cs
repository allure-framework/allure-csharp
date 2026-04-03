using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests.BddLabels;

class BddLabelTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetBddHierarchySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.BddHierarchyAttributeOnClass,
            AllureSampleRegistry.BddHierarchyAttributeOnMethod,
            AllureSampleRegistry.BddHierarchyAttributeOnBaseClass,
            AllureSampleRegistry.BddHierarchyAttributeOnInterface,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetBddHierarchySamples))]
    public async Task CheckBddLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            static (l) =>
            {
                return (string)l["name"] == "epic" && (string)l["value"] == "foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "feature" && (string)l["value"] == "bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "story" && (string)l["value"] == "baz";
            }
        );
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetEpicSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.EpicAttributeOnClass,
            AllureSampleRegistry.EpicAttributeOnMethod,
            AllureSampleRegistry.EpicAttributeOnBaseClass,
            AllureSampleRegistry.EpicAttributeOnInterface,
            AllureSampleRegistry.AddEpicFromTest,
            AllureSampleRegistry.AddEpicFromDispose,
            AllureSampleRegistry.LegacyEpicAttributeOnClass,
            AllureSampleRegistry.LegacyEpicAttributeOnMethod,
            AllureSampleRegistry.LegacyEpicAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetEpicSamples))]
    public async Task CheckEpicIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "epic" && (string)l["value"] == "foo";
            }
        );
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetFeatureSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.FeatureAttributeOnClass,
            AllureSampleRegistry.FeatureAttributeOnMethod,
            AllureSampleRegistry.FeatureAttributeOnBaseClass,
            AllureSampleRegistry.FeatureAttributeOnInterface,
            AllureSampleRegistry.AddFeatureFromTest,
            AllureSampleRegistry.AddFeatureFromDispose,
            AllureSampleRegistry.LegacyFeatureAttributeOnClass,
            AllureSampleRegistry.LegacyFeatureAttributeOnMethod,
            AllureSampleRegistry.LegacyFeatureAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetFeatureSamples))]
    public async Task CheckFeatureIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "feature" && (string)l["value"] == "foo";
            }
        );
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetStorySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.StoryAttributeOnClass,
            AllureSampleRegistry.StoryAttributeOnMethod,
            AllureSampleRegistry.StoryAttributeOnBaseClass,
            AllureSampleRegistry.StoryAttributeOnInterface,
            AllureSampleRegistry.AddStoryFromTest,
            AllureSampleRegistry.AddStoryFromDispose,
            AllureSampleRegistry.LegacyStoryAttributeOnClass,
            AllureSampleRegistry.LegacyStoryAttributeOnMethod,
            AllureSampleRegistry.LegacyStoryAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetStorySamples))]
    public async Task CheckStoryIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "story" && (string)l["value"] == "foo";
            }
        );
    }
}
