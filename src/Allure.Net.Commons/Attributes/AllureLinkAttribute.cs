using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a link.
/// </summary>
/// <param name="url">
/// A full URL or a portion of it. If a portion of the URL is used, a URL template that fits
/// the link's type must exist in the configuration.
/// </param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureLinkAttribute(string url) : AllureMetadataAttribute
{
    /// <summary>
    /// A display text of the link.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// A type of the link.
    /// </summary>
    public string? Type { get; set; }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (url is null)
        {
            return;
        }

        testResult.links.Add(new()
        {
            url = url,
            name = this.Title,
            type = this.Type,
        });
    }
}
