using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>parentSuite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureParentSuiteAttribute(string parentSuite)
    : AllureLabelAttribute(LabelName.ParentSuite, parentSuite);
