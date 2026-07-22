using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a link to a test management system (TMS) item.
/// </summary>
/// <param name="tmsItemIdOrUrl">
/// The item's ID or URL. If ID is specified, make sure a corresponding link template
/// exists in the configuration.
/// </param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureTmsItemAttribute(string tmsItemIdOrUrl) : AllureApiAttribute
{
    /// <summary>
    /// The ID of the TMS item or its full URL.
    /// </summary>
    public string IdOrUrl { get; init; } = tmsItemIdOrUrl;

    /// <summary>
    /// The display text of the TMS item link.
    /// </summary>
    public string? Title { get; set; }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (this.IdOrUrl is null)
        {
            return;
        }

        testResult.Links.Add(new()
        {
            Url = this.IdOrUrl,
            Name = this.Title,
            Type = LinkType.TmsItem,
        });
    }
}
