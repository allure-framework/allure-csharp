using System.Collections.Generic;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the title path of an Allure test result.
/// </summary>
/// <param name="titlePath">The title path to set.</param>
public sealed class AllureTitlePathProperty(IEnumerable<string> titlePath) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the title path to set.
    /// </summary>
    public List<string> TitlePath { get; } = [..titlePath];

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.TitlePath.Clear();
        target.TitlePath.AddRange([.. this.TitlePath]);
    }
}
