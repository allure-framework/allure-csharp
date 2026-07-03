using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Allure.Build.Tasks;

public class GenerateSampleSolution : Task
{
    static readonly StringComparer fsComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

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

    public override bool Execute()
    {
        var imports = this.GetImportsOfReferencedProjects();
        var directorySolutionTargets = GenerateDirectorySolutionTargets();
        var directoryBuildProps = this.GenerateDirectoryBuildProps(imports.PropsFiles);
        var directoryBuildTargets = this.GenerateDirectoryBuildTargets(imports.TargetsFiles);
        var directoryPackagesProps = this.GenerateDirectoryPackagesProps();
        var nugetConfig = this.GenerateNugetConfig();
        var projects = this.GenerateProjects();
        var slnx = this.GenerateSlnx(projects);
        this.CommitSampleFiles([
            slnx,
            directorySolutionTargets,
            directoryBuildProps,
            directoryBuildTargets,
            directoryPackagesProps,
            nugetConfig,
            ..projects.SelectMany(static (pair) => pair.Item2),
        ]);
        return true;
    }

    void CommitSampleFiles(IEnumerable<FileSource> sources)
    {
        var existingFiles = this.CollectExistingFiles();
        var (isNew, updated, relevantFiles) = this.WriteSampleFiles(sources);
        int removed = this.RemoveStaleFiles(existing: existingFiles, relevant: relevantFiles);
        Logging.LogCommitSummary(this.Log, this.SampleSolutionName, (isNew, updated, removed));
    }

    ImmutableHashSet<string> CollectExistingFiles() =>
        Directory.Exists(this.SampleSolutionDir)
            ? Directory.EnumerateFiles(
                this.SampleSolutionDir,
                "*",
                SearchOption.AllDirectories)
                .ToImmutableHashSet(fsComparer)
            : [];

    int RemoveStaleFiles(ImmutableHashSet<string> existing, ImmutableHashSet<string> relevant)
    {
        int removed = 0;
        foreach (var file in existing.Except(relevant))
        {
            try
            {
                File.Delete(file);
                Logging.LogStaleDeletion(this.Log, file);
                removed++;
            }
            catch (Exception e)
            {
                Logging.LogStaleDeletionFailedWarning(this.Log, file, e);
            }
        }
        return removed;
    }

