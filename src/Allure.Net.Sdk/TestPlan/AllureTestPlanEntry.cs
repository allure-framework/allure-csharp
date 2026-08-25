namespace Allure.Sdk.TestPlan;

/// <summary>
/// An Allure test plan entry that selects a test case to run.
/// </summary>
/// <param name="Id">
/// Gets the expected ALLURE_ID label value.
/// </param>
/// <param name="Selector">
/// Gets the expected fullName value.
/// </param>
public record class AllureTestPlanEntry(
    string? Id,
    string? Selector
);
