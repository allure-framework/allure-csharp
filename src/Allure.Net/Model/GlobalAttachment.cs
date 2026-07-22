namespace Allure.Model;

/// <summary>
/// Represents an attachment that is not associated with a test or fixture.
/// </summary>
public sealed class GlobalAttachment : Attachment
{
    /// <summary>
    /// Gets or sets the creation time as Unix epoch milliseconds.
    /// </summary>
    public long Timestamp { get; set; }
}
