using System;
using System.Globalization;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Sets an Allure ID. Can only be applied to methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllureIdAttribute(int id)
    : AllureLabelAttribute(
        LabelName.ALLURE_ID,
        Convert.ToString(id, CultureInfo.InvariantCulture));
