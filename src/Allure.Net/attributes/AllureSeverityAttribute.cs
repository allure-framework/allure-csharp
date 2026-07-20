using System;
using Allure.Model;

namespace Allure;

/// <summary>
/// Applies a <c>severity</c> label.
/// </summary>
[AttributeUsage(ALLURE_METADATA_TARGETS, AllowMultiple = false, Inherited = true)]
public class AllureSeverityAttribute(Severity severity)
    : AllureLabelAttribute(LabelName.Severity, severity.ToString());
