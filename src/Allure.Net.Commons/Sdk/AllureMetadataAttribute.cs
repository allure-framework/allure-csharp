using System;

namespace Allure.Net.Commons.Sdk;

/// <summary>
/// A base class for attributes that apply metadata to test results.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true
)]
public abstract class AllureMetadataAttribute : Attribute
{
    internal void Apply(AllureContext context)
    {
        if (context.HasTest)
        {
            this.Apply(context.CurrentTest);
        }
    }

    /// <summary>
    /// Applies the metadata denoted by the attribute to a test result.
    /// It's only called if a test is running.
    /// </summary>
    /// <param name="testResult">A test result to apply the metadata to.</param>
    protected internal abstract void Apply(TestResult testResult);
}