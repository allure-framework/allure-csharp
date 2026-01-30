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
    /// <summary>
    /// The provided name.
    /// </summary>
    public string Name { get; init; } = name;

    /// <inheritdoc/>
    public override void Apply(TestResult testResult)
    {
        if (this.Name is not null)
        {
            testResult.name = this.Name;
        }
    }
}
