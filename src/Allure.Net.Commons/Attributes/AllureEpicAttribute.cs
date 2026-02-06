using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies an <c>epic</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureEpicAttribute(string epic)
    : AllureLabelAttribute(LabelName.EPIC, epic);
