using System;
using System.Linq;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies tags.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureTagAttribute(string tag, params string[] moreTags)
    : AllureMetadataAttribute
{
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            testResult.labels.Add(Label.Tag(tag));
        }

        testResult.labels.AddRange(
            moreTags
                .Where(static (v) =>
                    !string.IsNullOrEmpty(v))
                .Select(Label.Tag)
        );
    }
}
