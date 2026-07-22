using System.Reflection;
using Allure.Abstractions;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies all the attributes applied to its subclass, serving as a shortcut for them.
/// </summary>
public abstract class AllureMetaAttribute : AllureApiAttribute
{
    private readonly AllureApiAttribute[] attributes;

    /// <summary>
    /// Initializes the composed metadata attribute.
    /// </summary>
    public AllureMetaAttribute()
    {
        this.attributes = [
            ..this.GetType().GetCustomAttributes<AllureApiAttribute>(true),
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
