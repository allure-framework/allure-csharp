using System.IO;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

abstract class FileSource(string destinationPath)
{
    public FileInfo Destination { get; } = new(destinationPath);

    public bool ShouldWrite => !this.Destination.Exists || this.HasChanged;

    protected abstract bool HasChanged { get; }

    public abstract void ShowChanged(TaskLoggingHelper log);

    public abstract void ShowUnchanged(TaskLoggingHelper log);

    protected abstract void WriteInternal();

    public void Write()
    {
        var directory = this.Destination.Directory;
        if (!directory.Exists)
        {
            directory.Create();
        }
        this.WriteInternal();
    }
}
