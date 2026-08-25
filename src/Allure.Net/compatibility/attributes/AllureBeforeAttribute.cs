using System;
using System.ComponentModel;
using Allure.Abstractions;
using Allure.Aspects;
using AspectInjector.Broker;

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// This class is a part of the legacy API compatibility layer and will be
/// removed in a future update.
/// Please, switch to <see cref="AllureSetUpAttribute"/>.
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
