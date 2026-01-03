using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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
    public string SampleSolutionDir { get; set; }

    [Required]
    public string SampleSolutionName { get; set; }

    [Required]
    public string SampleTargetFrameworks { get; set; }

    [Required]
    public string ArtifactsPath { get; set; }

    [Required]
    public string LocalNugetRepository { get; set; }

    [Output]
    public string SampleSolutionPath { get; set; }

    public override bool Execute()
    {
        this.CreateDirectoryBuildProps();
        this.CreateDirectoryPackagesProps();
        this.CreateNugetConfig();
        var projects = this.CreateProjects();
        this.SampleSolutionPath = this.CreateSlnx(projects);
        return true;
    }

    string CreateSlnx(IEnumerable<string> projects)
    {
        return WriteXmlFile(
            this.SampleSolutionDir,
            $"{this.SampleSolutionName}.slnx",
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
        foreach (var sample in this.SampleSources)
        {
            var sampleFileName = Path.GetFileName(sample.ItemSpec);
            var sampleName = Path.GetFileNameWithoutExtension(sample.ItemSpec);
            var sampleProjectName = $"{this.SampleSolutionName}.{sampleName}";
            var sampleProjectDir = Path.Combine(this.SampleSolutionDir, sampleProjectName);

            WriteXmlFile(
                sampleProjectDir,
                $"{sampleProjectName}.csproj",
                new XDocument(
                    new XElement(
                        "Project",
                        new XAttribute("Sdk", "Microsoft.NET.Sdk")
                    )
                )
            );

            File.Copy(sample.ItemSpec, Path.Combine(sampleProjectDir, sampleFileName), true);

            yield return sampleProjectName;

            Console.WriteLine($"Found sample {sampleProjectName}");
        }
    }

    void CreateNugetConfig()
    {
        WriteXmlFile(
            this.SampleSolutionDir,
            "nuget.config",
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
            this.SampleSolutionDir,
            "Directory.Packages.props",
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
            this.SampleSolutionDir,
            "Directory.Build.props",
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

    static string WriteXmlFile(string directory, string name, XNode node)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var path = Path.Combine(directory, name);

        using var writer = XmlWriter.Create(path, new XmlWriterSettings
        {
            Indent = true,
        });
        node.WriteTo(writer);

        return path;
    }
}
