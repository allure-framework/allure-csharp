using System;
using System.Reflection;
using Allure.Net.Commons.Sdk;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies all the attributes applied to its subclass, serving as a shortcut for them.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public abstract class AllureMetaAttribute : AllureMetadataAttribute
{
    private readonly AllureMetadataAttribute[] attributes;

    public AllureMetaAttribute()
    {
        this.attributes = [
            ..this.GetType().GetCustomAttributes<AllureMetadataAttribute>(true),
        ];
    }

    /// <inheritdoc/>
    sealed public override void Apply(TestResult testResult)
    {
        foreach (var attr in this.attributes)
        {
            attr.Apply(testResult);
        }
    }
}