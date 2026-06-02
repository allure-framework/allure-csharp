using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public static class Logging
{
    public static void LogUnexpectedLayoutWarning(TaskLoggingHelper log, string path)
        => log.LogWarning(
            $"Can't resolve metadata of '{path}': it's not inside a 'Samples' directory. "
                + "Please, fill at least the following metadata: SampleName, "
                + "RegistryNamespace, and ProjectName.");

    public static void LogFileOutsideProjectWarning(
        TaskLoggingHelper log,
        string sampleProjectDir,
        string path,
        string relativeSampleFilePath
    )
        => log.LogWarning(
            "Ignoring '{0}': the file's calculated destination '{1}' is outside of the "
                + "sample project directory '{2}'",
            path,
            relativeSampleFilePath,
            sampleProjectDir
        );

    public static void LogMissingCommonPrefixWarning(
        TaskLoggingHelper log,
        IGrouping<string, AllureSample> sampleGroup,
        string projectSuffix
    )
        => log.LogWarning(
            "Ignoring {0}: the sample files [{1}] don't have a common prefix.",
            projectSuffix,
            string.Join(
                ", ",
                sampleGroup.Select(static (sample) => $"'{sample.Path}'")
            )
        );

    public static void LogItemsWithNoSuffix(
        TaskLoggingHelper log,
        IGrouping<string, ITaskItem2> sampleGroup
    )
    {
        foreach (var sample in sampleGroup)
        {
            log.LogWarning(
                "Ignoring '{0}': no ProjectSuffix defined on the item.",
                sample.EvaluatedIncludeEscaped
            );
        }
    }

    public static void LogNoPath(TaskLoggingHelper log, ITaskItem2 item) =>
        log.LogWarning("Ignoring '{0}': bad path.", item.EvaluatedIncludeEscaped);

    public static void LogFileNotExist(TaskLoggingHelper log, ITaskItem2 item) =>
        log.LogWarning("Ignoring '{0}': thefile does not exist.", item.EvaluatedIncludeEscaped);

    public static void LogNoMetadata(
        TaskLoggingHelper log,
        ITaskItem2 item,
        string metadataKey
    ) =>
        log.LogWarning(
            "Ignoring '{0}': no {1} defined on the item.",
            item.EvaluatedIncludeEscaped,
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
            sample.EvaluatedIncludeEscaped,
            sampleName,
            Path.GetRelativePath(projectDirectory, sample.EvaluatedIncludeEscaped)
        );

    public static void LogInvalidRegistryNamespaceWarning(
        TaskLoggingHelper log,
        ITaskItem2 sample,
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
            sample.EvaluatedIncludeEscaped,
            registryNamespace,
            Path.GetRelativePath(projectDirectory, sample.EvaluatedIncludeEscaped),
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
        (bool, int) summaryData
    )
    {
        var (isNew, updatedFilesCount) = summaryData;
        if (updatedFilesCount == 0)
        {
            log.LogMessage(
                MessageImportance.High,
                "{0} is up to date",
                sampleSolutionName
            );
        }
        else if (isNew)
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
                updatedFilesCount,
                sampleSolutionName
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