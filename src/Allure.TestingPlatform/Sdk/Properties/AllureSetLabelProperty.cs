using System.Collections.Generic;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds a label to an Allure test result removing all labels with the same name.
/// </summary>
public sealed class AllureSetLabelProperty(string name, string value) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the label name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the label value.
    /// </summary>
    public string Value { get; } = value;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.Labels.RemoveAll((label) => label.Name == this.Name);
        target.Labels.Add(new() { Name = this.Name, Value = this.Value });
    }
}
