using System;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure;

/// <summary>
/// Wraps each call of the method in an Allure step.
/// </summary>
[Injection(typeof(AllureStepAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class AllureStepAttribute : AllureOperationAttribute
{
    /// <summary>
    /// Wraps each call of the method in an Allure step using the method's name as the
    /// name of the step.
    /// </summary>
    public AllureStepAttribute() : base(null) { }

    /// <summary>
    /// Wraps each call of the method in a named Allure step.
    /// </summary>
    /// <param name="name">
    /// A name of the step. Use the <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureStepAttribute(string name) : base(name) { }
}
