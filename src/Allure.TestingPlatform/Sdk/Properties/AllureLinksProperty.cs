using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds links to an Allure test result.
/// </summary>
public sealed class AllureLinksProperty(IEnumerable<Link> links) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the links to add.
    /// </summary>
    public List<Link> Links { get; } = [..links];

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.links.AddRange(this.Links);
    }
}
