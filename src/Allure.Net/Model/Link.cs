namespace Allure.Model;

public sealed class Link
{
    required public string Url { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }
}