using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Allure.Build.Tasks.DataTypes;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Allure.Build.Tasks.Functions;

public static partial class Imports
{
    public static MsBuildImportFiles GetImportsOfReferencedProjects(
        IBuildEngine3 buildEngine,
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string testProjectDirectory,
        IEnumerable<ITaskItem2> projectReferences
    ) =>
        projectReferences
            .Select((r) => CollectDependencyImportFiles(
                buildEngine,
                log,
                sampleSolutionDir,
                Path.GetFullPath(r.EvaluatedIncludeEscaped, testProjectDirectory)
            ))
            .Aggregate(new MsBuildImportFiles([], []), (f, s) => f with
            {
                PropsFiles = [..f.PropsFiles, ..s.PropsFiles],
                TargetsFiles = [..f.TargetsFiles, ..s.TargetsFiles],
            });

    static MsBuildImportFiles CollectDependencyImportFiles(
        IBuildEngine3 buildEngine,
        TaskLoggingHelper log,
        string sampleSolutionDir,
        string projectFullPath
    )
    {
        var graph = new Microsoft.Build.Graph.ProjectGraph(projectFullPath);
        var projects = graph.ProjectNodesTopologicallySorted
            .Select(static (n) => n.ProjectInstance)
            .ToImmutableArray();
        var arr = projects
            .Select(static (n) => n.FullPath)
            .ToArray();

        var buildResults = buildEngine.BuildProjectFilesInParallel(
            projectFileNames: arr,
            targetNames: ["Allure_GetPackageFiles"],
            globalProperties: [.. Enumerable.Repeat<System.Collections.IDictionary>(null, arr.Length)],
            removeGlobalProperties: null,
            toolsVersion: [.. Enumerable.Repeat<string>(null, arr.Length)],
            returnTargetOutputs: true
        );

        if (buildResults is not { Result: true, TargetOutputsPerProject: var outputs })
        {
            log.LogWarning($"Could not resolve the set of MSBuild extension files for {projectFullPath}");
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
            let packagePaths = Files.GetOsMatchingPath(packageFile.GetMetadata("PackagePath"))
                    .Split(';')
                    .Select(static (p) => p.TrimStart(Path.DirectorySeparatorChar))
                    .Select((p) => p.Length == 0 || p.EndsWith(Path.DirectorySeparatorChar)
                        ? (p
                            + Files.GetOsMatchingPath(packageFile.GetMetadata("RecursiveDir"))
                            + packageFile.GetMetadata("Filename")
                            + packageFile.GetMetadata("Extension"))
                        : p)
                    .ToImmutableHashSet()
            let isProps = packagePaths.Intersect(expectedPropsFiles) is { Count: >0 }
            let isTargets = packagePaths.Intersect(expectedTargetsFiles) is { Count: >0 }
            where isProps || isTargets
            select (
                isProps: isProps,
                path: Files.RebasePath(
                    Files.GetOsMatchingPath(packageFile.ItemSpec),
                    project.Directory,
                    sampleSolutionDir
                )
            )).ToImmutableArray();

        return new (
            PropsFiles: packageFiles
                .Where(static (f) => f.isProps)
                .Select(static (f) => f.path)
                .Distinct(Files.FsComparer),
            TargetsFiles: packageFiles
                .Where(static (f) => !f.isProps)
                .Select(static (f) => f.path)
                .Distinct(Files.FsComparer)
        );
    }
}