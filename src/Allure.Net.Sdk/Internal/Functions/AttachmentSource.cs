using System;
using System.Linq;
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
    /// <param name="fileExtension">
    /// An optional file extension.
    /// It cannot contain path separator characters '/' and '\'.
    /// </param>
    internal static string CreateName(string fileExtension = "")
    {
        fileExtension = NormalizeExtension(fileExtension);
        var suffix = "-attachment";
        var uuid = Ids.NewUuid();
        return $"{uuid}{suffix}{fileExtension}";
    }

    static string NormalizeExtension(string fileExtension) =>
        fileExtension switch
        {
            null => throw new ArgumentNullException(nameof(fileExtension)),

            _ when fileExtension.Any(static (c) => c is '/' or '\\') =>
                throw new ArgumentException(
                    $"The file extension contains invalid characters",
                    nameof(fileExtension)
                ),

            { Length: > 0 } when fileExtension[0] is not '.' => $".{fileExtension}",

            _ => fileExtension,
        };
}
