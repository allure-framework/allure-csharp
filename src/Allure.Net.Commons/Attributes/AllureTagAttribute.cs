using System;
using System.Collections.Immutable;
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
    /// <summary>
    /// The provided tags.
    /// </summary>
    public ImmutableArray<string> Tags { get; init; } = CreateTagArray(tag, moreTags);

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        testResult.labels.AddRange(
            this.Tags
                .Where(static (v) =>
                    !string.IsNullOrEmpty(v))
                .Select(Label.Tag)
        );
    }

    static ImmutableArray<string> CreateTagArray(string tag, string[] moreTags)
    {
        var builder = ImmutableArray.CreateBuilder<string>(moreTags.Length + 1);
        builder.Add(tag);
        builder.AddRange(moreTags);
        return builder.MoveToImmutable();
    }
}
