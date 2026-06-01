using Allure.Testing;

namespace Allure.NUnit.Tests.Owners;

class OwnerTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetOwnerSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.NUnitAuthorPropertyOnTest,
            AllureSampleRegistry.NUnitAuthorPropertyOnTestCase,
            AllureSampleRegistry.NUnitAuthorPropertyOnTestFixture,
            AllureSampleRegistry.OwnerAttributeOnBaseClass,
            AllureSampleRegistry.OwnerAttributeOnClass,
            AllureSampleRegistry.OwnerAttributeOnInterface,
            AllureSampleRegistry.OwnerAttributeOnMethod,
            AllureSampleRegistry.SetOwnerFromSetUp,
            AllureSampleRegistry.SetOwnerFromTearDown,
            AllureSampleRegistry.SetOwnerFromTest,
            AllureSampleRegistry.LegacyOwnerAttributeOnClass,
            AllureSampleRegistry.LegacyOwnerAttributeOnMethod,
            AllureSampleRegistry.LegacyOwnerAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetOwnerSamples))]
    public async Task CheckOwnerIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync2(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.SingleLabel("owner").With.Value("John Doe");
    }
}
