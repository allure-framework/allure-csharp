using Allure.Testing;

namespace Allure.NUnit.Tests.BddLabels;

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

        var testResult = await Assert.That(results).HasSingleTestResult();

        await Assert.That(testResult).HasSingleLabel("epic").With.Value("foo");
        await Assert.That(testResult).HasSingleLabel("feature").With.Value("bar");
        await Assert.That(testResult).HasSingleLabel("story").With.Value("baz");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetEpicSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.EpicAttributeOnClass,
            AllureSampleRegistry.EpicAttributeOnMethod,
            AllureSampleRegistry.EpicAttributeOnBaseClass,
            AllureSampleRegistry.EpicAttributeOnInterface,
            AllureSampleRegistry.AddEpicFromSetUp,
            AllureSampleRegistry.AddEpicFromTest,
            AllureSampleRegistry.AddEpicFromTearDown,
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

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("epic")
            .With.Value("foo");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetFeatureSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.FeatureAttributeOnClass,
            AllureSampleRegistry.FeatureAttributeOnMethod,
            AllureSampleRegistry.FeatureAttributeOnBaseClass,
            AllureSampleRegistry.FeatureAttributeOnInterface,
            AllureSampleRegistry.AddFeatureFromSetUp,
            AllureSampleRegistry.AddFeatureFromTest,
            AllureSampleRegistry.AddFeatureFromTearDown,
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

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("feature")
            .With.Value("foo");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetStorySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.StoryAttributeOnClass,
            AllureSampleRegistry.StoryAttributeOnMethod,
            AllureSampleRegistry.StoryAttributeOnBaseClass,
            AllureSampleRegistry.StoryAttributeOnInterface,
            AllureSampleRegistry.AddStoryFromSetUp,
            AllureSampleRegistry.AddStoryFromTest,
            AllureSampleRegistry.AddStoryFromTearDown,
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

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("story")
            .With.Value("foo");
    }
}
