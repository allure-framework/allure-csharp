namespace Allure.Model;

/// <summary>
/// Represents an error that is not associated with a test or fixture.
/// </summary>
public sealed class GlobalError : StatusDetails
{
    /// <summary>
    /// Gets or sets the occurrence time as Unix epoch milliseconds.
    /// </summary>
    public long Timestamp { get; set; }
}
