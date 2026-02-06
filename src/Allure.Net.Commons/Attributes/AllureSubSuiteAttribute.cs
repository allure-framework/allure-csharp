using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>subSuite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureSubSuiteAttribute(string subSuite)
    : AllureLabelAttribute(LabelName.SUB_SUITE, subSuite);
