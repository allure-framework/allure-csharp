using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a custom label.
/// </summary>
/// <param name="name">A label's name.</param>
/// <param name="value">A label's value.</param>
public class AllureLabelAttribute(string name, string value) : AllureMetadataAttribute
{
    /// <inheritdoc/>
    protected internal override void Apply(TestResult testResult)
    {
        testResult.labels.Add(new() { name = name, value = value });
    }
}