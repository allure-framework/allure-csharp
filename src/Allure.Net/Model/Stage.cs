namespace Allure.Model;

/// <summary>
/// Specifies the execution stage of a test, fixture, or step.
/// </summary>
public enum Stage
{
    /// <summary>
    /// The item is scheduled for execution.
    /// </summary>
    Scheduled,

    /// <summary>
    /// The item is running.
    /// </summary>
    Running,

    /// <summary>
    /// The item has finished.
    /// </summary>
    Finished,

    /// <summary>
    /// The item is awaiting execution or completion.
    /// </summary>
    Pending,

    /// <summary>
    /// The item was interrupted.
    /// </summary>
    Interrupted,
}
