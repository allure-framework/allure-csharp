using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>parentSuite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureParentSuiteAttribute(string parentSuite)
    : AllureLabelAttribute(LabelName.PARENT_SUITE, parentSuite);
