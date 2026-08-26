using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Allure.Build.Tasks.Functions;

using ProjectFileData =
    (string itemType, IEnumerable<(string path, ImmutableArray<(string key, string value)> metadata)> items);

public static class XmlDefinitions
{
    public static XDocument DirectorySolutionTarget { get; }
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

    public static XDocument Solution(IEnumerable<string> sampleSolutionRelativeProjectPath) => new(
        new XElement(
            "Solution",
            sampleSolutionRelativeProjectPath.Select(static (file) => new XElement(
                "Project",
                new XAttribute("Path", file)
            ))
        )
    );

    public static XDocument Project(
        IEnumerable<(string key, string value)> projectProperties,
        IEnumerable<ProjectFileData> groupedProjectItems
    ) => new(
        new XElement(
            "Project",
            (object[])[
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                ..Collections.MapNotEmpty(projectProperties, static (props) => MsBuild.GetPropertyGroup(props)),
                ..groupedProjectItems
                    .Select(static (group) => MsBuild.GetItemGroup(
                        group.items,
                        group.itemType,
                        static (f) => [("Include", f.path), ..f.metadata]
                    )
                ),
            ]
        )
    );

    public static XDocument NugetConfig(string localRepository, string packageCacheLocation) =>
        new(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "configuration",
                (object[])[
                    ..Collections.MapNotEmpty(packageCacheLocation, static (path) => new XElement(
                        "config",
                        new XElement(
                            "add",
                            new XAttribute("key", "globalPackagesFolder"),
                            new XAttribute("value", path)
                        )
                    )),
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
                            new XAttribute("value", localRepository)
                        )
                    ),
                ]
            )
        );

    public static XDocument DirectoryBuildProps(
        IEnumerable<string> imports,
        string targetFrameworks,
        string outputType,
        IEnumerable<(string name, string value)> properties,
        IEnumerable<string> packages,
        IEnumerable<string> projects,
        IEnumerable<string> analyzerProjects
    ) => new(
        new XElement(
            "Project",
            [
                MsBuild.GetImportFromParentDir("Directory.Build.props"),
                ..imports.Select(MsBuild.GetImport),
                MsBuild.GetPropertyGroup([
                    ("TargetFrameworks", targetFrameworks),
                    ("OutputType", outputType),
                    ("EnableDefaultItems", "false"),
                    ("IsTestProject", "true"),
                    ("AspectInjector_Enabled", "false"),
                    ..properties,
                ]),
                MsBuild.GetItemGroup("Compile", [("Include", Path.Combine("**", "*.cs"))]),
                MsBuild.GetItemGroup("Clean", [
                    ("Include", Path.Combine("$(TargetDir)allure-results", "**", "*"))
                ]),
                ..Collections.MapNotEmpty(packages, static (packages) => MsBuild.GetItemGroup(
                    packages,
                    "PackageReference",
                    static (p) => [("Include", p)]
                )),
                ..Collections.MapNotEmpty(projects, static (projects) => MsBuild.GetItemGroup(
                    projects,
                    "ProjectReference",
                    static (p) => [("Include", MsBuild.NormalizeToThisFileDirectory(p))]
                )),
                ..Collections.MapNotEmpty(analyzerProjects, static (projects) => MsBuild.GetItemGroup(
                    projects,
                    "ProjectReference",
                    static (p) => [
                        ("Include", MsBuild.NormalizeToThisFileDirectory(p)),
                        ("ReferenceOutputAssembly", "false"),
                        ("OutputItemType", "Analyzer"),
                        ("PrivateAssets", "All")
                    ]
                )),
            ]
        )
    );

    public static XDocument DirectoryBuildTargets(IEnumerable<string> imports) => new(
        new XElement(
            "Project",
            [
                MsBuild.GetImportFromParentDir("Directory.Build.targets"),
                ..imports.Select(MsBuild.GetImport),
            ]
        )
    );

    public static XDocument DirectoryPackagesProps(
        IEnumerable<(string name, string version)> packages
    ) => new(
        new XElement(
            "Project",
            [
                MsBuild.GetPropertyGroup(("ManagePackageVersionsCentrally", "true")),
                MsBuild.GetItemGroup(packages, "PackageVersion", static (spec) => [
                    ("Include", spec.name),
                    ("Version", spec.version),
                ]),
            ]
        )
    );
}