using System;

#nullable enable

namespace Allure.Net.Commons.Sdk;

/// <summary>
/// A base class for attributes that apply metadata to test results.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Method
        | AttributeTargets.Interface,
    AllowMultiple = true,
    Inherited = true
)]
public abstract class AllureMetadataAttribute : Attribute
{
    /// <summary>
    /// Default targets for Allure metadata attributes.
    /// </summary>
    public const AttributeTargets ALLURE_METADATA_TARGETS
        = AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Method
            | AttributeTargets.Interface;

    /// <summary>
    /// Applies the attribute to a test result.
    /// </summary>
    public abstract void Apply(TestResult testResult);
}