using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds a label to an Allure test result, replacing all labels with the same name.
/// </summary>
/// <param name="name">The label name.</param>
/// <param name="value">The label value.</param>
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
