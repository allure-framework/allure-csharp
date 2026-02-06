using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>suite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureSuiteAttribute(string suite)
    : AllureLabelAttribute(LabelName.SUITE, suite);
