using System;
using System.Globalization;
using Allure.Model;

namespace Allure;

/// <summary>
/// Sets an Allure ID. Can only be applied to methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class AllureIdAttribute(int id)
    : AllureLabelAttribute(
        LabelName.AllureId,
        Convert.ToString(id, CultureInfo.InvariantCulture));
