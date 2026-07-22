using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a link.
/// </summary>
/// <param name="url">
/// A full URL or a portion of it. If a portion of the URL is used, a URL template that fits
/// the link's type must exist in the configuration.
/// </param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureLinkAttribute(string url) : AllureApiAttribute
{
    /// <summary>
    /// The URL of the link.
    /// </summary>
    public string Url { get; init; } = url;

    /// <summary>
    /// The display text of the link.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The type of the link. Use this property to select the correct link template from the
    /// configuration.
    /// </summary>
    public string? Type { get; init; }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (this.Url is null)
        {
            return;
        }

        testResult.Links.Add(new()
        {
            Url = this.Url,
            Name = this.Title,
            Type = this.Type,
        });
    }
}
