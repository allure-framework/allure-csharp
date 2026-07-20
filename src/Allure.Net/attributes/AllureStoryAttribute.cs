using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>story</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureStoryAttribute(string story)
    : AllureLabelAttribute(LabelName.Story, story);
