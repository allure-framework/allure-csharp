using System;
using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a description.
/// </summary>
/// <param name="description">A description text. Markdown is supported.</param>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true
)]
public class AllureDescriptionAttribute(string description) : AllureMetadataAttribute
{
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        testResult.description = description;
    }
}
