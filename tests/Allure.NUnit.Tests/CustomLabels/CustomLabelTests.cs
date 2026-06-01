using Allure.Testing;

namespace Allure.NUnit.Tests.CustomLabels;

class CustomLabelTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetCustomLabelSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.LabelAttributeOnClass,
            AllureSampleRegistry.LabelAttributeOnMethod,
            AllureSampleRegistry.LabelAttributeOnBaseClass,
            AllureSampleRegistry.LabelAttributeOnInterface,
            AllureSampleRegistry.AddLabelFromSetUp,
            AllureSampleRegistry.AddLabelFromTest,
            AllureSampleRegistry.AddLabelFromTearDown,
            AllureSampleRegistry.LegacyLabelAttributeOnClass,
            AllureSampleRegistry.LegacyLabelAttributeOnMethod,
            AllureSampleRegistry.LegacyLabelAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetCustomLabelSamples))]
    public async Task CheckCustomLabelIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync2(sample);

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("foo")
            .With.Value("bar");
    }
}
