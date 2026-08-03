using System.Linq;
using Allure.Model;

namespace Allure.Sdk.Functions;

/// <summary>
/// Applies default suite-hierarchy labels to test results.
/// </summary>
public static class SuiteLabels
{
    /// <summary>
    /// Adds the provided default suite-hierarchy labels unless the test result
    /// already contains a <c>parentSuite</c>, <c>suite</c>, or
    /// <c>subSuite</c> label.
    /// </summary>
    /// <param name="testResult">The test result to modify.</param>
    /// <param name="parentSuite">
    /// A value for the <c>parentSuite</c> label. If null or empty, the label
    /// is not added.
    /// </param>
    /// <param name="suite">
    /// A value for the <c>suite</c> label. If null or empty, the label is not
    /// added.
    /// </param>
    /// <param name="subSuite">
    /// A value for the <c>subSuite</c> label. If null or empty, the label is not
    /// added.
    /// </param>
    public static void Ensure(
        TestResult testResult,
        string? parentSuite,
        string? suite,
        string? subSuite
    )
    {
        var labels = testResult.Labels;
        if (labels.Any(IsSuiteLabel))
        {
            return;
        }

        if (!string.IsNullOrEmpty(parentSuite))
        {
            labels.Add(Label.ParentSuite(parentSuite!));
        }

        if (!string.IsNullOrEmpty(suite))
        {
            labels.Add(Label.Suite(suite!));
        }

        if (!string.IsNullOrEmpty(subSuite))
        {
            labels.Add(Label.SubSuite(subSuite!));
        }
    }

    static bool IsSuiteLabel(Label label) => label.Name switch
    {
        LabelName.ParentSuite or LabelName.Suite or LabelName.SubSuite => true,
        _ => false
    };
}
