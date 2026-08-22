using Allure.Sdk.Functions;

namespace Allure.Sdk.Internal.Functions;

/// <summary>
/// Generates source file names for Allure attachments.
/// </summary>
static class AttachmentSource
{
    /// <summary>
    /// Returns a name for an attachment file.
    /// </summary>
    /// <param name="fileExtension">An optional file extension.</param>
    internal static string CreateName(string fileExtension = "")
    {
        fileExtension ??= "";
        var suffix = "-attachment";
        var uuid = Ids.NewUuid();
        return $"{uuid}{suffix}{fileExtension}";
    }
}
