using System;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a display name to a test or a class.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false
)]
public class AllureNameAttribute(string name) : AllureApiAttribute, IAllureNameSource
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
            testResult.Name = this.Name;
        }
    }
}
