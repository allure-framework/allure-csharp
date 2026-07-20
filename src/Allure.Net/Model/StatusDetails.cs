namespace Allure.Model;

public class StatusDetails
{
    required public string Message { get; set; }

    public string? Trace { get; set; }

    public bool Flaky { get; set; }

    public bool Known { get; set; }

    public bool Muted { get; set; }
}