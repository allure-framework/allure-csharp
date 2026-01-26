using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a link to an issue.
/// </summary>
/// <param name="issueIdOrUrl">
/// The issue's ID or URL. If ID is specified, make sure a corresponding link template
/// exists in the configuration.
/// </param>
public class AllureIssueAttribute(string issueIdOrUrl) : AllureMetadataAttribute
{
    string Title { get; set; }

    /// <inheritdoc/>
    protected internal override void Apply(TestResult testResult)
    {
        testResult.links.Add(new()
        {
            url = issueIdOrUrl,
            name = this.Title,
            type = LinkType.ISSUE,
        });
    }
}
