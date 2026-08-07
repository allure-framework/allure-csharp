using System;
using System.IO;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.Sdk.Results;

namespace Allure.TestingPlatform.Functions;

static class GlobalAttachments
{
    public static void SaveFile(IAllureResultsDestination writer, string? name, FileInfo file)
    {
        var inputPath = file.FullName;
        var outputFileName = AttachmentSource.CreateName(file.Extension);
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        writer.CopyAttachment(outputFileName, inputPath);
        writer.WriteGlobals(
            new Globals
            {
                Attachments =
                [
                    new GlobalAttachment
                    {
                        Name = name ?? file.Name,
                        Source = outputFileName,
                        Timestamp = timestamp,
                    },
                ],
            }
        );
    }
}