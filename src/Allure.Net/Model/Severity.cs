namespace Allure.Model;

/// <summary>
/// Specifies the impact of a test failure.
/// </summary>
public enum Severity
{
    /// <summary>
    /// A failure blocks testing of a feature or product.
    /// </summary>
    Blocker,

    /// <summary>
    /// A failure affects critical functionality.
    /// </summary>
    Critical,

    /// <summary>
    /// A failure has normal impact.
    /// </summary>
    Normal,

    /// <summary>
    /// A failure has minor impact.
    /// </summary>
    Minor,

    /// <summary>
    /// A failure has trivial impact.
    /// </summary>
    Trivial,
}
