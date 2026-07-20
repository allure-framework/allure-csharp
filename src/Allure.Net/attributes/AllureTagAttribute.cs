using System;
using System.Collections.Immutable;
using System.Linq;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies tags.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureTagAttribute(string tag, params string[] moreTags)
    : AllureApiAttribute
{
    /// <summary>
    /// The provided tags.
    /// </summary>
    public ImmutableArray<string> Tags { get; init; } = [tag, .. moreTags];

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        testResult.Labels.AddRange(
            this.Tags
                .Where(static (v) =>
                    !string.IsNullOrEmpty(v))
                .Select(Label.Tag)
        );
    }
}
