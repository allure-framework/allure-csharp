using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.Tasks.Functions;

public static class Projects
{
    public static (ImmutableArray<string> paths, ImmutableArray<FileSource> sources) GenerateProjects(
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string testProjectDirectory,
        string testProjectRootNamespace,
        IEnumerable<ITaskItem2> sampleSources
    )
    {
        var x = sampleSources
            .Select((sampleItem) => ToAllureSample(
                log,
                testProjectDirectory,
                testProjectRootNamespace,
                sampleItem
            ))
            .Where(static (sample) => sample.WellDefined)
            .GroupBy(
                static (sample) => sample.ProjectName)
            .Select((group) => GenerateProject(log, sampleSolutionDir, group.Key, [..group]))
            .Where(static (pair) => pair.path is not null)
            .ToImmutableArray();

        return (
            [..x.Select(static (p) => p.path)],
            [..x.SelectMany(static (p) => p.files)]
        );
    }

    static AllureSample ToAllureSample(
        TaskLoggingHelper log,
        string testProjectDirectory,
        string testProjectRootNamespace,
        ITaskItem2 sample
    )
    {
        var path = GetSamplePath(log, sample);
        var sampleName = GetSampleName(log, testProjectDirectory, sample);
        var registryNamespace = GetRegistryNamespace(
            log,
            testProjectDirectory,
            testProjectRootNamespace,
            sample
        );
        var projectName = GetSampleMetadata(log, sample, "ProjectName");
        var properties = GetSampleSpecificProperties(sample);

        var wellDefines
            = path.Length > 0
                && sampleName.Length > 0
                && registryNamespace.Length > 0
                && projectName.Length > 0;

        return new (path, sampleName, registryNamespace, projectName, properties, wellDefines);
    }

    static (string path, ImmutableArray<FileSource> files) GenerateProject(
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
            return (null, []);
        }

        Logging.LogGreatestCommonPrefixMessage(log, sampleProjectName, greatestCommonPrefix);

        return CreateProjectFileSources(
            log,
            sampleSolutionDir,
            sampleProjectName,
            samples,
            greatestCommonPrefix
        );
    }

    static string GetSamplePath(TaskLoggingHelper log, ITaskItem2 sample)
    {
        var path = sample.EvaluatedIncludeEscaped;
        if (string.IsNullOrEmpty(path))
        {
            Logging.LogNoPath(log, sample);
            return "";
        }

        if (!Path.Exists(path))
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

    static (string, ImmutableArray<FileSource>) CreateProjectFileSources(
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string sampleProjectName,
        ImmutableArray<AllureSample> samples,
        string greatestCommonPrefix
    )
    {
        var sampleProjectDir = Path.Combine(sampleSolutionDir, sampleProjectName);

        var sampleSources = PrepareSampleSources(
            log: log,
            samples: samples,
            greatestCommonPrefix: greatestCommonPrefix,
            sampleProjectDir: sampleProjectDir
        )
            .GroupBy(static (s) => s.Destination.Extension)
            .ToImmutableDictionary(
                static (g) => g.Key,
                static (g) => g.ToImmutableArray(),
                StringComparer.OrdinalIgnoreCase
            );

        var compile = sampleSources
                .GetValueOrDefault(".cs", [])
                .Select((cs) => Path.GetRelativePath(sampleProjectDir, cs.Source.FullName));

        var csproj = Files.Project(
            projectDir: sampleProjectDir,
            projectName: sampleProjectName,
            properties: GetSampleProjectProperties(samples),
            compile: compile
        );

        var csprojRelativePath = Path.GetRelativePath(
            sampleSolutionDir,
            csproj.Destination.FullName
        );

        var restProjectSampleSources = sampleSources
            .Where(static (kv) => !StringComparer.OrdinalIgnoreCase.Equals(kv.Key, ".cs"))
            .SelectMany(static (kv) => kv.Value);

        return (csprojRelativePath, [csproj, ..restProjectSampleSources]);
    }

    static IEnumerable<MappedFileSource> PrepareSampleSources(
        TaskLoggingHelper log,
        IEnumerable<AllureSample> samples,
        string greatestCommonPrefix,
        string sampleProjectDir
    ) =>
        samples
            .Select((sample) =>
                PrepareSampleSource(log, sampleProjectDir, greatestCommonPrefix, sample))
            .Where(static (sample) => sample is not null);


    static MappedFileSource PrepareSampleSource(
        TaskLoggingHelper log,
        string sampleProjectDir,
        string greatestCommonPrefix,
        AllureSample sample
    )
    {
        var absolutePath = sample.Path;
        var relativeSampleFilePath = Path.GetRelativePath(greatestCommonPrefix, absolutePath);
        if (relativeSampleFilePath.StartsWith($"..{Path.DirectorySeparatorChar}"))
        {
            Logging.LogFileOutsideProjectWarning(
                log,
                sampleProjectDir,
                absolutePath,
                relativeSampleFilePath
            );
            return null;
        }
        else
        {
            var destination = Path.Combine(sampleProjectDir, relativeSampleFilePath);
            return new (absolutePath, destination);
        }
    }

    static IEnumerable<(string key, string value)> GetSampleProjectProperties(
        IEnumerable<AllureSample> sampleSources
    ) =>
        [.. sampleSources
            .SelectMany(static (sample) => sample.MsbuildProperties)
            .Distinct()];
}