using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Build.Tasks.Functions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.Tasks;

public class ResolveRegistryEntries : Task
{
    public record class RegistryDescriptor(
        string Namespace,
        ImmutableArray<RegistryEntryDescriptor> Entries
    );

    public record class RegistryEntryDescriptor(
        string SampleName,
        string SourcePath,
        string ProjectRelativePath,
        string ProjectFilePath,
        string ResultsDirectory
    );

    public record class SampleFile(
        string Path,
        string RegistryNamespace,
        string SampleName,
        string ProjectRelativePath,
        string ProjectFilePath,
        string ResultsDirectory
    );

    [Required]
    public ITaskItem[] SampleFiles { get; set; }

    [Output]
    public ITaskItem[] Entries { get; set; }

    IEnumerable<ITaskItem2> SampleFiles2 =>
        this.SampleFiles.Cast<ITaskItem2>();

    bool success = false;

    public override bool Execute()
    {
        this.success = true;

        var registryDescriptors = this.SampleFiles2
            .Select(this.CreateSampleFile)
            .Where(static file => file is not null)
            .GroupBy(
                static file => file!.RegistryNamespace,
                (registryNamespace, files) => new RegistryDescriptor(
                    registryNamespace,
                    [.. files
                        .GroupBy(
                            static file => file!.SampleName,
                            (sampleName, sampleFiles) => this.CreateValidEntryDescriptor(
                                registryNamespace,
                                sampleName,
                                [.. sampleFiles]))
                        .Where(static descriptor => descriptor is not null)]))
            .ToImmutableArray();

        this.Entries = [.. this.FlattenRegistryEntries(registryDescriptors)];

        return this.success;
    }

    SampleFile CreateSampleFile(ITaskItem2 item)
    {
        var registryNamespace = item.GetMetadataValueEscaped("RegistryNamespace");
        if (string.IsNullOrEmpty(registryNamespace))
        {
            Logging.LogResolveRegistryEntryNoMetadata(this.Log, item, "RegistryNamespace");
            success = false;
            return null;
        }

        if (!Syntax.IsValidNamespace(registryNamespace))
        {
            Logging.LogResolveRegistryEntryInvalidNamespace(this.Log, item, registryNamespace);
            success = false;
            return null;
        }

        var sampleName = item.GetMetadataValueEscaped("SampleName");
        if (string.IsNullOrEmpty(sampleName))
        {
            Logging.LogResolveRegistryEntryNoMetadata(this.Log, item, "SampleName");
            success = false;
            return null;
        }

        if (!SyntaxFacts.IsValidIdentifier(sampleName))
        {
            Logging.LogResolveRegistryEntryInvalidSampleName(this.Log, item, sampleName);
            success = false;
            return null;
        }

        var projectRelativePath = item.GetMetadataValueEscaped("ProjectRelativePath");
        if (string.IsNullOrEmpty(projectRelativePath))
        {
            Logging.LogResolveRegistryEntryNoMetadata(this.Log, item, "ProjectRelativePath");
            success = false;
            return null;
        }

        var projectFilePath = item.GetMetadataValueEscaped("ProjectFilePath");
        if (string.IsNullOrEmpty(projectFilePath))
        {
            Logging.LogResolveRegistryEntryNoMetadata(this.Log, item, "ProjectFilePath");
            success = false;
            return null;
        }

        var resultsDirectory = item.GetMetadataValueEscaped("ResultsDirectory");
        if (string.IsNullOrEmpty(resultsDirectory))
        {
            Logging.LogResolveRegistryEntryNoMetadata(this.Log, item, "ResultsDirectory");
            success = false;
            return null;
        }

        return new(
            Path: item.EvaluatedIncludeEscaped,
            RegistryNamespace: registryNamespace,
            SampleName: sampleName,
            ProjectRelativePath: projectRelativePath,
            ProjectFilePath: projectFilePath,
            ResultsDirectory: resultsDirectory
        );
    }

