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
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (string.IsNullOrEmpty(name) || value is null)
        {
            return;
        }

        testResult.labels.Add(new() { name = name, value = value });
    }
}