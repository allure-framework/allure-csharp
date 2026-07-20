using System.Collections.Generic;

namespace Allure.Model;

public sealed class TestResult : ExecutableItem
{
    required public string Uuid { get; init; }

    public string? FullName { get; set; }

    public string? TestCaseId { get; set; }

    public string? HistoryId { get; set; }

    public List<string> TitlePath { get; init; } = [];

    public List<Label> Labels { get; init; } = [];

    public List<Link> Links { get; init; } = [];
}
