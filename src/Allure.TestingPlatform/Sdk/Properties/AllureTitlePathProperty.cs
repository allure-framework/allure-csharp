using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the title path of an Allure test result.
/// </summary>
public sealed class AllureTitlePathProperty(IEnumerable<string> titlePath) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the title path to set.
    /// </summary>
    public List<string> TitlePath { get; } = [..titlePath];

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.titlePath = [..this.TitlePath];
    }
}
