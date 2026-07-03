using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks.Functions;

public static class FileProcessor
{
    public static void CommitSampleFiles(
        TaskLoggingHelper log,
        string targetDirectory,
        IEnumerable<FileSource> sources
    )
    {
        var existingFiles = CollectExistingFiles(targetDirectory);
        var (isNew, updated, relevantFiles) = WriteSampleFiles(log, sources);
        int removed = RemoveStaleFiles(log, existing: existingFiles, relevant: relevantFiles);
        Logging.LogCommitSummary(log, targetDirectory, (isNew, updated, removed));
    }

    static ImmutableHashSet<string> CollectExistingFiles(
        string targetDirectory
    ) =>
        Directory.Exists(targetDirectory)
            ? Directory.EnumerateFiles(
                targetDirectory,
                "*",
                SearchOption.AllDirectories)
                .ToImmutableHashSet(Files.FsComparer)
            : [];


    static (bool, int, ImmutableHashSet<string>) WriteSampleFiles(
        TaskLoggingHelper log,
        IEnumerable<FileSource> sources
    )
    {
        var files = ImmutableHashSet.CreateBuilder(Files.FsComparer);

        bool isNew = true;
        int updatedFilesCount = 0;
        foreach (var source in sources)
        {
            var fullName = source.Destination.FullName;
            files.Add(fullName);

            if (source.Destination.Exists)
            {
                isNew = false;
            }

            if (source.ShouldWrite)
            {
                source.Write();
                source.ShowChanged(log);
                updatedFilesCount++;
            }
            else
            {
                source.ShowUnchanged(log);
            }
        }
        return (isNew, updatedFilesCount, files.ToImmutable());
    }

    static int RemoveStaleFiles(
        TaskLoggingHelper log,
        ImmutableHashSet<string> existing, ImmutableHashSet<string> relevant
    )
    {
        int removed = 0;
        foreach (var file in existing.Except(relevant))
        {
            try
            {
                File.Delete(file);
                removed++;
                Logging.LogStaleDeletion(log, file);
            }
            catch (Exception e)
            {
                Logging.LogStaleDeletionFailedWarning(log, file, e);
            }
        }
        return removed;
    }

}