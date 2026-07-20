namespace Allure.Model;

public class Attachment
{
    required public string Name { get; set; }

    required public string Source { get; set; }

    public string? MediaType { get; set; }

    required public string FileExtension { get; set; }
}
