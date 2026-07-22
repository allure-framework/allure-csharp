using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies an <c>epic</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureEpicAttribute(string epic)
    : AllureLabelAttribute(LabelName.Epic, epic);
