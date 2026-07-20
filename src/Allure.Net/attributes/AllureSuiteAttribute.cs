using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>suite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureSuiteAttribute(string suite)
    : AllureLabelAttribute(LabelName.Suite, suite);
