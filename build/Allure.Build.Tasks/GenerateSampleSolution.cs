using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis.CSharp;

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
        var directorySolutionTargets = GenerateDirectorySolutionTargets();
        var directoryBuildProps = this.GenerateDirectoryBuildProps();
        var directoryPackagesProps = this.GenerateDirectoryPackagesProps();
        var nugetConfig = this.GenerateNugetConfig();
        var projects = this.GenerateProjects();
        var slnx = this.GenerateSlnx(projects);
        this.CommitSampleFiles([
            slnx,
            directorySolutionTargets,
            directoryBuildProps,
            directoryPackagesProps,
            nugetConfig,
            ..projects.SelectMany(static (pair) => pair.Item2),
        ]);
        return true;
    }

    void CommitSampleFiles(IEnumerable<FileSource> sources)
    {
        var summary = WriteSampleFiles(sources);
        LogCommitSummary(summary);
    }

    (bool, int) WriteSampleFiles(IEnumerable<FileSource> sources)
    {
        bool isNew = true;
        int updatedFilesCount = 0;
        foreach (var source in sources)
        {
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
        return (isNew, updatedFilesCount);
    }

    void LogCommitSummary((bool, int) summaryData)
    {
        var (isNew, updatedFilesCount) = summaryData;
        if (updatedFilesCount == 0)
        {
            this.Log.LogMessage(
                MessageImportance.High,
                "{0} is up to date",
                this.SampleSolutionName
            );
        }
        else if (isNew)
        {
            this.Log.LogMessage(
                MessageImportance.High,
                "{0} successfully generated",
                this.SampleSolutionName
            );
        }
        else
        {
            this.Log.LogMessage(
                MessageImportance.High,
                "{0} files of {1} were updated",
                updatedFilesCount,
                this.SampleSolutionName
            );
        }
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
            .GroupBy(
                static (sample) => sample.GetMetadataValueEscaped("ProjectSuffix") ?? "")
            .Select(this.GenerateProject)
            .Where(static (pair) => pair.Item1 is not null)];

    (string, List<FileSource>) GenerateProject(IGrouping<string, ITaskItem2> projectSourcesGroup)
    {
        var projectSuffix = projectSourcesGroup.Key;
        if (string.IsNullOrEmpty(projectSuffix))
        {
            this.ShowItemsWithNoSuffix(projectSourcesGroup);
            return (null, []);
        }

        if (!SyntaxFacts.IsValidIdentifier(projectSuffix))
        {
            this.ShowInvalidSuffixWarning(projectSourcesGroup, projectSuffix);
            return (null, []);
        }

        var greatestCommonPrefix = GetGreatestCommonPrefix(projectSourcesGroup);
        if (greatestCommonPrefix is "")
        {
            this.ShowMissingCommonPrefixWarning(projectSourcesGroup, projectSuffix);
            return (null, []);
        }

        this.ShowGreatestCommonPrefixMessage(projectSuffix, greatestCommonPrefix);

        return CreateProjectFileSources(projectSourcesGroup, projectSuffix, greatestCommonPrefix);
    }

    (string, List<FileSource>) CreateProjectFileSources(
        IGrouping<string, ITaskItem2> sampleGroup,
        string projectSuffix,
        string greatestCommonPrefix
    )
    {
        var sampleProjectName = $"{this.SampleSolutionName}.{projectSuffix}";
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
        IGrouping<string, ITaskItem2> sampleGroup,
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
        ITaskItem2 sample
    )
    {
        var absolutePath = sample.EvaluatedIncludeEscaped;
        var relativeSampleFilePath = Path.GetRelativePath(greatestCommonPrefix, absolutePath);
        if (relativeSampleFilePath.StartsWith($"..{Path.DirectorySeparatorChar}"))
        {
            this.ShowFileOutsideProjectWarning(sampleProjectDir, sample, relativeSampleFilePath);
            return null;
        }
        else
        {
            var destination = Path.Combine(sampleProjectDir, relativeSampleFilePath);
            return new (absolutePath, destination);
        }
    }

    void ShowFileOutsideProjectWarning(
        string sampleProjectDir,
        ITaskItem2 sample,
        string relativeSampleFilePath
    )
    {
        this.Log.LogWarning(
            "Ignoring '{0}': the file's calculated destination '{1}' is outside of the "
                + "sample project directory '{2}'",
            sample.EvaluatedIncludeEscaped,
            relativeSampleFilePath,
            sampleProjectDir
        );
    }

    void ShowGreatestCommonPrefixMessage(string projectSuffix, string greatestCommonPrefix)
    {
        this.Log.LogMessage(
            MessageImportance.Low,
            "The greatest common prefix of '{0}' is '{1}'",
            projectSuffix,
            greatestCommonPrefix
        );
    }

    void ShowMissingCommonPrefixWarning(IGrouping<string, ITaskItem2> sampleGroup, string projectSuffix)
    {
        this.Log.LogWarning(
            "Ignoring {0}: the sample files [{1}] don't have a common prefix.",
            projectSuffix,
            string.Join(
                ", ",
                sampleGroup.Select(static (sample) => $"'{sample.EvaluatedIncludeEscaped}'")
            )
        );
    }

    void ShowItemsWithNoSuffix(IGrouping<string, ITaskItem2> sampleGroup)
    {
        foreach (var sample in sampleGroup)
        {
            this.Log.LogWarning(
                "Ignoring '{0}': no ProjectSuffix defined on the item.",
                sample.EvaluatedIncludeEscaped
            );
        }
    }

    static XDocument CreateCsprojXml(IEnumerable<ITaskItem2> sampleSources)
    {
        var project = new XElement(
            "Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk")
        );

        var properties = GetSampleSpecificProperties(sampleSources);

        if (properties.Count > 0)
        {
            project.Add(new XElement(
                "PropertyGroup",
                properties.Select(
                    static (p) => new XElement(p.Key, p.Value)
                )
            ));
        }

        return new(project);
    }

    static List<(string Key, string Value)> GetSampleSpecificProperties(
        IEnumerable<ITaskItem2> sampleSources
    ) => [
        .. sampleSources
            .Select(static (item) => item.GetMetadataValueEscaped("Properties"))
            .Where(static (p) => !string.IsNullOrEmpty(p))
            .SelectMany(
                static (p) => p.Split(';', StringSplitOptions.RemoveEmptyEntries)
            )
            .Select(
                static (p) => p.Split('=', StringSplitOptions.RemoveEmptyEntries)
            )
            .Where(
                static (p) => p.Length == 2
            )
            .Select(
                static (p) => (Key: p[0], Value: p[1])
            )
            .GroupBy(
                static (p) => p.Key,
                static (key, values) => (Key: key, values.Last().Value)
            )
        ];

    void ShowInvalidSuffixWarning(IGrouping<string, ITaskItem2> sampleGroup, string projectSuffix)
    {
        this.Log.LogWarning(
            "Ignoring {0} sample file(s): invalid ProjectSuffix '{1}' defined on the item(s). "
                + "A project suffix must be a valid C# identifier. "
                + "Please, rename the corresponding file or folder, "
                + "or assign the value manually. For example:"
                + """

                    <ItemGroup>
                      <AllureSample Update="./Samples/{1}/**" ProjectSuffix="ValidSuffix" />
                    </ItemGroup>

                  """
                + "Here is the list of skipped files:\n{2}",
            sampleGroup.Count(),
            projectSuffix,
            string.Join(
                "\n",
                sampleGroup.Select(static (item) => $"  - {item.EvaluatedIncludeEscaped}")
            )
        );
    }

    static string GetGreatestCommonPrefix(IEnumerable<ITaskItem2> items)
    {
        var paths = items.Select(static (item) => item.EvaluatedIncludeEscaped);
        var first = paths.First();
        var rest = paths.Skip(1).ToList();

        string prefix = first;

        while ((prefix = Path.GetDirectoryName(prefix)) is not null)
        {
            if (IsCommonPrefix(rest, prefix))
            {
                return prefix;
            }
        }

        return "";
    }

    static bool IsCommonPrefix(List<string> files, string prefix) =>
        files.All((path) =>
            path.StartsWith(prefix)
                && path.Length > prefix.Length
                && path[prefix.Length] == Path.DirectorySeparatorChar);

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

    XDocument CreateDirectoryPackagesPropsXml() => new(
        new XElement(
            "Project",
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
        )
    );

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

    string ResolveDependencyProjectPath(ITaskItem2 dependencyProject)
    {
        var dependnecyProjectPath = Path.GetRelativePath(
            this.SampleSolutionDir,
            dependencyProject.EvaluatedIncludeEscaped
        );
        return $"$([MSBuild]::NormalizePath('$(MSBuildThisFileDirectory)', '{dependnecyProjectPath}'))";
    }

    XDocument CreateDirectoryBuildPropsXml() => new(
        new XElement(
            "Project",
            [
                CreateParentDirectoryBuildPropsImport(),
                this.CreateCommonProjectProperties(),
                CreateCommonProjectCompileItems(),
                CreateAllureResultsCleanItems(),
                ..CreateProjectReferencesXml(),
            ]
        )
    );

    static XElement CreateParentDirectoryBuildPropsImport() => new(
        "Import",
        new XAttribute(
            "Project",
            "$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))"
        ),
        new XAttribute(
            "Condition",
            "'' != $([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))"
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
        IEnumerable<ITaskItem2> sampleSources
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

    GeneratedFileSource GenerateDirectoryPackagesProps()
    {
        var directoryPackagesPropsPath
            = Path.Combine(this.SampleSolutionDir, "Directory.Packages.props");
        var directoryPackagesPropsXml = CreateDirectoryPackagesPropsXml();

        return GeneratedFileSource.FromXmlDocument(
            directoryPackagesPropsXml,
            directoryPackagesPropsPath,
            omitDeclaration: true
        );
    }

    GeneratedFileSource GenerateDirectoryBuildProps()
    {
        var directoryBuildPropsPath
            = Path.Combine(this.SampleSolutionDir, "Directory.Build.props");
        var directoryBuildPropsXml = this.CreateDirectoryBuildPropsXml();

        return GeneratedFileSource.FromXmlDocument(
            directoryBuildPropsXml,
            directoryBuildPropsPath,
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
}
