using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
    public string ArtifactsPath { get; set; }

    [Required]
    public string LocalNugetRepository { get; set; }

    IEnumerable<ITaskItem> CommonPackageReferences =>
        this.SamplePackageReferences.Where(spec =>
        {
            var optional = spec.GetMetadata("Optional");
            return string.IsNullOrEmpty(optional) || optional.ToLower() is not "true";
        });

    public override bool Execute()
    {
        this.CreateDirectoryBuildProps();
        this.CreateDirectoryPackagesProps();
        this.CreateNugetConfig();
        var projects = this.GenerateProjects();
        this.CreateSlnx(projects);
        return true;
    }

    static XElement CreateSlnxXml(IEnumerable<string> projects) => new(
        "Solution",
        projects.Select(p => new XElement(
            "Project",
            new XAttribute("Path", Path.Combine(p, $"{p}.csproj"))
        ))
    );

    IEnumerable<string> GenerateProjects() =>
        this.SampleSources
            .GroupBy(
                static (sample) => sample.GetMetadata("ProjectSuffix") ?? "",
                static (sample) => sample.ItemSpec)
            .Select(this.GenerateProject)
            .Where(p => p is not null);

    string GenerateProject(IGrouping<string, string> projectSourcesGroup)
    {
        var projectSuffix = projectSourcesGroup.Key;
        if (string.IsNullOrEmpty(projectSuffix))
        {
            this.ShowItemsWithNoSuffix(projectSourcesGroup);
            return null;
        }

        if (!SyntaxFacts.IsValidIdentifier(projectSuffix))
        {
            this.ShowInvalidSuffixWarning(projectSourcesGroup, projectSuffix);
            return null;
        }

        var greatestCommonPrefix = GetGreatestCommonPrefix(projectSourcesGroup);
        if (greatestCommonPrefix is "")
        {
            this.ShowMissingCommonPrefixWarning(projectSourcesGroup, projectSuffix);
            return null;
        }

        this.ShowGreatestCommonPrefixMessage(projectSuffix, greatestCommonPrefix);

        return WriteProjectFiles(projectSourcesGroup, projectSuffix, greatestCommonPrefix);
    }

    string WriteProjectFiles(IGrouping<string, string> sampleGroup, string projectSuffix, string greatestCommonPrefix)
    {
        this.ShowProjectGeneratingMessage(sampleGroup, projectSuffix);

        var sampleProjectName = $"{this.SampleSolutionName}.{projectSuffix}";
        var sampleProjectDir = Path.Combine(this.SampleSolutionDir, sampleProjectName);

        CreateCsproj(sampleProjectName, sampleProjectDir);
        this.CopySampleSourceFiles(sampleGroup, greatestCommonPrefix, sampleProjectDir);

        this.ShowProjectCreatedMessage(sampleProjectName);

        return sampleProjectName;
    }

    void CopySampleSourceFiles(IGrouping<string, string> sampleGroup, string greatestCommonPrefix, string sampleProjectDir)
    {
        foreach (var sample in sampleGroup)
        {
            this.CopySampleSourceFile(sampleProjectDir, greatestCommonPrefix, sample);
        }
    }

    void ShowProjectCreatedMessage(string sampleProjectName)
    {
        this.Log.LogMessage(MessageImportance.Low, "Sample project '{0}' created", sampleProjectName);
    }

    void CopySampleSourceFile(string sampleProjectDir, string greatestCommonPrefix, string sample)
    {
        var relativeSampleFilePath = Path.GetRelativePath(greatestCommonPrefix, sample);
        if (relativeSampleFilePath.StartsWith($"..{Path.DirectorySeparatorChar}"))
        {
            this.ShowFileOutsideProjectWarning(sampleProjectDir, sample, relativeSampleFilePath);
        }
        else
        {
            File.Copy(sample, Path.Combine(sampleProjectDir, relativeSampleFilePath), true);
        }
    }

    void ShowFileOutsideProjectWarning(string sampleProjectDir, string sample, string relativeSampleFilePath)
    {
        this.Log.LogWarning(
            "Ignoring '{0}': the file's calculated destination '{1}' is outside of the "
                + "sample project directory '{2}'",
            sample,
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

    void ShowMissingCommonPrefixWarning(IGrouping<string, string> sampleGroup, string projectSuffix)
    {
        this.Log.LogWarning(
            "Ignoring {0}: the sample files [{1}] don't have a common prefix.",
            projectSuffix,
            string.Join(", ", sampleGroup.Select(static (sample) => $"'{sample}'"))
        );
    }

    void ShowProjectGeneratingMessage(IGrouping<string, string> sampleGroup, string projectSuffix)
    {
        this.Log.LogMessage(
            MessageImportance.Low,
            "Creating '{0}' with {1} sample files",
            projectSuffix,
            sampleGroup.Count()
        );
    }

    void ShowItemsWithNoSuffix(IGrouping<string, string> sampleGroup)
    {
        foreach (var sample in sampleGroup)
        {
            this.Log.LogWarning("Ignoring '{0}': no ProjectSuffix defined on the item.", sample);
        }
    }

    static XElement CreateCsprojXml() => new(
        "Project",
        new XAttribute("Sdk", "Microsoft.NET.Sdk")
    );

    void ShowInvalidSuffixWarning(IGrouping<string, string> sampleGroup, string projectSuffix)
    {
        this.Log.LogWarning(
            "Ignoring {0} sample file(s): invalid ProjectSuffix '{1}' defined on the items. "
                + "A project suffix must be a valid C# identifier. "
                + "Please, rename the corresponding file or folder, "
                + "or assign the value manually. For example:\n"
                + "  <ItemGroup>\n"
                + $"    <AllureSample Remove=\"./Samples/{projectSuffix}/**\" />\n"
                + $"    <AllureSample Include=\"./Samples/{projectSuffix}/**\" ProjectSuffix=\"ValidSuffix\" />\n"
                + "  </ItemGroup>\n"
                + "Here is the list of skipped files:\n{2}",
            sampleGroup.Count(),
            projectSuffix,
            string.Join(
                "\n",
                sampleGroup.Select(s => $"  - {s}")
            )
        );
    }

    static string GetGreatestCommonPrefix(IEnumerable<string> paths)
    {
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

    XDocument CreateNugetConfigXml() => new(
        new XDeclaration("1.0", "utf-8", null),
        new XElement(
            "configuration",
            new XElement(
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
            )
        )
    );

    XElement CreateDirectoryPackagesPropsXml() => new(
        "Project",
        new XElement(
            "PropertyGroup",
            new XElement("ManagePackageVersionsCentrally", "true")
        ),
        new XElement(
            "ItemGroup",
            this.SamplePackageReferences.Select(spec => new XElement(
                "PackageVersion",
                new XAttribute("Include", spec.ItemSpec),
                new XAttribute("Version", spec.GetMetadata("Version")))
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
        this.CommonPackageReferences.Select((reference) => new XElement(
            "PackageReference",
            new XAttribute("Include", reference.ItemSpec)
        ))
    );

    XElement CreateCommonProjectReferencesXml() => new(
        "ItemGroup",
        this.SampleProjectReferences.Select((reference) => new XElement(
            "ProjectReference",
            new XAttribute(
                "Include",
                this.ResolveDependencyProjectPath(reference)
            )
        ))
    );

    string ResolveDependencyProjectPath(ITaskItem dependencyProject)
    {
        var dependnecyProjectPath = Path.GetRelativePath(this.SampleSolutionDir, dependencyProject.ItemSpec);
        return $"$([MSBuild]::NormalizePath('$(MSBuildThisFileDirectory)', '{dependnecyProjectPath}'))";
    }

    XElement CreateDirectoryBuildPropsXml() => new(
        "Project",
        [
            CreateParentDirectoryBuildPropsImport(),
            this.CreateCommonProjectProperties(),
            CreateCommonProjectCompileItems(),
            CreateAllureResultsCleanItems(),
            ..CreateProjectReferencesXml(),
        ]
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
        new XElement("EnableDefaultItems", "false")
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

    void CreateSlnx(IEnumerable<string> projects)
    {
        var slnxXml = CreateSlnxXml(projects);
        WriteXmlFile(this.SampleSolutionPath, slnxXml);
    }

    static void CreateCsproj(string sampleProjectName, string sampleProjectDir)
    {
        var csprojXml = CreateCsprojXml();
        var csprojPath = Path.Combine(sampleProjectDir, $"{sampleProjectName}.csproj");

        WriteXmlFile(csprojPath, csprojXml);
    }

    void CreateNugetConfig()
    {
        var nugetConfigPath = Path.Combine(this.SampleSolutionDir, "nuget.config");
        var nugetConfigXml = CreateNugetConfigXml();

        WriteXmlFile(nugetConfigPath, nugetConfigXml);
    }

    void CreateDirectoryPackagesProps()
    {
        var directoryPackagesPropsPath = Path.Combine(this.SampleSolutionDir, "Directory.Packages.props");
        var directoryPackagesPropsXml = CreateDirectoryPackagesPropsXml();

        WriteXmlFile(directoryPackagesPropsPath, directoryPackagesPropsXml);
    }

    void CreateDirectoryBuildProps()
    {
        var directoryBuildPropsPath = Path.Combine(this.SampleSolutionDir, "Directory.Build.props");
        var directoryBuildPropsXml = this.CreateDirectoryBuildPropsXml();

        WriteXmlFile(directoryBuildPropsPath, directoryBuildPropsXml);
    }

    static void WriteXmlFile(string path, XNode node)
    {
        var fInfo = new FileInfo(path);
        var dInfo = fInfo.Directory;
        if (!dInfo.Exists)
        {
            dInfo.Create();
        }

        using var writer = XmlWriter.Create(path, new XmlWriterSettings
        {
            Indent = true,
        });
        node.WriteTo(writer);
    }
}
