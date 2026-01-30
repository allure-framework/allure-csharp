using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class DescriptionTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetCommonDescriptionSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.NUnitDescriptionAttributeOnMethod,
            AllureSampleRegistry.NUnitDescriptionAttributeOnClass,
            AllureSampleRegistry.NUnitDescriptionPropertyOnTest,
            AllureSampleRegistry.NUnitDescriptionPropertyOnTestCase,
            AllureSampleRegistry.NUnitDescriptionPropertyOnTestFixture,
            AllureSampleRegistry.DescriptionAttributeOnMethod,
            AllureSampleRegistry.DescriptionAttributeOnClass,
            AllureSampleRegistry.DescriptionAttributeOnBaseClass,
            AllureSampleRegistry.DescriptionAttributeOnInterface,
            AllureSampleRegistry.AddDescriptionFromSetUp,
            AllureSampleRegistry.AddDescriptionFromTest,
            AllureSampleRegistry.AddDescriptionFromTearDown,
            AllureSampleRegistry.LegacyDescriptionAttribute,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetCommonDescriptionSamples))]
    public async Task CheckDescriptionIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["description"]).IsEqualTo("Lorem Ipsum");
    }

    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetHtmlDescriptionSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.DescriptionHtmlAttributeOnMethod,
            AllureSampleRegistry.DescriptionHtmlAttributeOnClass,
            AllureSampleRegistry.DescriptionHtmlAttributeOnBaseClass,
            AllureSampleRegistry.DescriptionHtmlAttributeOnInterface,
            AllureSampleRegistry.AddDescriptionHtmlFromSetUp,
            AllureSampleRegistry.AddDescriptionHtmlFromTest,
            AllureSampleRegistry.AddDescriptionHtmlFromTearDown,
            AllureSampleRegistry.LegacyDescriptionAttributeHtml,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetHtmlDescriptionSamples))]
    public async Task CheckHtmkDescriptionIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["descriptionHtml"]).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task NUnitDescriptionAttributesCompose()
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.NUnitDescriptionAttributeComposition
        );

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert
            .That((string)results.TestResults[0]["description"])
            .IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet");
    }

    [Test]
    public async Task NUnitDescriptionPropertiesCompose()
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.NUnitDescriptionPropertyComposition
        );

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert
            .That((string)results.TestResults[0]["description"])
            .IsEqualTo("Lorem Ipsum\n\nDolor Sit Amet\n\nConsectetur Adipiscing Elit");
    }

    [Test]
    public async Task NUnitDescriptionIgnoredIfDescriptionAlreadyProvided()
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.NUnitDescriptionPropertyWithAllureDescription
        );

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert
            .That((string)results.TestResults[0]["description"])
            .IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task NUnitDescriptionIgnoredIfDescriptionHtmlAlreadyProvided()
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.NUnitDescriptionPropertyWithAllureDescriptionHtml
        );

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert
            .That((string)results.TestResults[0]["description"])
            .IsNull();
    }
}
