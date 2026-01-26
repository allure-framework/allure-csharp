using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a link.
/// </summary>
/// <param name="url">
/// A full URL or a portion of it. If a portion of the URL is used, a URL template that fits
/// the link's type must exist in the configuration.
/// </param>
public class AllureLinkAttribute(string url) : AllureMetadataAttribute
{
    /// <summary>
    /// A display text of the link.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A type of the link.
    /// </summary>
    public string Type { get; set; }

    /// <inheritdoc/>
    protected internal override void Apply(TestResult testResult)
    {
        testResult.links.Add(new()
        {
            url = url,
            name = this.Title,
            type = this.Type,
        });
    }
}
