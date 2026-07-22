using System.Collections.Generic;

namespace Allure.Model;

/// <summary>
/// Contains result data that is not associated with a test or fixture.
/// </summary>
public sealed class Globals
{
    /// <summary>
    /// Gets the global attachments.
    /// </summary>
    public List<GlobalAttachment> Attachments { get; init; } = [];

    /// <summary>
    /// Gets the global errors.
    /// </summary>
    public List<GlobalError> Errors { get; init; } = [];
}
