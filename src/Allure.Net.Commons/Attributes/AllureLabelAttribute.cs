using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a custom label.
/// </summary>
/// <param name="name">A label's name.</param>
/// <param name="value">A label's value.</param>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureLabelAttribute(string name, string value) : AllureMetadataAttribute
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

        testResult.labels.Add(new() { name = this.Name, value = this.Value });
    }
}