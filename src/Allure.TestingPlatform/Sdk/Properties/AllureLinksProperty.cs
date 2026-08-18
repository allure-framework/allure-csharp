using System.Collections.Generic;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds links to an Allure test result.
/// </summary>
/// <param name="links">The links to add.</param>
public sealed class AllureLinksProperty(IEnumerable<Link> links) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the links to add.
    /// </summary>
    public List<Link> Links { get; } = [..links];

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.Links.AddRange(this.Links);
    }
}