    (bool, int, ImmutableHashSet<string>) WriteSampleFiles(IEnumerable<FileSource> sources)
    {
        var files = ImmutableHashSet.CreateBuilder(fsComparer);

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
                source.ShowChanged(this.Log);
                updatedFilesCount++;
            }
            else
            {
                source.ShowUnchanged(this.Log);
            }
        }
        return (isNew, updatedFilesCount, files.ToImmutable());
    }

    static XDocument CreateSlnxXml(List<(string, List<FileSource>)> projects) => new(
        new XElement(
            new XElement(
                "Solution",
                projects.Select(static (pair) => new XElement(
                    "Project",
                    new XAttribute("Path", Path.Combine(pair.Item1, $"{pair.Item1}.csproj"))
                ))
            )
        )
    );

    List<(string, List<FileSource>)> GenerateProjects() =>
        [.. this.SampleSources2
            .Select(this.ToAllureSample)
            .Where(static (sample) => sample.WellDefined)
            .GroupBy(
                static (sample) => sample.ProjectName)
            .Select(this.GenerateProject)
            .Where(static (pair) => pair.Item1 is not null)];

    AllureSample ToAllureSample(ITaskItem2 sample)
    {
        var path = this.GetSamplePath(sample);
        var sampleName = this.GetSampleName(sample);
        var registryNamespace = this.GetRegistryNamespace(sample);
        var projectName = this.GetSampleMetadata(sample, "ProjectName");
        var properties = GetSampleSpecificProperties(sample);

        var wellDefines
            = path.Length > 0
                && sampleName.Length > 0
                && registryNamespace.Length > 0
                && projectName.Length > 0;

        return new (path, sampleName, registryNamespace, projectName, properties, wellDefines);
    }

    string GetSamplePath(ITaskItem2 sample)
    {
        var path = sample.EvaluatedIncludeEscaped;
        if (string.IsNullOrEmpty(path))
        {
            Logging.LogNoPath(this.Log, sample);
            return "";
        }

        if (!Path.Exists(path))
        {
            Logging.LogFileNotExist(this.Log, sample);
            return "";
        }

        return path;
    }

    string GetSampleName(ITaskItem2 sample)
    {
        var sampleName = this.GetSampleMetadata(sample, "SampleName");
        if (sampleName.Length > 0 && !SyntaxFacts.IsValidIdentifier(sampleName))
        {
            Logging.LogInvalidSampleNameWarning(
                this.Log,
                sample,
                sampleName,
                this.ProjectDirectory
            );
            return "";
        }
        return sampleName;
    }

    string GetRegistryNamespace(ITaskItem2 sample)
    {
        var registryNamespace = this.GetSampleMetadata(sample, "RegistryNamespace");
        if (registryNamespace.Length > 0 && !Functions.IsValidNamespace(registryNamespace))
        {
            Logging.LogInvalidRegistryNamespaceWarning(
                this.Log,
                sample,
                registryNamespace,
                this.ProjectDirectory,
                this.RootNamespace
            );
            return "";
        }
        return registryNamespace;
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

    string GetSampleMetadata(ITaskItem2 sample, string metadataKey)
    {
        var value = sample.GetMetadataValueEscaped(metadataKey);
        if (string.IsNullOrEmpty(value))
        {
            Logging.LogNoMetadata(this.Log, sample, metadataKey);
            return "";
        }

        return value;
    }

    (string, List<FileSource>) GenerateProject(IGrouping<string, AllureSample> projectSourcesGroup)
    {
        var projectName = projectSourcesGroup.Key;

        var greatestCommonPrefix = GetGreatestCommonPrefix(projectSourcesGroup);
        if (greatestCommonPrefix is "")
        {
            Logging.LogMissingCommonPrefixWarning(this.Log, projectSourcesGroup, projectName);
            return (null, []);
        }

        Logging.LogGreatestCommonPrefixMessage(this.Log, projectName, greatestCommonPrefix);

        return CreateProjectFileSources(projectSourcesGroup, projectName, greatestCommonPrefix);
    }

    (string, List<FileSource>) CreateProjectFileSources(
        IGrouping<string, AllureSample> sampleGroup,
        string sampleProjectName,
        string greatestCommonPrefix
    )
    {
        var sampleProjectDir = Path.Combine(this.SampleSolutionDir, sampleProjectName);

        var csproj = GenerateCsproj(sampleProjectName, sampleProjectDir, sampleGroup);
        var sampleSources = this.PrepareSampleSources(
            sampleGroup,
            greatestCommonPrefix,
            sampleProjectDir
        );

        return (sampleProjectName, [csproj, ..sampleSources]);
    }

    IEnumerable<MappedFileSource> PrepareSampleSources(
        IGrouping<string, AllureSample> sampleGroup,
        string greatestCommonPrefix,
        string sampleProjectDir
    ) =>
        sampleGroup
            .Select((sample) =>
                this.PrepareSampleSource(sampleProjectDir, greatestCommonPrefix, sample))
            .Where(static (sample) => sample is not null);

    MappedFileSource PrepareSampleSource(
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
                this.Log,
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

    static XDocument CreateCsprojXml(IEnumerable<AllureSample> sampleSources)
    {
        var project = new XElement(
            "Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk")
        );

        var properties
            = sampleSources
                .SelectMany(static (sample) => sample.MsbuildProperties)
                .Distinct()
                .ToImmutableArray();

        if (properties.Length > 0)
        {
            project.Add(new XElement(
                "PropertyGroup",
                properties.Select(
                    static (p) => new XElement(p.key, p.value)
                )
            ));
        }

        return new(project);
    }

    static string GetGreatestCommonPrefix(IEnumerable<AllureSample> samples) =>
        Functions.GetGreatestCommonPrefix(samples.Select(static sample => sample.Path));

    XDocument CreateNugetConfigXml()
    {
        var packageSources = new XElement(
            "packageSources",
            new XElement("clear"),
            new XElement(
                "add",
                new XAttribute("key", "nuget"),
                new XAttribute("value", "https://api.nuget.org/v3/index.json")
            ),
            new XElement(
                "add",
                new XAttribute("key", "local"),
                new XAttribute("value", this.LocalNugetRepository)
            )
        );

        IEnumerable<XElement> configurationElements = [packageSources];
        if (!string.IsNullOrEmpty(this.PackageCacheDirectory))
        {
            var config = new XElement(
                "config",
                new XElement(
                    "add",
                    new XAttribute("key", "globalPackagesFolder"),
                    new XAttribute("value", this.PackageCacheDirectory)
                )
            );
            configurationElements = configurationElements.Prepend(config);
        }

        return new(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("configuration", configurationElements)
        );
    }

    IEnumerable<XElement> CreateProjectReferencesXml()
    {
        if (this.CommonPackageReferences.Any())
        {
            yield return this.CreateCommonPackageReferencesXml();
        }

        if (this.SampleProjectReferences.Any())
        {
            yield return this.CreateCommonProjectReferencesXml();
        }
    }

    MsBuildImportFiles GetImportsOfReferencedProjects() =>
        this.SampleProjectReferences2
            .Select((r) => CollectDependencyImportFiles(
                Path.GetFullPath(r.EvaluatedIncludeEscaped, this.ProjectDirectory)
            ))
            .Aggregate(new MsBuildImportFiles([], []), (f, s) => f with
            {
                PropsFiles = [..f.PropsFiles, ..s.PropsFiles],
                TargetsFiles = [..f.TargetsFiles, ..s.TargetsFiles],
            });

    MsBuildImportFiles CollectDependencyImportFiles(string projectFullPath)
    {
        var graph = new Microsoft.Build.Graph.ProjectGraph(projectFullPath);
        var projects = graph.ProjectNodesTopologicallySorted
            .Select(static (n) => n.ProjectInstance)
            .ToImmutableArray();
        var arr = projects
            .Select(static (n) => n.FullPath)
            .ToArray();

        var buildResults = this.BuildEngine9.BuildProjectFilesInParallel(
            projectFileNames: arr,
            targetNames: ["Allure_GetPackageFiles"],
            globalProperties: [.. Enumerable.Repeat<System.Collections.IDictionary>(null, arr.Length)],
            removeGlobalProperties: null,
            toolsVersion: [.. Enumerable.Repeat<string>(null, arr.Length)],
            returnTargetOutputs: true
        );

        if (buildResults is not { Result: true, TargetOutputsPerProject: var outputs })
        {
            this.Log.LogWarning($"Could not resolve the set of MSBuild extension files for {projectFullPath}");
            return new([], []);
        }

        var packageFiles =
            (from projectIndex in Enumerable.Range(0, arr.Length)
            let project = projects[projectIndex]
            let projectName2 = project.GetPropertyValue("ProjectName")
            let buildDirectories = projectIndex == 0
                ? new[] { "build", "buildTransitive" }
                : new[] { "buildTransitive" }
            let expectedPropsFiles =    (from dir in buildDirectories
                                        select Path.Combine(dir, $"{projectName2}.props"))
                                            .ToImmutableArray()
            let expectedTargetsFiles =  (from dir in buildDirectories
                                        select Path.Combine(dir, $"{projectName2}.targets"))
                                            .ToImmutableArray()
            from projectTargetOutput in outputs[projectIndex].Values
            from ITaskItem2 packageFile in projectTargetOutput
            let packagePaths = Functions.NormalizePath(packageFile.GetMetadata("PackagePath"))
                    .Split(';')
                    .Select(static (p) => p.TrimStart(Path.DirectorySeparatorChar))
                    .Select((p) => p.Length == 0 || p.EndsWith(Path.DirectorySeparatorChar)
                        ? (p
                            + Functions.NormalizePath(packageFile.GetMetadata("RecursiveDir"))
                            + packageFile.GetMetadata("Filename")
                            + packageFile.GetMetadata("Extension"))
                        : p)
                    .ToImmutableHashSet()
            let isProps = packagePaths.Intersect(expectedPropsFiles) is { Count: >0 }
            let isTargets = packagePaths.Intersect(expectedTargetsFiles) is { Count: >0 }
            where isProps || isTargets
            select (
                isProps: isProps,
                path: Functions.ResolveToNewBase(
                    packageFile.ItemSpec,
                    project.Directory,
                    this.SampleSolutionDir
                )
            )).ToImmutableArray();

        return new (
            PropsFiles: packageFiles
                .Where(static (f) => f.isProps)
                .Select(static (f) => f.path)
                .Distinct(fsComparer),
            TargetsFiles: packageFiles
                .Where(static (f) => !f.isProps)
                .Select(static (f) => f.path)
                .Distinct(fsComparer)
        );
    }

    XElement CreateCommonPackageReferencesXml() => new(
        "ItemGroup",
        this.CommonPackageReferences.Select(static (item) => new XElement(
            "PackageReference",
            new XAttribute("Include", item.EvaluatedIncludeEscaped)
        ))
    );

    XElement CreateCommonProjectReferencesXml() => new(
        "ItemGroup",
        this.SampleProjectReferences2.Select((item) => new XElement(
            "ProjectReference",
            new XAttribute(
                "Include",
                this.ResolveDependencyProjectPath(item)
            )
        ))
    );

    string ResolveDependencyProjectPath(ITaskItem2 dependencyProject) =>
        GetMsBuildRelativePathEvaluation(
            dependencyProject.EvaluatedIncludeEscaped,
            this.ProjectDirectory,
            this.SampleSolutionDir
        );

    static string GetMsBuildRelativePathEvaluation(string relativePath) =>
        $"$([MSBuild]::NormalizePath('$(MSBuildThisFileDirectory)', '{relativePath}'))";

    static string GetMsBuildRelativePathEvaluation(string fullPath, string newBasePath) =>
        GetMsBuildRelativePathEvaluation(
            Path.GetRelativePath(newBasePath, fullPath)
        );

    static string GetMsBuildRelativePathEvaluation(string maybeRelativePath, string basePath, string newBasePath) =>
        GetMsBuildRelativePathEvaluation(Path.GetFullPath(maybeRelativePath, basePath), newBasePath);

    static IEnumerable<XElement> CreateImportElements(IEnumerable<string> imports) =>
        imports.Select(CreateImportElement);

    static XElement CreateImportElement(string import) => new(
        "Import",
        new XAttribute(
            "Project",
            GetMsBuildRelativePathEvaluation(import)
        )
    );

    static XElement CreateParentImport(string import) => new(
        "Import",
        new XAttribute(
            "Project",
            $"$([MSBuild]::GetPathOfFileAbove('{import}', '$(MSBuildThisFileDirectory)../'))"
        ),
        new XAttribute(
            "Condition",
            $"'' != $([MSBuild]::GetPathOfFileAbove('{import}', '$(MSBuildThisFileDirectory)../'))"
        )
    );

    XElement CreateCommonProjectProperties() => new(
        "PropertyGroup",
        new XElement("TargetFrameworks", this.SampleTargetFrameworks),
        new XElement("OutputType", "Library"),
        new XElement("EnableDefaultItems", "false"),
        new XElement("IsTestProject", "true"),
        new XElement("AspectInjector_Enabled", "false")
    );

    static XElement CreateCommonProjectCompileItems() => new(
        "ItemGroup",
        new XElement(
            "Compile",
            new XAttribute("Include", "**/*.cs")
        )
    );

    static XElement CreateAllureResultsCleanItems() => new(
        "ItemGroup",
        new XElement(
            "Clean",
            new XAttribute("Include", "$(TargetDir)allure-results/**/*")
        )
    );

    GeneratedFileSource GenerateSlnx(List<(string, List<FileSource>)> projects)
    {
        var slnxXml = CreateSlnxXml(projects);
        return GeneratedFileSource.FromXmlDocument(
            slnxXml,
            this.SampleSolutionPath,
            omitDeclaration: true
        );
    }

    GeneratedFileSource GenerateDirectorySolutionTargets() => GeneratedFileSource.FromXmlDocument(
        DirectorySolutionTargetXml,
        Path.Combine(this.SampleSolutionDir, "Directory.Solution.targets")
    );

    static GeneratedFileSource GenerateCsproj(
        string sampleProjectName,
        string sampleProjectDir,
        IEnumerable<AllureSample> sampleSources
    )
    {
        var csprojXml = CreateCsprojXml(sampleSources);
        var csprojPath = Path.Combine(sampleProjectDir, $"{sampleProjectName}.csproj");

        return GeneratedFileSource.FromXmlDocument(csprojXml, csprojPath, omitDeclaration: true);
    }

    GeneratedFileSource GenerateNugetConfig()
    {
        var nugetConfigPath = Path.Combine(this.SampleSolutionDir, "nuget.config");
        var nugetConfigXml = CreateNugetConfigXml();

        return GeneratedFileSource.FromXmlDocument(nugetConfigXml, nugetConfigPath);
    }

    GeneratedFileSource GenerateDirectoryPackagesProps() =>
        this.GenerateMsbuildExtensionFile(
            "Directory.Packages.props",
            [
                new XElement(
                    "PropertyGroup",
                    new XElement("ManagePackageVersionsCentrally", "true")
                ),
                new XElement(
                    "ItemGroup",
                    this.SamplePackageReferences2.Select(static (spec) => new XElement(
                        "PackageVersion",
                        new XAttribute("Include", spec.EvaluatedIncludeEscaped),
                        new XAttribute("Version", spec.GetMetadataValueEscaped("Version")))
                    )
                )
            ]
        );

    GeneratedFileSource GenerateDirectoryBuildProps(IEnumerable<string> imports) =>
        this.GenerateMsbuildExtensionFile(
            "Directory.Build.props",
            [
                CreateParentImport("Directory.Build.props"),
                ..CreateImportElements(imports),
                this.CreateCommonProjectProperties(),
                CreateCommonProjectCompileItems(),
                CreateAllureResultsCleanItems(),
                ..CreateProjectReferencesXml(),
            ]
        );

    GeneratedFileSource GenerateDirectoryBuildTargets(IEnumerable<string> imports) =>
        this.GenerateMsbuildExtensionFile(
            "Directory.Build.targets",
            [
                CreateParentImport("Directory.Build.targets"),
                ..CreateImportElements(imports),
            ]
        );

    GeneratedFileSource GenerateMsbuildExtensionFile(string relativePath, params IEnumerable<XElement> projectChildren)
    {
        var outputPath = Path.Combine(this.SampleSolutionDir, relativePath);
        XDocument content = new(
            new XElement("Project", projectChildren)
        );

        return GeneratedFileSource.FromXmlDocument(
            content,
            outputPath,
            omitDeclaration: true
        );
    }

    static XDocument DirectorySolutionTargetXml { get; }
        = new(
            new XElement(
                "Project",
                new XElement(
                    "Target",
                    new XAttribute("Name", "Allure_RunTestSamples"),
                    new XElement(
                        "MSBuild",
                        new XAttribute("Projects", "@(ProjectReference)"),
                        new XAttribute("Targets", "$(Allure_TestSampleTarget)"),
                        new XAttribute("BuildInParallel", "$(Allure_BuildInParallel)"),
                        new XAttribute(
                            "Properties",
                            "BuildingSolutionFile=true;"
                                + "CurrentSolutionConfigurationContents=$(CurrentSolutionConfigurationContents);"
                                + "SolutionDir=$(SolutionDir);"
                                + "SolutionExt=$(SolutionExt);"
                                + "SolutionFileName=$(SolutionFileName);"
                                + "SolutionName=$(SolutionName);"
                                + "SolutionPath=$(SolutionPath)"
                        )
                    )
                )
            )
        );

    record MsBuildImportFiles(IEnumerable<string> PropsFiles, IEnumerable<string> TargetsFiles);
}
