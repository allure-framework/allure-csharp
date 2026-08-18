using System.Collections.Generic;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds labels to an Allure test result.
/// </summary>
/// <param name="labels">The labels to add.</param>
public sealed class AllureLabelsProperty(IEnumerable<Label> labels) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the labels to add.
    /// </summary>
    public List<Label> Labels { get; } = [..labels];

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.Labels.AddRange(this.Labels);
    }
}
