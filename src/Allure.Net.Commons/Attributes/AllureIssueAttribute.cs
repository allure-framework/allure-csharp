using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a link to an issue.
/// </summary>
/// <param name="issueIdOrUrl">
/// The issue's ID or URL. If ID is specified, make sure a corresponding link template
/// exists in the configuration.
/// </param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureIssueAttribute(string issueIdOrUrl) : AllureApiAttribute
{
    /// <summary>
    /// The ID of the issue or its full URL.
    /// </summary>
    public string IdOrUrl { get; init; } = issueIdOrUrl;

    /// <summary>
    /// A display text of the issue link.
    /// </summary>
    public string? Title { get; set; }

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (this.IdOrUrl is null)
        {
            return;
        }

        testResult.links.Add(new()
        {
            url = this.IdOrUrl,
            name = this.Title,
            type = LinkType.ISSUE,
        });
    }
}
