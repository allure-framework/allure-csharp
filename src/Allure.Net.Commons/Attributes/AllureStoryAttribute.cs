using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>story</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = true, Inherited = true)]
public class AllureStoryAttribute(string story)
    : AllureLabelAttribute(LabelName.STORY, story);
