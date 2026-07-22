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
public sealed class AllureAfterAttribute : AllureOperationAttribute
{
    /// <summary>
    /// Wraps each call of the method in a tear down fixture using the method's
    /// name as the name of the fixture.
    /// </summary>
    public AllureAfterAttribute() : base(null) { }

    /// <summary>
    /// Wraps each call of the method in a named tear down fixture.
    /// </summary>
    /// <param name="name">
    /// The name of the fixture. Use <c>{paramName}</c> placeholders to interpolate the
    /// arguments.
    /// </param>
    public AllureAfterAttribute(string name) : base(name) { }
}
