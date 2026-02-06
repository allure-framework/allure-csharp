using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

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
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteHierarchySamples))]
    public async Task CheckSuiteLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            static (l) =>
            {
                return (string)l["name"] == "parentSuite" && (string)l["value"] == "foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "suite" && (string)l["value"] == "bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "subSuite" && (string)l["value"] == "baz";
            }
        );
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
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetParentSuiteSamples))]
    public async Task CheckParentSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "parentSuite" && (string)l["value"] == "foo";
            }
        );
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
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteSamples))]
    public async Task CheckSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "suite" && (string)l["value"] == "foo";
            }
        );
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
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSubSuiteSamples))]
    public async Task CheckSubSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "subSuite" && (string)l["value"] == "foo";
            }
        );
    }
}
