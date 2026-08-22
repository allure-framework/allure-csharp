using System.IO;
using System.Text;
using Allure.Model;
using Allure.Sdk.Results;

namespace Allure.TestingPlatform.Internal.Functions;

static class TestAttachments
{
    public static void SaveText(
        IAllureResultsDestination resultsDestination,
        TestResult testResult,
        string name,
        string content
    )
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        using MemoryStream stream = new(contentBytes);
        var outputFileName = resultsDestination.WriteAttachment(stream, ".txt");

        testResult.Attachments.Add(new()
        {
            Name = name,
            Type = "text/plain",
            Source = outputFileName,
        });
    }

    public static void SaveFile(
        IAllureResultsDestination resultsDestination,
        TestResult testResult,
        string? name,
        FileInfo file
    )
    {
        var inputPath = file.FullName;
        var outputFileName = resultsDestination.CopyAttachment(inputPath);

        var attachment = new Attachment
        {
            Name = name ?? file.Name,
            Source = outputFileName,
        };

        testResult.Attachments.Add(attachment);
    }
}