    RegistryEntryDescriptor CreateValidEntryDescriptor(
        string registryNamespace,
        string sampleName,
        ImmutableArray<SampleFile> sampleFiles
    )
    {
        var sourcePath = sampleFiles.Length == 1
            ? sampleFiles[0].Path
            : Files.GetGreatestCommonPrefix(
                sampleFiles.Select(static file => file.Path));

        if (string.IsNullOrEmpty(sourcePath))
        {
            Logging.LogResolveRegistryEntryMissingCommonPrefix(
                this.Log,
                registryNamespace,
                sampleName,
                sampleFiles.Select(static file => file.Path)
            );
            success = false;
            return null;
        }

        var projectRelativePaths = sampleFiles
            .Select(static file => file.ProjectRelativePath)
            .Distinct()
            .ToImmutableArray();
        if (projectRelativePaths.Length > 1)
        {
            Logging.LogResolveRegistryEntryInconsistentMetadata(
                this.Log,
                registryNamespace,
                sampleName,
                "ProjectRelativePath",
                projectRelativePaths
            );
            success = false;
            return null;
        }

        var projectFilePaths = sampleFiles
            .Select(static file => file.ProjectFilePath)
            .Distinct()
            .ToImmutableArray();
        if (projectFilePaths.Length > 1)
        {
            Logging.LogResolveRegistryEntryInconsistentMetadata(
                this.Log,
                registryNamespace,
                sampleName,
                "ProjectFilePath",
                projectFilePaths
            );
            success = false;
            return null;
        }

        var resultsDirectories = sampleFiles
            .Select(static file => file.ResultsDirectory)
            .Distinct()
            .ToImmutableArray();
        if (resultsDirectories.Length > 1)
        {
            Logging.LogResolveRegistryEntryInconsistentMetadata(
                this.Log,
                registryNamespace,
                sampleName,
                "ResultsDirectory",
                resultsDirectories
            );
            success = false;
            return null;
        }

        return new(
            SampleName: sampleName,
            SourcePath: sourcePath,
            ProjectRelativePath: projectRelativePaths[0],
            ProjectFilePath: projectFilePaths[0],
            ResultsDirectory: resultsDirectories[0]
        );
    }

    IEnumerable<ITaskItem> FlattenRegistryEntries(ImmutableArray<RegistryDescriptor> registries)
    {
        Dictionary<string, (string Registry, string Entry)> sourcePathToSample = [];
        Dictionary<string, (string Registry, string Entry)> projectRelativePathToSample = [];
        Dictionary<string, (string Registry, string Entry)> projectFilePathToSample = [];
        Dictionary<string, (string Registry, string Entry)> resultsDirectoryToSample = [];

        foreach (var registry in registries)
        {
            foreach (var sample in registry.Entries)
            {
                var sampleName = sample.SampleName;
                var sourcePath = sample.SourcePath;
                var projectRelativePath = sample.ProjectRelativePath;
                var projectFilePath = sample.ProjectFilePath;
                var resultsDirectory = sample.ResultsDirectory;

                if (sourcePathToSample.ContainsKey(sourcePath))
                {
                    Logging.LogResolveRegistryEntryDuplicateValue(
                        this.Log,
                        "SourcePath",
                        sourcePath,
                        sourcePathToSample[sourcePath],
                        (registry.Namespace, sample.SampleName)
                    );
                    success = false;
                    continue;
                }

                if (projectRelativePathToSample.ContainsKey(projectRelativePath))
                {
                    Logging.LogResolveRegistryEntryDuplicateValue(
                        this.Log,
                        "ProjectRelativePath",
                        projectRelativePath,
                        projectRelativePathToSample[projectRelativePath],
                        (registry.Namespace, sample.SampleName)
                    );
                    success = false;
                    continue;
                }

                if (projectFilePathToSample.ContainsKey(projectFilePath))
                {
                    Logging.LogResolveRegistryEntryDuplicateValue(
                        this.Log,
                        "ProjectFilePath",
                        projectFilePath,
                        projectFilePathToSample[projectFilePath],
                        (registry.Namespace, sample.SampleName)
                    );
                    success = false;
                    continue;
                }

                if (resultsDirectoryToSample.ContainsKey(resultsDirectory))
                {
                    Logging.LogResolveRegistryEntryDuplicateValue(
                        this.Log,
                        "ResultsDirectory",
                        resultsDirectory,
                        resultsDirectoryToSample[resultsDirectory],
                        (registry.Namespace, sample.SampleName)
                    );
                    success = false;
                    continue;
                }

                var lookupValue = (registry.Namespace, sample.SampleName);
                sourcePathToSample.Add(sourcePath, lookupValue);
                projectRelativePathToSample.Add(projectRelativePath, lookupValue);
                projectFilePathToSample.Add(projectFilePath, lookupValue);
                resultsDirectoryToSample.Add(resultsDirectory, lookupValue);

                ITaskItem2 item = new TaskItem(sourcePath, false);

                item.SetMetadataValueLiteral("RegistryNamespace", registry.Namespace);
                item.SetMetadataValueLiteral("SampleName", sampleName);
                item.SetMetadataValueLiteral("ProjectFilePath", sample.ProjectFilePath);
                item.SetMetadataValueLiteral("ProjectRelativePath", sample.ProjectRelativePath);
                item.SetMetadataValueLiteral("ResultsDirectory", sample.ResultsDirectory);

                yield return item;
            }
        }
    }
}
