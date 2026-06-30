using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds labels to an Allure test result.
/// </summary>
public sealed class AllureLabelsProperty(IEnumerable<Label> labels) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the labels to add.
    /// </summary>
    public List<Label> Labels { get; } = [..labels];

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.labels.AddRange(this.Labels);
    }
}
