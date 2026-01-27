using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies the whole BDD hierarchy at once.
/// </summary>
public class AllureBddHierarchyAttribute : AllureMetadataAttribute
{
    string? Epic { get; init; }
    string? Feature { get; init; }
    string? Story { get; init; }

    /// <summary>
    /// A shorthand for <see cref="AllureEpicAttribute"/>,
    /// <see cref="AllureFeatureAttribute"/>, and <see cref="AllureStoryAttribute"/>.
    /// </summary>
    public AllureBddHierarchyAttribute(string epic, string feature, string story)
    {
        this.Epic = epic;
        this.Feature = feature;
        this.Story = story;
    }

    /// <summary>
    /// A shorthand for <see cref="AllureEpicAttribute"/> and
    /// <see cref="AllureFeatureAttribute"/>.
    /// </summary>
    public AllureBddHierarchyAttribute(string epic, string feature)
    {
        this.Epic = epic;
        this.Feature = feature;
    }

    /// <summary>
    /// An alias for <see cref="AllureFeatureAttribute"/>.
    /// </summary>
    public AllureBddHierarchyAttribute(string feature)
    {
        this.Feature = feature;
    }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        var labels = testResult.labels;

        if (this.Epic is not null)
        {
            labels.Add(Label.Epic(this.Epic));
        }

        if (this.Feature is not null)
        {
            labels.Add(Label.Feature(this.Feature));
        }

        if (this.Story is not null)
        {
            labels.Add(Label.Story(this.Story));
        }
    }
}