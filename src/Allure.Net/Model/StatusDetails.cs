namespace Allure.Model;

/// <summary>
/// Provides diagnostic and classification details for an execution status.
/// </summary>
public class StatusDetails
{
    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    required public string Message { get; set; }

    /// <summary>
    /// Gets or sets the stack trace or other diagnostic details.
    /// </summary>
    public string? Trace { get; set; }

    /// <summary>
    /// Gets or sets whether the failure is considered flaky.
    /// </summary>
    public bool Flaky { get; set; }

    /// <summary>
    /// Gets or sets whether the failure is known.
    /// </summary>
    public bool Known { get; set; }

    /// <summary>
    /// Gets or sets whether the failure is muted.
    /// </summary>
    public bool Muted { get; set; }
}
