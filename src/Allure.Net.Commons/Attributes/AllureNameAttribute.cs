using System;
using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a display name.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true
)]
public class AllureNameAttribute(string name) : AllureMetadataAttribute
{
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        testResult.name = name;
    }
}
