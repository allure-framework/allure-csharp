using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Wraps each call of the method in a tear down fixture.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllureAfterAttribute : Steps.AllureStepAttributes.AbstractAfterAttribute
{
    /// <summary>
    /// Wraps each call of the method in a tear down fixture using the method's
    /// name as the name of the fixture.
    /// </summary>
    public AllureAfterAttribute() : base() { }

    /// <summary>
    /// Wraps each call of the method in a named tear down fixture.
    /// </summary>
    /// <param name="name">
    /// A name of the fixture. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureAfterAttribute(string name) : base(name) { }
}
