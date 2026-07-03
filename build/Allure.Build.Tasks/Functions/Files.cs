using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.DataTypes;

namespace Allure.Build.Tasks.Functions;

public static class Files
{
    public static StringComparer FsComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    public static GeneratedFileSource NugetConfig(
        string localRepository,
        string cacheLocation,
        string solutionDir
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.NugetConfig(localRepository, cacheLocation),
            destinationPath: Path.Combine(solutionDir, "nuget.config")
        );

    public static GeneratedFileSource Solution(
        string solutionFilePath,
        IEnumerable<string> projects
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.Solution(
                sampleSolutionRelativeProjectPath: projects
                    .Select((p) => Path.GetRelativePath(solutionFilePath, p))
            ),
            destinationPath: solutionFilePath,
            omitDeclaration: true
        );

    public static GeneratedFileSource DirectorySolutionTargets(
        string solutionDir
    ) =>
        GeneratedFileSource.FromXmlDocument(
            XmlDefinitions.DirectorySolutionTarget,
            Path.Combine(solutionDir, "Directory.Solution.targets")
        );

    public static GeneratedFileSource DirectoryPackagesProps(
        string solutionDir,
        IEnumerable<(string name, string version)> packages
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.DirectoryPackagesProps(packages),
            destinationPath: Path.Combine(solutionDir, "Directory.Packages.props")
        );

    public static GeneratedFileSource DirectoryBuildProps(
        string solutionDir,
        string targetFrameworks,
        IEnumerable<string> imports,
        IEnumerable<string> packages,
        IEnumerable<string> projects,
        IEnumerable<string> analyzerProjects
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.DirectoryBuildProps(
                imports,
                targetFrameworks,
                packages,
                projects,
                analyzerProjects
            ),
            destinationPath: Path.Combine(solutionDir, "Directory.Build.props"),
            omitDeclaration: true
        );

    public static GeneratedFileSource DirectoryBuildTargets(
        string solutionDir,
        IEnumerable<string> imports
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.DirectoryBuildTargets(imports),
            destinationPath: Path.Combine(solutionDir, "Directory.Build.targets"),
            omitDeclaration: true
        );

    public static GeneratedFileSource Project(
        string projectDir,
        string projectName,
        IEnumerable<(string key, string value)> properties,
        IEnumerable<AllureSample> files
    ) =>
        GeneratedFileSource.FromXmlDocument(
            document: XmlDefinitions.Project(
                projectProperties: properties,
                groupedProjectItems: files.GroupBy(
                    static (file) => file.ItemType,
                    (file) => (
                        path: Path.GetRelativePath(projectDir, file.Path),
                        metadata: file.ItemMetadata
                    ),
                    static (itemType, items) => (itemType, items)
                )
            ),
            destinationPath: Path.Combine(projectDir, $"{projectName}.csproj"),
            omitDeclaration: true
        );

    public static string GetGreatestCommonPrefix(IEnumerable<string> paths)
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

    public static string GetOsMatchingPath(string path)
    {
        if (Path.DirectorySeparatorChar != '/')
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);
        }

        if (Path.DirectorySeparatorChar != '\\')
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
        }

        return path;
    }

    public static string RebasePath(string path, string basePath, string newBasePath) =>
        Path.GetRelativePath(newBasePath, Path.GetFullPath(path, basePath));
}
