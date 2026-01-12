using System;
using System.Diagnostics;
using System.IO;

namespace Allure.Testing.Internal;

internal class Guard<T>(T value, Action<T> dispose, bool own = true) : IDisposable
{
    public T Value { get; init; } = value;

    public void Dispose()
    {
        if (own)
        {
            dispose(this.Value);
        }
    }
}

internal static class Guard
{
    public static Guard<string> WrapFile(string path) => new(path, File.Delete);

    public static Guard<DirectoryInfo> WrapDirectory(DirectoryInfo dir, bool own) =>
        new(dir, (dir) => dir.Delete(true), own);

    public static Guard<Process> WrapProcess(Process process) => new(process, EnsureStopped);

    static void EnsureStopped(Process process)
    {
        try
        {
            process.Refresh();

            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch (Exception)
        {

        }
    }
}
