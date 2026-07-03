using Allure.Testing;
using Allure.Testing.Execution;

namespace Allure.NUnit.Tests.SuiteLabels;

class SuiteLabelTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSuiteHierarchySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SuiteHierarchyAttributeOnClass,
            AllureSampleRegistry.SuiteHierarchyAttributeOnMethod,
            AllureSampleRegistry.SuiteHierarchyAttributeOnBaseClass,
            AllureSampleRegistry.SuiteHierarchyAttributeOnInterface,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteHierarchySamples))]
    public async Task CheckSuiteLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.OnlyOneLabel(l => l.HasName("parentSuite").And.HasValue("foo"))
            .With.OnlyOneLabel(l => l.HasName("suite").And.HasValue("bar"))
            .With.OnlyOneLabel(l => l.HasName("subSuite").And.HasValue("baz"));
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetParentSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.ParentSuiteAttributeOnClass,
            AllureSampleRegistry.ParentSuiteAttributeOnMethod,
            AllureSampleRegistry.ParentSuiteAttributeOnBaseClass,
            AllureSampleRegistry.ParentSuiteAttributeOnInterface,
            AllureSampleRegistry.AddParentSuiteFromSetUp,
            AllureSampleRegistry.AddParentSuiteFromTest,
            AllureSampleRegistry.AddParentSuiteFromTearDown,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnClass,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnMethod,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetParentSuiteSamples))]
    public async Task CheckParentSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.SingleLabel("parentSuite").With.Value("foo");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SuiteAttributeOnClass,
            AllureSampleRegistry.SuiteAttributeOnMethod,
            AllureSampleRegistry.SuiteAttributeOnBaseClass,
            AllureSampleRegistry.SuiteAttributeOnInterface,
            AllureSampleRegistry.AddSuiteFromSetUp,
            AllureSampleRegistry.AddSuiteFromTest,
            AllureSampleRegistry.AddSuiteFromTearDown,
            AllureSampleRegistry.LegacySuiteAttributeOnClass,
            AllureSampleRegistry.LegacySuiteAttributeOnMethod,
            AllureSampleRegistry.LegacySuiteAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteSamples))]
    public async Task CheckSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.SingleLabel("suite").With.Value("foo");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSubSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SubSuiteAttributeOnClass,
            AllureSampleRegistry.SubSuiteAttributeOnMethod,
            AllureSampleRegistry.SubSuiteAttributeOnBaseClass,
            AllureSampleRegistry.SubSuiteAttributeOnInterface,
            AllureSampleRegistry.AddSubSuiteFromSetUp,
            AllureSampleRegistry.AddSubSuiteFromTest,
            AllureSampleRegistry.AddSubSuiteFromTearDown,
            AllureSampleRegistry.LegacySubSuiteAttributeOnClass,
            AllureSampleRegistry.LegacySubSuiteAttributeOnMethod,
            AllureSampleRegistry.LegacySubSuiteAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetSubSuiteSamples))]
    public async Task CheckSubSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.SingleLabel("subSuite").With.Value("foo");
    }
}
