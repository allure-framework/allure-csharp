using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>feature</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureFeatureAttribute(string feature)
    : AllureLabelAttribute(LabelName.Feature, feature);
