using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks.Functions;

public static class Logging
{
    public static void LogResolveRegistryEntryNoMetadata(
        TaskLoggingHelper log,
        ITaskItem item,
        string metadataKey
    ) =>
        log.LogError(
            "Invalid sample file '{0}': no {1} defined.",
            item.ItemSpec,
            metadataKey
        );

    public static void LogResolveRegistryEntryInvalidNamespace(
        TaskLoggingHelper log,
        ITaskItem item,
        string registryNamespace
    ) =>
        log.LogError(
            "Invalid sample file '{0}': invalid RegistryNamespace '{1}'. The value must be a valid namespace.",
            item.ItemSpec,
            registryNamespace
        );

    public static void LogResolveRegistryEntryInvalidSampleName(
        TaskLoggingHelper log,
        ITaskItem item,
        string sampleName
    ) =>
        log.LogError(
            "Invalid sample file '{0}': invalid SampleName '{1}'. The value must be a valid C# identifier.",
            item.ItemSpec,
            sampleName
        );

    public static void LogResolveRegistryEntryMissingCommonPrefix(
        TaskLoggingHelper log,
        string registryNamespace,
        string sampleName,
        IEnumerable<string> sampleFiles
    ) =>
        log.LogError(
            "Can't resolve registry '{0}' sample '{1}': the sample files [{2}] don't have a common prefix.",
            registryNamespace,
            sampleName,
            string.Join(", ", sampleFiles.Select(static path => $"'{path}'"))
        );

    public static void LogResolveRegistryEntryInconsistentMetadata(
        TaskLoggingHelper log,
        string registryNamespace,
        string sampleName,
        string metadataKey,
        IEnumerable<string> values
    ) =>
        log.LogError(
            "Can't resolve registry '{0}' sample '{1}': inconsistent {2} values [{3}] were resolved for the same registry entry.",
            registryNamespace,
            sampleName,
            metadataKey,
            string.Join(", ", values.Select(static value => $"'{value}'"))
        );

    public static void LogResolveRegistryEntryDuplicateValue(
        TaskLoggingHelper log,
        string metadataKey,
        string value,
        (string Registry, string Entry) first,
        (string Registry, string Entry) second
    ) =>
        log.LogError(
            "Can't resolve registry '{0}' sample '{1}': {2} '{3}' is already used by registry '{4}' sample '{5}'.",
            second.Registry,
            second.Entry,
            metadataKey,
            value,
            first.Registry,
            first.Entry
        );

    public static void LogUnexpectedLayoutWarning(TaskLoggingHelper log, string path)
        => log.LogWarning(
            $"Can't resolve metadata of '{path}': it's not inside a 'Samples' directory. "
                + "Please, fill at least the following metadata: SampleName, "
                + "RegistryNamespace, and ProjectName.");

    public static void LogStaleDeletionFailedWarning(TaskLoggingHelper log, string path, Exception error)
        => log.LogWarning($"Couldn't delete a stale file '{path}': {error.Message}.");

    public static void LogStaleDeletion(TaskLoggingHelper log, string path)
        => log.LogMessage($"Deleted a stale file '{path}'.");

    public static void LogMissingCommonPrefixWarning(
        TaskLoggingHelper log,
        IEnumerable<AllureSample> projectSamples,
        string projectSuffix
    )
        => log.LogWarning(
            "Ignoring {0}: the sample files [{1}] don't have a common prefix.",
            projectSuffix,
            string.Join(
                ", ",
                projectSamples.Select(static (sample) => $"'{sample.Path}'")
            )
        );

    public static void LogNoPath(TaskLoggingHelper log, ITaskItem item) =>
        log.LogWarning("Ignoring '{0}': bad path.", item.ItemSpec);

    public static void LogFileNotExist(TaskLoggingHelper log, ITaskItem item) =>
        log.LogWarning("Ignoring '{0}': the file does not exist.", item.ItemSpec);

