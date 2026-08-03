using System.Collections.Generic;

namespace Allure.Model;

/// <summary>
/// Contains fields shared by tests, fixtures, and steps.
/// </summary>
public class ExecutableItem
{
    /// <summary>
    /// Gets or sets the item's display name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the item's outcome.
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// Gets or sets details about the item's outcome.
    /// </summary>
    public StatusDetails? StatusDetails { get; set; }

    /// <summary>
    /// Gets or sets the item's execution stage.
    /// </summary>
    public Stage Stage { get; set; }

    /// <summary>
    /// Gets or sets the item's Markdown description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the item's HTML description.
    /// </summary>
    public string? DescriptionHtml { get; set; }

    /// <summary>
    /// Gets the item's nested steps.
    /// </summary>
    public List<StepResult> Steps { get; init; } = [];

    /// <summary>
    /// Gets the item's attachments.
    /// </summary>
    public List<Attachment> Attachments { get; init; } = [];

    /// <summary>
    /// Gets the item's parameters.
    /// </summary>
    public List<Parameter> Parameters { get; init; } = [];

    /// <summary>
    /// Gets or sets the start time as Unix epoch milliseconds.
    /// </summary>
    public long Start { get; set; }

    /// <summary>
    /// Gets or sets the stop time as Unix epoch milliseconds.
    /// </summary>
    public long Stop { get; set; }
}
