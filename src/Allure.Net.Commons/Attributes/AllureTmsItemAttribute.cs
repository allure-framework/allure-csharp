using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a link to a test management system (TMS) item.
/// </summary>
/// <param name="tmsItemIdOrUrl">
/// The item's ID or URL. If ID is specified, make sure a corresponding link template
/// exists in the configuration.
/// </param>
public class AllureTmsItemAttribute(string tmsItemIdOrUrl) : AllureMetadataAttribute
{
    /// <summary>
    /// A display text of the TMS item link.
    /// </summary>
    public string Title { get; set; }

    /// <inheritdoc/>
    protected internal override void Apply(TestResult testResult)
    {
        testResult.links.Add(new()
        {
            url = tmsItemIdOrUrl,
            name = this.Title,
            type = LinkType.TMS_ITEM,
        });
    }
}
