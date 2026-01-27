using System.Reflection;
using Allure.Net.Commons.Sdk;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies all the attributes applied to its subclass. Allows reducing boilerplate code.
/// </summary>
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