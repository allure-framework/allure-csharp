namespace Allure.Sdk.Functions;

public static class AttachmentSource
{
    /// <summary>
    /// Returns a name for an attachment file.
    /// </summary>
    /// <param name="fileExtension">An optional file extension.</param>
    public static string CreateName(string fileExtension = "")
    {
        fileExtension ??= "";
        var suffix = "-attachment";
        var uuid = Ids.NewUuid();
        return $"{uuid}{suffix}{fileExtension}";
    }
}
