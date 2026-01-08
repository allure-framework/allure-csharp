using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks;

public partial class GenerateSampleSolution : Task
{
    static readonly Regex projectSuffixPattern = MyRegex();

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
    public string SampleSolutionPath { get; set;}

    [Required]
    public string SampleTargetFrameworks { get; set; }

    [Required]
    public string ArtifactsPath { get; set; }

    [Required]
    public string LocalNugetRepository { get; set; }

    public override bool Execute()
    {
        this.CreateDirectoryBuildProps();
        this.CreateDirectoryPackagesProps();
        this.CreateNugetConfig();
        var projects = this.CreateProjects();
        this.CreateSlnx(projects);
        return true;
    }

    void CreateSlnx(IEnumerable<string> projects)
    {
        WriteXmlFile(
            this.SampleSolutionPath,
            new XDocument(
                new XElement(
                    "Solution",
                    projects.Select(p => new XElement(
                        "Project",
                        new XAttribute("Path", Path.Combine(p, $"{p}.csproj"))
                    ))
                )
            )
        );
    }

    IEnumerable<string> CreateProjects()
    {
        var groups = this.SampleSources.GroupBy(
            static (sample) => sample.GetMetadata("ProjectSuffix") ?? "",
            static (sample) => sample.ItemSpec);

        foreach (var sampleGroup in groups)
        {
            var projectSuffix = sampleGroup.Key;
            if (string.IsNullOrEmpty(projectSuffix))
            {
                foreach (var sample in sampleGroup)
                {
                    this.Log.LogWarning("Ignoring '{0}': no ProjectSuffix defined on the item.", sample);
                }
                continue;
            }

            if (!projectSuffixPattern.IsMatch(projectSuffix))
            {
                foreach (var sample in sampleGroup)
                {
                    this.Log.LogWarning(
                        "Ignoring '{0}': invalid ProjectSuffix '{1}' defined on the item. "
                            + "Expected a latin letter or an underscore followed by one or more "
                            + "latin letters, digits, or underscores.",
                        sample,
                        projectSuffix);
                }
                continue;
            }

            this.Log.LogMessage(
                MessageImportance.Low,
                "Creating '{0}' with {1} sample files",
                projectSuffix,
                sampleGroup.Count()
            );

            var sampleProjectName = $"{this.SampleSolutionName}.{projectSuffix}";
            var sampleProjectDir = Path.Combine(this.SampleSolutionDir, sampleProjectName);

            var greatestCommonPrefix = GetGreatestCommonPrefix(sampleGroup);
            if (greatestCommonPrefix is "")
            {
                this.Log.LogWarning(
                    "Ignoring {0}: the sample files [{1}] don't have a common prefix.",
                    projectSuffix,
                    string.Join(", ", sampleGroup.Select(static (sample) => $"'{sample}'")));
                continue;
            }

            this.Log.LogMessage(
                MessageImportance.Low,
                "The greatest common prefix of files in '{0}' is '{1}'",
                sampleProjectName,
                greatestCommonPrefix
            );

            WriteXmlFile(
                Path.Combine(sampleProjectDir, $"{sampleProjectName}.csproj"),
                new XDocument(
                    new XElement(
                        "Project",
                        new XAttribute("Sdk", "Microsoft.NET.Sdk")
                    )
                )
            );

            foreach (var sample in sampleGroup)
            {
                var relativeSampleFilePath = Path.GetRelativePath(greatestCommonPrefix, sample);
                if (relativeSampleFilePath.StartsWith($"..{Path.DirectorySeparatorChar}"))
                {
                    this.Log.LogWarning(
                        "Ignoring '{0}': the file's calculated destination '{1}' is outside of the sample project directory '{2}'",
                        sample,
                        relativeSampleFilePath,
                        sampleProjectDir);
                    continue;
                }
                File.Copy(sample, Path.Combine(sampleProjectDir, relativeSampleFilePath), true);
            }

            this.Log.LogMessage(MessageImportance.High, "Sample project '{0}' created", sampleProjectName);

            yield return sampleProjectName;

        }
    }

    static string GetGreatestCommonPrefix(IEnumerable<string> paths)
    {
        var first = paths.First();
        var rest = paths.Skip(1).ToList();

        for (var prefix = Path.GetDirectoryName(first); prefix is not null; prefix = Path.GetDirectoryName(prefix))
        {
            if (rest.All((otherPath) => otherPath.StartsWith(prefix) && otherPath.Length > prefix.Length && otherPath[prefix.Length] == Path.DirectorySeparatorChar))
            {
                return prefix;
            }
        }

        return "";
    }

    void CreateNugetConfig()
    {
        WriteXmlFile(
            Path.Combine(this.SampleSolutionDir, "nuget.config"),
            new XDocument(
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
            )
        );
    }

    void CreateDirectoryPackagesProps()
    {
        WriteXmlFile(
            Path.Combine(this.SampleSolutionDir, "Directory.Packages.props"),
            new XElement(
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
            )
        );
    }

    void CreateDirectoryBuildProps()
    {
        var packages = this.SamplePackageReferences.Where(spec =>
        {
            var optional = spec.GetMetadata("Optional");
            return string.IsNullOrEmpty(optional) || optional.ToLower() is not "true";
        }).Select(reference => new XElement(
            "PackageReference",
            new XAttribute("Include", reference.ItemSpec)
        ));

        var projects = this.SampleProjectReferences.Select(reference => new XElement(
            "ProjectReference",
            new XAttribute(
                "Include",
                $"$([MSBuild]::NormalizePath('$(MSBuildThisFileDirectory)', '{Path.GetRelativePath(this.SampleSolutionDir, reference.ItemSpec)}'))"
            )
        ));

        IEnumerable<XElement> references = [
            new XElement(
                "PropertyGroup",
                new XElement("TargetFrameworks", this.SampleTargetFrameworks),
                new XElement("OutputType", "Library"),
                new XElement("EnableDefaultItems", "false")
            ),
            new XElement(
                "ItemGroup",
                new XElement(
                    "Compile",
                    new XAttribute("Include", "**/*.cs")
                )
            ),
            packages.Any() ? new XElement("ItemGroup", packages) : null,
            projects.Any() ? new XElement("ItemGroup", projects) : null,
        ];

        WriteXmlFile(
            Path.Combine(this.SampleSolutionDir, "Directory.Build.props"),
            new XElement(
                "Project",
                [
                    new XElement(
                        "Import",
                        new XAttribute(
                            "Project",
                            "$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))"
                        ),
                        new XAttribute(
                            "Condition",
                            "'' != $([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))"
                        )
                    ),
                    ..references.Where(item => item is not null),
                ]
            )
        );
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

    [GeneratedRegex("[a-zA-Z_][a-zA-Z0-9_]*")]
    private static partial Regex MyRegex();
}
