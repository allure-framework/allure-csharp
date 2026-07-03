using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Allure.Build.Tasks.Functions;
using Allure.Build.Tasks.Sources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public class GenerateSampleSolution : Task
{
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

    IEnumerable<ITaskItem2> SampleSources2 =>
        this.SampleSources.Cast<ITaskItem2>();

    IEnumerable<ITaskItem2> SamplePackageReferences2 =>
        this.SamplePackageReferences.Cast<ITaskItem2>();

    IEnumerable<ITaskItem2> SampleProjectReferences2 =>
        this.SampleProjectReferences.Cast<ITaskItem2>();

    IEnumerable<ITaskItem2> CommonPackageReferences =>
        this.SamplePackageReferences2.Where(static (spec) =>
        {
            var optional = spec.GetMetadataValueEscaped("Optional");
            return string.IsNullOrEmpty(optional) || optional.ToLower() is not "true";
        });

    IEnumerable<string> AnalyzerPathsRelativeToSolutionDir =>
        this.SampleProjectReferences2
            .SelectMany((item) => item
                .GetMetadata("AnalyzerProjects")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select((path) => Files.RebasePath(
                    Files.GetOsMatchingPath(path),
                    this.ProjectDirectory,
                    this.SampleSolutionDir
                ))
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
        var (csprojRelativePaths, projectFiles) = this.PrepareProjects();
        var slnx = this.PrepareSolutionFile(csprojRelativePaths);

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
            this.SampleProjectReferences2
        );

    GeneratedFileSource PrepareDirectorySolutionTargets() =>
        Files.DirectorySolutionTargets(this.SampleSolutionDir);

    GeneratedFileSource PrepareDirectoryBuildProps(MsBuildImportFiles imports) =>
        Files.DirectoryBuildProps(
            solutionDir: this.SampleSolutionDir,
            targetFrameworks: this.SampleTargetFrameworks,
            imports.PropsFiles,
            packages: this.CommonPackageReferences.Select(static (item) => item.ItemSpec),
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
            this.SamplePackageReferences2.Select(
                static (item) => (item.ItemSpec, item.GetMetadata("Version"))
            )
        );

    GeneratedFileSource PrepareNugetConfig() =>
        Files.NugetConfig(
            this.LocalNugetRepository,
            this.PackageCacheDirectory,
            this.SampleSolutionDir
        );

    (ImmutableArray<string> paths, ImmutableArray<FileSource> sources) PrepareProjects() =>
        Projects.GenerateProjects(
            this.Log,
            this.SampleSolutionDir,
            this.ProjectDirectory,
            this.RootNamespace,
            this.SampleSources2
        );

    GeneratedFileSource PrepareSolutionFile(IEnumerable<string> projectPaths) =>
        Files.Solution(this.SampleSolutionPath, projectPaths);
}
