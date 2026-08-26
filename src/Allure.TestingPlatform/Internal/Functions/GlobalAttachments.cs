using System;
using System.IO;
using Allure.Model;
using Allure.Sdk.Results;

namespace Allure.TestingPlatform.Internal.Functions;

static class GlobalAttachments
{
    public static void SaveFile(
        IAllureResultsDestination resultsDestination,
        string? name,
        FileInfo file
    )
    {
        var inputPath = file.FullName;
        var outputFileName = resultsDestination.CopyAttachment(
            inputPath,
            file.Extension
        );

        resultsDestination.WriteGlobals(
            new Globals
            {
                Attachments =
                [
                    new GlobalAttachment
                    {
                        Name = name ?? file.Name,
                        Source = outputFileName,
                        Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    },
                ],
            }
        );
    }
}
