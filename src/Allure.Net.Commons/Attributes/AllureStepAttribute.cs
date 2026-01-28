using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Wraps each call of the method in an Allure step.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllureStepAttribute : Steps.AllureStepAttributes.AbstractStepAttribute
{
    /// <summary>
    /// Wraps each call of the method in an Allure step using the method's name as the
    /// name of the step.
    /// </summary>
    public AllureStepAttribute() : base() { }

    /// <summary>
    /// Wraps each call of the method in a named Allure step.
    /// </summary>
    /// <param name="name">
    /// A name of the step. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureStepAttribute(string name) : base(name) { }
}
