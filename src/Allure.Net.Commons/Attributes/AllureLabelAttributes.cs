namespace Allure.Net.Commons.Attributes;

/// <summary>
/// Applies a <c>parentSuite</c> label.
/// </summary>
public class AllureParentSuiteAttribute(string parentSuite)
    : AllureLabelAttribute(LabelName.PARENT_SUITE, parentSuite);

/// <summary>
/// Applies a <c>suite</c> label.
/// </summary>
public class AllureSuiteAttribute(string suite)
    : AllureLabelAttribute(LabelName.SUITE, suite);

/// <summary>
/// Applies a <c>subSuite</c> label.
/// </summary>
public class AllureSubSuiteAttribute(string subSuite)
    : AllureLabelAttribute(LabelName.SUB_SUITE, subSuite);

/// <summary>
/// Applies an <c>epic</c> label.
/// </summary>
public class AllureEpicAttribute(string epic)
    : AllureLabelAttribute(LabelName.EPIC, epic);

/// <summary>
/// Applies a <c>feature</c> label.
/// </summary>
public class AllureFeatureAttribute(string feature)
    : AllureLabelAttribute(LabelName.FEATURE, feature);

/// <summary>
/// Applies a <c>story</c> label.
/// </summary>
public class AllureStoryAttribute(string story)
    : AllureLabelAttribute(LabelName.STORY, story);

/// <summary>
/// Applies an Allure ID.
/// </summary>
public class AllureId(string id)
    : AllureLabelAttribute(LabelName.ALLURE_ID, id);

/// <summary>
/// Applies an <c>owner</c> label.
/// </summary>
public class AllureOwner(string owner)
    : AllureLabelAttribute(LabelName.OWNER, owner);

/// <summary>
/// Applies a <c>severity</c> label.
/// </summary>
public class AllureSeverity(SeverityLevel severity)
    : AllureLabelAttribute(LabelName.SEVERITY, severity.ToString());
