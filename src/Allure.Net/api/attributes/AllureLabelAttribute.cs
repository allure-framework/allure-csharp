using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a custom label.
/// </summary>
/// <param name="name">A label's name.</param>
/// <param name="value">A label's value.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureLabelAttribute(string name, string value) : AllureApiAttribute
{
    /// <summary>
    /// The name of the label.
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// The value of the label.
    /// </summary>
    public string Value { get; init; } = value;

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (string.IsNullOrEmpty(this.Name) || this.Value is null)
        {
            return;
        }

        testResult.Labels.Add(new() { Name = this.Name, Value = this.Value });
    }
}