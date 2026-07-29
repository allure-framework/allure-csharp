namespace Allure.Model;

/// <summary>
/// Specifies the outcome of a test, fixture, or step.
/// </summary>
public enum Status
{
    /// <summary>
    /// The outcome is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The item completed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// The item failed because an assertion was not satisfied.
    /// </summary>
    Failed,

    /// <summary>
    /// The item failed for a reason other than an assertion.
    /// </summary>
    Broken,

    /// <summary>
    /// The item was skipped.
    /// </summary>
    Skipped,
}
