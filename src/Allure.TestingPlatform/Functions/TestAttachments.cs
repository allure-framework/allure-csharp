using System.IO;
using System.Text;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.Sdk.Results;

namespace Allure.TestingPlatform.Functions;

static class TestAttachments
{
    public static void SaveText(
        IAllureResultsDestination writer,
        TestResult testResult,
        string name,
        string content
    )
    {
        var outputFileName = AttachmentSource.CreateName(".txt");
        var contentBytes = Encoding.UTF8.GetBytes(content);
        using MemoryStream stream = new(contentBytes);

        testResult.Attachments.Add(new()
        {
            Name = name,
            Type = "text/plain",
            Source = outputFileName,
        });
        writer.WriteAttachment(outputFileName, stream);
    }

    public static void SaveFile(
        IAllureResultsDestination writer,
        TestResult testResult,
        string? name,
        FileInfo file
    )
    {
        var inputPath = file.FullName;
        var outputFileName = AttachmentSource.CreateName(".txt");

        var attachment = new Attachment
        {
            Name = name ?? file.Name,
            Source = outputFileName,
        };

        writer.CopyAttachment(outputFileName, inputPath);
        testResult.Attachments.Add(attachment);
    }
}
