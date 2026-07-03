using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Allure.Build.Tasks.Functions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public class GenerateSampleSolution : Task
{
    [Required]
    public ITaskItem[] SampleItemTypes { get; set; }

    [Required]
    public ITaskItem[] SampleSources { get; set; }

    [Required]
    public ITaskItem[] SamplePackageReferences { get; set; }

    [Required]
    public ITaskItem[] SampleProjectReferences { get; set; }

    [Required]
    public string ProjectDirectory { get; set; }

    [Required]
    public string RootNamespace { get; set; }

    [Required]
    public string SampleSolutionDir { get; set; }

    [Required]
    public string SampleSolutionName { get; set; }

    [Required]
    public string SampleSolutionPath { get; set; }

    [Required]
    public string SampleTargetFrameworks { get; set; }

    [Required]
    public string LocalNugetRepository { get; set; }

    public string PackageCacheDirectory { get; set; }

    IEnumerable<string> CommonPackageNames =>
        from x in this.SamplePackageReferences
        where x.GetMetadata("Optional").ToLower() is "" or not "true"
        select x.ItemSpec;

    IEnumerable<string> AnalyzerPathsRelativeToSolutionDir =>
        this.SampleProjectReferences
            .SelectMany((item) => item
                .GetMetadata("AnalyzerProjects")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select((path) => Files.RebasePath(
                    Files.GetOsMatchingPath(path),
                    this.ProjectDirectory,
                    this.SampleSolutionDir
                ))
            );


    public ImmutableDictionary<string, string> SampleItemTypeMap =>
        this.SampleItemTypes.ToImmutableDictionary(
            static (m) => m.ItemSpec,
            static (m) => m.GetMetadata("ItemType") is { Length: >0 } itemType
                ? itemType
                : "None",
            StringComparer.OrdinalIgnoreCase,
            StringComparer.Ordinal
        );


    static internal TaskLoggingHelper log;

    public override bool Execute()
    {
        log = this.Log;
        var imports = this.GetImportsOfReferencedProjects();

        var directorySolutionTargets = this.PrepareDirectorySolutionTargets();
        var directoryBuildProps = this.PrepareDirectoryBuildProps(imports);
        var directoryBuildTargets = this.PrepareDirectoryBuildTargets(imports);
        var directoryPackagesProps = this.PrepareDirectoryPackagesProps();
        var nugetConfig = this.PrepareNugetConfig();
        var projectFiles = this.PrepareProjects();
        var slnx = this.PrepareSolutionFile(projectFiles);

        FileProcessor.CommitSampleFiles(this.Log, this.SampleSolutionDir, [
            slnx,
            directorySolutionTargets,
            directoryBuildProps,
            directoryBuildTargets,
            directoryPackagesProps,
            nugetConfig,
            ..projectFiles,
        ]);

        return true;
    }

    MsBuildImportFiles GetImportsOfReferencedProjects() =>
        Imports.GetImportsOfReferencedProjects(
            this.BuildEngine3,
            this.Log,
            this.SampleSolutionDir,
            this.ProjectDirectory,
            this.SampleProjectReferences
        );

    GeneratedFileSource PrepareDirectorySolutionTargets() =>
        Files.DirectorySolutionTargets(this.SampleSolutionDir);

    GeneratedFileSource PrepareDirectoryBuildProps(MsBuildImportFiles imports) =>
        Files.DirectoryBuildProps(
            solutionDir: this.SampleSolutionDir,
            targetFrameworks: this.SampleTargetFrameworks,
            imports.PropsFiles,
            packages: this.CommonPackageNames,
            projects: this.SampleProjectReferences.Select((item) =>
                Files.RebasePath(item.ItemSpec, this.ProjectDirectory, this.SampleSolutionDir)
            ),
            analyzerProjects: this.AnalyzerPathsRelativeToSolutionDir
        );

    GeneratedFileSource PrepareDirectoryBuildTargets(MsBuildImportFiles imports) =>
        Files.DirectoryBuildTargets(this.SampleSolutionDir, imports.TargetsFiles);

    GeneratedFileSource PrepareDirectoryPackagesProps() =>
        Files.DirectoryPackagesProps(
            this.SampleSolutionDir,
            this.SamplePackageReferences.Select(
                static (item) => (item.ItemSpec, item.GetMetadata("Version"))
            )
        );

    GeneratedFileSource PrepareNugetConfig() =>
        Files.NugetConfig(
            this.LocalNugetRepository,
            this.PackageCacheDirectory,
            this.SampleSolutionDir
        );

    ImmutableArray<GeneratedFileSource> PrepareProjects() =>
        Projects.GenerateProjects(
            this.Log,
            this.SampleSolutionDir,
            this.ProjectDirectory,
            this.RootNamespace,
            this.SampleItemTypeMap,
            this.SampleSources
        );

    GeneratedFileSource PrepareSolutionFile(IEnumerable<GeneratedFileSource> csprojFiles) =>
        Files.Solution(
            this.SampleSolutionPath,
            csprojFiles.Select(static (f) => f.Destination.FullName)
        );
}
