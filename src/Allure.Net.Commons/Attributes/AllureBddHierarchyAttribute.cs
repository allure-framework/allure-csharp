using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies the whole BDD hierarchy at once.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = false, Inherited = true)]
public class AllureBddHierarchyAttribute : AllureMetadataAttribute
{
    /// <summary>
    /// Value for the <c>epic</c> label.
    /// </summary>
    public string? Epic { get; init; }

    /// <summary>
    /// Value for the <c>feature</c> label.
    /// </summary>
    public string? Feature { get; init; }

    /// <summary>
    /// Value for the <c>story</c> label.
    /// </summary>
    public string? Story { get; init; }

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