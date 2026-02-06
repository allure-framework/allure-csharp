using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies an <c>owner</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = false, Inherited = true)]
public class AllureOwnerAttribute(string owner)
    : AllureLabelAttribute(LabelName.OWNER, owner);
