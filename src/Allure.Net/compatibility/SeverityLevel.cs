using System;
using System.ComponentModel;

namespace Allure.Net.Commons;

/// <summary>
/// This is a part of the legacy API that will be removed in a future update. Please, switch to <see cref="Model.Severity"/>
/// </summary>
[Obsolete("Use Allure.Model.Severity instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public enum SeverityLevel
{
    normal,
    blocker,
    critical,
    minor,
    trivial
}