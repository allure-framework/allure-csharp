namespace Allure.Model;

/// <summary>
/// Describes a link associated with a test.
/// </summary>
public sealed class Link
{
    /// <summary>
    /// Gets or sets the link's URL.
    /// </summary>
    required public string Url { get; set; }

    /// <summary>
    /// Gets or sets the link's display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the link type.
    /// </summary>
    public string? Type { get; set; }
}
