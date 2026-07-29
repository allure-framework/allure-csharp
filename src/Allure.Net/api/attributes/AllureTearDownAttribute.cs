using System;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure;

/// <summary>
/// Wraps each call of the method in a tear down fixture.
/// </summary>
[Injection(typeof(AllureTearDownAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class AllureTearDownAttribute : AllureOperationAttribute
{
    /// <summary>
    /// Wraps each call of the method in a tear down fixture using the method's
    /// name as the name of the fixture.
    /// </summary>
    public AllureTearDownAttribute() : base(null) { }

    /// <summary>
    /// Wraps each call of the method in a named tear down fixture.
    /// </summary>
    /// <param name="name">
    /// The name of the fixture. Use <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureTearDownAttribute(string name) : base(name) { }
}
