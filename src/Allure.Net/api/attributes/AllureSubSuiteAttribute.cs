using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>subSuite</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureSubSuiteAttribute(string subSuite)
    : AllureLabelAttribute(LabelName.SubSuite, subSuite);
