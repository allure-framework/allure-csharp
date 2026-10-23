namespace Allure.Model;

/// <summary>
/// Classifies a global error based on whether it was caused by an assertion failure.
/// </summary>
public enum GlobalErrorStatus
{
    /// <summary>
    /// The error was caused by something other than a failed assertion.
    /// </summary>
    Broken,

    /// <summary>
    /// The error was caused by a failed assertion.
    /// </summary>
    Failed,
}
