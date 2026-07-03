using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.Tasks.Functions;

public static class Projects
{
    public static ImmutableArray<GeneratedFileSource> GenerateProjects(
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string testProjectDirectory,
        string testProjectRootNamespace,
        ImmutableDictionary<string, string> itemTypeMap,
        IEnumerable<ITaskItem2> sampleSources
    ) =>
        [
            ..sampleSources
                .Select((sampleItem) => ToAllureSample(
                    log,
                    testProjectDirectory,
                    testProjectRootNamespace,
                    itemTypeMap,
                    sampleItem
                ))
                .Where(static (sample) => sample.WellDefined)
                .GroupBy(
                    static (sample) => sample.ProjectName)
                .Select((group) => GenerateProject(log, sampleSolutionDir, group.Key, [..group]))
                .Where(static (csproj) => csproj is not null)
        ];

    static AllureSample ToAllureSample(
        TaskLoggingHelper log,
        string testProjectDirectory,
        string testProjectRootNamespace,
        ImmutableDictionary<string, string> itemTypeMap,
        ITaskItem2 sample
    )
    {
        var path = GetSamplePath(log, testProjectDirectory, sample);
        var sampleName = GetSampleName(log, testProjectDirectory, sample);
        var registryNamespace = GetRegistryNamespace(
            log,
            testProjectDirectory,
            testProjectRootNamespace,
            sample
        );
        var projectName = GetSampleMetadata(log, sample, "ProjectName");
        var properties = GetSampleSpecificProperties(sample);
        var itemType = itemTypeMap.GetValueOrDefault(Path.GetExtension(path), "None");

        var wellDefined
            = path.Length > 0
                && sampleName.Length > 0
                && registryNamespace.Length > 0
                && projectName.Length > 0;

        return new(
            Path: path,
            SampleName: sampleName,
            RegistryNamespace: registryNamespace,
            ProjectName: projectName,
            MsbuildProperties: properties,
            ItemType: itemType,
            ItemMetadata: [
                ..itemType is not "None"
                    || sample.MetadataNames.OfType<string>().Contains("Sample_CopyToOutputDirectory")
                        ? (IEnumerable<(string, string)>)[]
                        : [("CopyToOutputDirectory", "PreserveNewest")],
                ..sample.MetadataNames
                    .OfType<string>()
                    .Where(static (n) => n.StartsWith("Sample_"))
                    .Select((n) => (key: n, value: sample.GetMetadata(n)))
            ],
            WellDefined: wellDefined
        );
    }

    static GeneratedFileSource GenerateProject(
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string sampleProjectName,
        ImmutableArray<AllureSample> samples
    )
    {
        var greatestCommonPrefix = GetGreatestCommonPrefix(samples);
        if (greatestCommonPrefix is "")
        {
            Logging.LogMissingCommonPrefixWarning(log, samples, sampleProjectName);
            return null;
        }

        Logging.LogGreatestCommonPrefixMessage(log, sampleProjectName, greatestCommonPrefix);

        return CreateProjectFileSources(
            sampleSolutionDir,
            sampleProjectName,
            samples
        );
    }

    static string GetSamplePath(
        TaskLoggingHelper log,
        string basePath,
        ITaskItem2 sample
    )
    {
        var path = sample.ItemSpec;
        if (string.IsNullOrEmpty(path))
        {
            Logging.LogNoPath(log, sample);
            return "";
        }

        var fullPath = Path.GetFullPath(path, basePath);
        if (!Path.Exists(fullPath))
        {
            Logging.LogFileNotExist(log, sample);
            return "";
        }

        return path;
    }

    static string GetSampleName(
        TaskLoggingHelper log,
        string testProjectDirectory,
        ITaskItem2 sample
    )
    {
        var sampleName = GetSampleMetadata(log, sample, "SampleName");
        if (sampleName.Length > 0 && !SyntaxFacts.IsValidIdentifier(sampleName))
        {
            Logging.LogInvalidSampleNameWarning(
                log,
                sample,
                sampleName,
                testProjectDirectory
            );
            return "";
        }
        return sampleName;
    }

    static string GetRegistryNamespace(
        TaskLoggingHelper log,
        string testProjectDirectory,
        string testProjectRootNamespace,
        ITaskItem2 sample
    )
    {
        var registryNamespace = GetSampleMetadata(log, sample, "RegistryNamespace");
        if (registryNamespace.Length > 0 && !Syntax.IsValidNamespace(registryNamespace))
        {
            Logging.LogInvalidRegistryNamespaceWarning(
                log,
                sample,
                registryNamespace,
                testProjectDirectory,
                testProjectRootNamespace
            );
            return "";
        }
        return registryNamespace;
    }

    static string GetSampleMetadata(TaskLoggingHelper log, ITaskItem2 sample, string metadataKey)
    {
        var value = sample.GetMetadataValueEscaped(metadataKey);
        if (string.IsNullOrEmpty(value))
        {
            Logging.LogNoMetadata(log, sample, metadataKey);
            return "";
        }

        return value;
    }

    static ImmutableArray<(string key, string value)> GetSampleSpecificProperties(
        ITaskItem2 sample
    )
        => [
            .. sample
                .GetMetadataValueEscaped("Properties")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(static (p) => p.Split('=', StringSplitOptions.RemoveEmptyEntries))
                .Where(static (p) => p.Length == 2)
                .Select(static (p) => (Key: p[0], Value: p[1]))
                .GroupBy(
                    static (p) => p.Key,
                    static (key, values) => (Key: key, values.Last().Value)
                )
        ];

    static string GetGreatestCommonPrefix(IEnumerable<AllureSample> samples) =>
        Files.GetGreatestCommonPrefix(samples.Select(static sample => sample.Path));

    static GeneratedFileSource CreateProjectFileSources(
        string sampleSolutionDir,
        string sampleProjectName,
        ImmutableArray<AllureSample> samples
    ) =>
        Files.Project(
            projectDir: Path.Combine(sampleSolutionDir, sampleProjectName),
            projectName: sampleProjectName,
            properties: GetSampleProjectProperties(samples),
            files: samples
        );

    static IEnumerable<(string key, string value)> GetSampleProjectProperties(
        IEnumerable<AllureSample> sampleSources
    ) =>
        [.. sampleSources
            .SelectMany(static (sample) => sample.MsbuildProperties)
            .Distinct()];
}