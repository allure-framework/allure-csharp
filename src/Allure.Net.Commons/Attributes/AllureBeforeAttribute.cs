using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Wraps each call of the method in a setup fixture.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
public class AllureBeforeAttribute : Steps.AllureStepAttributes.AbstractBeforeAttribute
{
    /// <summary>
    /// Wraps each call of the method or constructor in an setup fixture using the method's
    /// name as the name of the fixture.
    /// </summary>
    public AllureBeforeAttribute() : base() { }

    /// <summary>
    /// Wraps each call of the method or constructor in a named setup fixture.
    /// </summary>
    /// <param name="name">
    /// A name of the fixture. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureBeforeAttribute(string name) : base(name) { }
}
