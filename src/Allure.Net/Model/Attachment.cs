namespace Allure.Model;

/// <summary>
/// Describes an attachment stored in the Allure results directory.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Gets or sets the attachment's display name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the attachment file name in the results directory.
    /// </summary>
    required public string Source { get; set; }

    /// <summary>
    /// Gets or sets the attachment's media type.
    /// </summary>
    /// <remarks>
    /// If set to <see langword="null"/>, the media type is derived by the
    /// report generator.
    /// </remarks>
    public string? MediaType { get; set; }

    /// <summary>
    /// Gets or sets the attachment file extension, including the leading dot.
    /// </summary>
    required public string FileExtension { get; set; }
}
