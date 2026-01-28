using System;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a display name to a test or a class.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false
)]
public class AllureNameAttribute(string name) : AllureMetadataAttribute
{
    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (name is not null)
        {
            testResult.name = name;
        }
    }
}
