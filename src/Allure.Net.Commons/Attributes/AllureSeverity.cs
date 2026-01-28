using System;

#nullable enable

namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>severity</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = false, Inherited = true)]
public class AllureSeverity(SeverityLevel severity)
    : AllureLabelAttribute(LabelName.SEVERITY, severity.ToString());
