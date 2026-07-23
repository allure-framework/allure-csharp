using System;
using System.ComponentModel;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// This is a part of the legacy API. Please, switch to <see cref="AllureSetUpAttribute"/>.
/// </summary>
[Obsolete("Use Allure.AllureSetUpAttribute instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
[Injection(typeof(AllureSetUpAspect))]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
public class AllureBeforeAttribute : AllureOperationAttribute
{
    public AllureBeforeAttribute() : base() { }

    public AllureBeforeAttribute(string name) : base(name) { }
}
