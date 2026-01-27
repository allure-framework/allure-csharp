using System.Linq;
using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies tags.
/// </summary>
public class AllureTagAttribute(string tag, params string[] moreTags)
    : AllureMetadataAttribute
{
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        testResult.labels.Add(Label.Tag(tag));
        testResult.labels.AddRange(moreTags.Select(Label.Tag));
    }
}
