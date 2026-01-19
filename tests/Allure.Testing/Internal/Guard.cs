using System;
using System.Diagnostics;
using System.IO;

namespace Allure.Testing.Internal;

internal class Guard<T>(T value, Action<T> dispose, bool own = true) : IDisposable
{
    readonly object monitor = new();

    public T Value { get; init; } = value;

    public bool Own { get; private set; } = own;

    public void Dispose()
    {
        lock(this.monitor)
        {
            if (this.Own)
            {
                dispose(this.Value);
            }
        }
    }

    public Guard<T> Transfer()
    {
        lock (this.monitor)
        {
            if (!this.Own)
            {
                throw new InvalidOperationException(
                    "Can't transfer: the guard don't own the resourse"
                );
            }

            this.Own = false;
        }

        return new(this.Value, dispose);
    }

    public static implicit operator Guard<T>(T value) =>
        new(value, static (v) => {}, false);
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
