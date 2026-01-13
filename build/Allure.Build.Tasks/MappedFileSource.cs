using System.IO;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

class MappedFileSource(string sourcePath, string destinationPath): FileSource(destinationPath)
{
    public FileInfo Source { get; } = new(sourcePath);

    protected override bool HasChanged =>
        this.Source.LastWriteTime > this.Destination.LastWriteTime;

    protected override void WriteInternal()
    {
        this.Source.CopyTo(this.Destination.FullName, true);
    }

    public override void ShowChanged(TaskLoggingHelper log) =>
        log.LogMessage(
            "{0} -> {1} (updated)",
            this.Source.FullName,
            this.Destination.FullName
        );

    public override void ShowUnchanged(TaskLoggingHelper log) =>
        log.LogMessage(
            "{0} skipped, {1} is newer",
            this.Source.FullName,
            this.Destination.FullName
        );
}