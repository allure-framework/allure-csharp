using System;
using System.ComponentModel;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// This is a part of the legacy API. Please, switch to <see cref="AllureTearDownAttribute"/>.
/// </summary>
[Obsolete("Use Allure.AllureTearDownAttribute instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
[Injection(typeof(AllureTearDownAspect))]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AllureAfterAttribute : AllureOperationAttribute
{
    public AllureAfterAttribute() : base() { }

    public AllureAfterAttribute(string name) : base(name) { }
}
