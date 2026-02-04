using System.IO;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public class MappedFileSource(string sourcePath, string destinationPath): FileSource(destinationPath)
{
    public FileInfo Source { get; } = new(sourcePath);

    protected override bool HasChanged =>
        this.Source.LastWriteTime > this.Destination.LastWriteTime;

    protected override void WriteInternal()
    {
        this.Source.CopyTo(this.Destination.FullName, true);
    }

    public override void ShowChanged(TaskLoggingHelper log) =>
        Logging.LogMappingFileChanged(log, this);

    public override void ShowUnchanged(TaskLoggingHelper log) =>
        Logging.LogMappingFileUnchanged(log, this);
}