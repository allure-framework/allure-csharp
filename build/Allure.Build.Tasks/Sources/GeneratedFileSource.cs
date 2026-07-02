using System.IO;
using System.Xml;
using System.Xml.Linq;
using Allure.Build.Tasks.Functions;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public class GeneratedFileSource(byte[] content, string destinationPath): FileSource(destinationPath)
{
    public byte[] Content { get; init; } = content;

    protected override bool HasChanged
    {
        get
        {
            using var dst = this.Destination.OpenRead();
            var dstLength = dst.Length;

            if (this.Content.Length != dstLength)
            {
                return true;
            }

            var srcBuf = this.Content;
            using var dstBuf = new BufferedStream(this.Destination.OpenRead());
            for (int i = 0; i < dstLength; i++)
            {
                if (srcBuf[i] != dstBuf.ReadByte())
                {
                    return true;
                }
            }

            return false;
        }
    }

    protected override void WriteInternal()
    {
        using var dstStream = this.Destination.OpenWrite();
        dstStream.Write(this.Content);
        dstStream.SetLength(this.Content.Length);
    }

    public override void ShowChanged(TaskLoggingHelper log) =>
        Logging.LogGeneratedFileChanged(log, this);

    public override void ShowUnchanged(TaskLoggingHelper log) =>
        Logging.LogGeneratedFileUnchanged(log, this);

    public static GeneratedFileSource FromXmlDocument(
        XDocument document,
        string destinationPath
    ) =>
        FromXmlDocument(document, destinationPath, false);

    public static GeneratedFileSource FromXmlDocument(
        XDocument document,
        string destinationPath,
        bool omitDeclaration
    )
    {
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = omitDeclaration,
        });
        document.Save(writer);
        writer.Flush();
        stream.Position = 0;
        return new(stream.ToArray(), destinationPath);
    }
}