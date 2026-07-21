using Allure.Abstractions;
using Allure.Model;
using ModelTestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Metadata;

public class MetadataAttributeTests
{
    [Test]
    public async Task LabelAttributesApplyExpectedLabels()
    {
        var result = CreateResult();
        AllureApiAttribute.ApplyAttributes(
            [
                new AllureLabelAttribute("custom", "value"),
                new AllureIdAttribute(42),
                new AllureEpicAttribute("epic"),
                new AllureFeatureAttribute("feature"),
                new AllureStoryAttribute("story"),
                new AllureOwnerAttribute("owner"),
                new AllureParentSuiteAttribute("parent"),
                new AllureSuiteAttribute("suite"),
                new AllureSubSuiteAttribute("sub-suite"),
                new AllureSeverityAttribute(Severity.Critical),
            ],
            result
        );

        await Assert.That(result.Labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                [
                    ("custom", "value"),
                    (LabelName.AllureId, "42"),
                    (LabelName.Epic, "epic"),
                    (LabelName.Feature, "feature"),
                    (LabelName.Story, "story"),
                    (LabelName.Owner, "owner"),
                    (LabelName.ParentSuite, "parent"),
                    (LabelName.Suite, "suite"),
                    (LabelName.SubSuite, "sub-suite"),
                    (LabelName.Severity, "critical"),
                ]
            );
    }

    [Test]
    public async Task LabelAndTagAttributesIgnoreInvalidValues()
    {
        var result = CreateResult();

        new AllureLabelAttribute("", "value").Apply(result);
        new AllureLabelAttribute("name", null!).Apply(result);
        new AllureTagAttribute("first", "", null!, "second").Apply(result);

        await Assert.That(result.Labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                [
                    (LabelName.Tag, "first"),
                    (LabelName.Tag, "second"),
                ]
            );
    }

    [Test]
    public async Task LinkAttributesApplyExpectedLinks()
    {
        var result = CreateResult();

        new AllureLinkAttribute("https://example.test/reference")
        {
            Title = "reference",
            Type = "documentation",
        }.Apply(result);
        new AllureIssueAttribute("ISSUE-1") { Title = "issue" }.Apply(result);
        new AllureTmsItemAttribute("CASE-1") { Title = "case" }.Apply(result);

        await Assert.That(result.Links.Select(link => (link.Url, link.Name!, link.Type!)))
            .IsEquivalentTo(
                [
                    ("https://example.test/reference", "reference", "documentation"),
                    ("ISSUE-1", "issue", LinkType.Issue),
                    ("CASE-1", "case", LinkType.TmsItem),
                ]
            );
    }

    [Test]
    public async Task LinkAttributesIgnoreNullUrls()
    {
        var result = CreateResult();

        new AllureLinkAttribute(null!).Apply(result);
        new AllureIssueAttribute(null!).Apply(result);
        new AllureTmsItemAttribute(null!).Apply(result);

        await Assert.That(result.Links).IsEmpty();
    }

    [Test]
    public async Task DescriptionAttributesOverwriteAndAppendUsingTheirFormats()
    {
        var result = CreateResult();

        new AllureDescriptionAttribute("first").Apply(result);
        new AllureDescriptionAttribute("second") { Append = true }.Apply(result);
        new AllureDescriptionHtmlAttribute("<p>first</p>").Apply(result);
        new AllureDescriptionHtmlAttribute("<p>second</p>") { Append = true }.Apply(result);

        await Assert.That(result.Description).IsEqualTo("first\n\nsecond");
        await Assert.That(result.DescriptionHtml)
            .IsEqualTo("<p>first</p><p>second</p>");
    }

    [Test]
    public async Task NullDescriptionsDoNotChangeExistingValues()
    {
        var result = CreateResult();
        result.Description = "markdown";
        result.DescriptionHtml = "<p>html</p>";

        new AllureDescriptionAttribute(null!).Apply(result);
        new AllureDescriptionHtmlAttribute(null!).Apply(result);

        await Assert.That(result.Description).IsEqualTo("markdown");
        await Assert.That(result.DescriptionHtml).IsEqualTo("<p>html</p>");
    }

    [Test]
    public async Task HierarchyAttributesApplyOnlyDefinedLevels()
    {
        var result = CreateResult();

        new AllureBddHierarchyAttribute("feature")
        {
            Epic = "epic",
            Story = null,
        }.Apply(result);
        new AllureSuiteHierarchyAttribute("suite")
        {
            ParentSuite = null,
            SubSuite = "sub-suite",
        }.Apply(result);

        await Assert.That(result.Labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                [
                    (LabelName.Epic, "epic"),
                    (LabelName.Feature, "feature"),
                    (LabelName.Suite, "suite"),
                    (LabelName.SubSuite, "sub-suite"),
                ]
            );
    }

    [Test]
    public async Task HierarchyConstructorsPopulateTheirDeclaredLevels()
    {
        var result = CreateResult();

        new AllureBddHierarchyAttribute("epic", "feature", "story").Apply(result);
        new AllureBddHierarchyAttribute("other epic", "other feature").Apply(result);
        new AllureSuiteHierarchyAttribute("parent", "suite", "sub-suite").Apply(result);
        new AllureSuiteHierarchyAttribute("other parent", "other suite").Apply(result);

        await Assert.That(result.Labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                new[]
                {
                    (LabelName.Epic, "epic"),
                    (LabelName.Feature, "feature"),
                    (LabelName.Story, "story"),
                    (LabelName.Epic, "other epic"),
                    (LabelName.Feature, "other feature"),
                    (LabelName.ParentSuite, "parent"),
                    (LabelName.Suite, "suite"),
                    (LabelName.SubSuite, "sub-suite"),
                    (LabelName.ParentSuite, "other parent"),
                    (LabelName.Suite, "other suite"),
                }
            );
    }

    [Test]
    public async Task NameAttributeUpdatesAValidNameAndIgnoresNull()
    {
        var result = CreateResult();

        new AllureNameAttribute("updated").Apply(result);
        new AllureNameAttribute(null!).Apply(result);

        await Assert.That(result.Name).IsEqualTo("updated");
    }

    [Test]
    public async Task MetaAttributeAppliesItsComponentAttributes()
    {
        var result = CreateResult();

        new ProductMetadataAttribute().Apply(result);

        await Assert.That(result.Labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                [
                    (LabelName.Owner, "team"),
                    (LabelName.Tag, "component"),
                ]
            );
    }

    static ModelTestResult CreateResult() => new() { Uuid = "test-id", Name = "test" };

    [AllureOwner("team")]
    [AllureTag("component")]
    sealed class ProductMetadataAttribute : AllureMetaAttribute;
}
