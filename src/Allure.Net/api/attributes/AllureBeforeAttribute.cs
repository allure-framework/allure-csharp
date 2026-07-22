using System;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure;

/// <summary>
/// Wraps each call of the method in a setup fixture.
/// </summary>
[Injection(typeof(AllureSetUpAspect))]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class AllureBeforeAttribute : AllureOperationAttribute
{
    /// <summary>
    /// Wraps each call of the method or constructor in an setup fixture using the method's
    /// name as the name of the fixture.
    /// </summary>
    public AllureBeforeAttribute() : base(null) { }

    /// <summary>
    /// Wraps each call of the method or constructor in a named setup fixture.
    /// </summary>
    /// <param name="name">
    /// The name of the fixture. Use <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureBeforeAttribute(string name) : base(name) { }
}
