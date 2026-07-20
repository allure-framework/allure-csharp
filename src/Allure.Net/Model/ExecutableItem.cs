using System.Collections.Generic;

namespace Allure.Model;

public class ExecutableItem
{
    required public string Name { get; set; }

    public Status Status { get; set; }

    public StatusDetails? StatusDetails { get; set; }

    public Stage Stage { get; set; }

    public string? Description { get; set; }

    public string? DescriptionHtml { get; set; }

    public List<StepResult> Steps { get; init; } = [];

    public List<Attachment> Attachments { get; init; } = [];

    public List<Parameter> Parameters { get; init; } = [];

    public long Start { get; set; }

    public long Stop { get; set; }
}