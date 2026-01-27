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
    /// <summary>
    /// Applies the attribute to a test result.
    /// </summary>
    public abstract void Apply(TestResult testResult);
}