    public static void LogNoMetadata(
        TaskLoggingHelper log,
        ITaskItem item,
        string metadataKey
    ) =>
        log.LogWarning(
            "Ignoring '{0}': no {1} defined on the item.",
            item.ItemSpec,
            metadataKey
        );

    public static void LogInvalidSampleNameWarning(
        TaskLoggingHelper log,
        ITaskItem2 sample,
        string sampleName,
        string projectDirectory
    )
        => log.LogWarning(
            "Ignoring {0}: invalid SampleName '{1}' defined on the item. "
                + "A sample name must be a valid C# identifier. "
                + "Please, rename the corresponding file or folder, "
                + "or assign the value manually. For example:"
                + """

                    <ItemGroup>
                      <AllureSample Update="{2}" SampleName="ValidName" />
                    </ItemGroup>

                  """,
            sample.ItemSpec,
            sampleName,
            // Keep escapint to display a value that is ready for copy and paste
            // to the test project file.
            Path.GetRelativePath(projectDirectory, sample.EvaluatedIncludeEscaped)
        );

    public static void LogInvalidRegistryNamespaceWarning(
        TaskLoggingHelper log,
        ITaskItem sample,
        string registryNamespace,
        string projectDirectory,
        string rootNamespace
    )
        => log.LogWarning(
            "Ignoring {0}: invalid RegistryNamespace '{1}' defined on the item. "
                + "A registry namespace must be a valid C# namespace. "
                + "Please, rename the corresponding folder(s), "
                + "or assign the value manually. For example:"
                + """

                    <ItemGroup>
                      <AllureSample Update="{2}" RegistryNamespace="{3}" />
                    </ItemGroup>

                  """,
            sample.ItemSpec,
            registryNamespace,
            Path.GetRelativePath(projectDirectory, sample.ItemSpec),
            rootNamespace
        );

    public static void LogGreatestCommonPrefixMessage(
        TaskLoggingHelper log,
        string projectSuffix,
        string greatestCommonPrefix
    )
        => log.LogMessage(
            MessageImportance.Low,
            "The greatest common prefix of '{0}' files is '{1}'",
            projectSuffix,
            greatestCommonPrefix
        );

    public static void LogCommitSummary(
        TaskLoggingHelper log,
        string sampleSolutionName,
        (bool IsNew, int Updated, int Removed) summary
    )
    {
        if (summary.Updated == 0)
        {
            log.LogMessage(
                MessageImportance.High,
                "{0} is up to date",
                sampleSolutionName
            );
        }
        else if (summary.IsNew)
        {
            log.LogMessage(
                MessageImportance.High,
                "{0} successfully generated",
                sampleSolutionName
            );
        }
        else
        {
            log.LogMessage(
                MessageImportance.High,
                "{0} files of {1} were updated",
                summary.Updated,
                sampleSolutionName
            );
        }

        if (summary.Removed > 0)
        {
            log.LogMessage(
                MessageImportance.High,
                "{0} stale files were deleted",
                summary.Removed
            );
        }
    }

    public static void LogGeneratedFileChanged(TaskLoggingHelper log, GeneratedFileSource source)
        => log.LogMessage(
            "{0} bytes -> {1} (updated)",
            source.Content.Length,
            source.Destination.FullName
        );

    public static void LogGeneratedFileUnchanged(TaskLoggingHelper log, GeneratedFileSource source)
        => log.LogMessage(
            "{0} bytes skipped, {1} unchanged",
            source.Content.Length,
            source.Destination.FullName
        );

    public static void LogMappingFileChanged(TaskLoggingHelper log, MappedFileSource source)
        => log.LogMessage(
            "{0} -> {1} (updated)",
            source.Source.FullName,
            source.Destination.FullName);

    public static void LogMappingFileUnchanged(TaskLoggingHelper log, MappedFileSource source)
        => log.LogMessage(
            "{0} skipped, {1} is newer",
            source.Source.FullName,
            source.Destination.FullName);
}