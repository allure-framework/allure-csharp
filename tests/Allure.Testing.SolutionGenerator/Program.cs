using System.CommandLine;
using System.CommandLine.Parsing;
using System.Xml;
using System.Xml.Linq;
using NuGet.Frameworks;
using NuGet.Versioning;

Option<string> assemblyOption = new("--assembly")
{
    Description = "A full path to the test assembly.",
    Required = true,
};
assemblyOption.Validators.Add((result) =>
{
    var assemblyPath = result.Tokens.Single().Value;
    if (!File.Exists(assemblyPath))
    {
        result.AddError($"The assembly '{assemblyPath}' doesn't exist. Make sure the test project was built successfully.");
    }
});

Option<string?> frameworkOption = new("--framework")
{
    Description = "The target framework of the sample projects in the generated solution.",
    Required = true,
};
frameworkOption.Validators.Add((result) =>
{
    var value = result.Tokens.Single().Value;
    if (NuGetFramework.Parse(value).IsUnsupported)
    {
        result.AddError($"The target framework moniker {value} is not supported.");
    }
});

Option<List<Dependency>> dependenciesOption = new("--dependencies")
{
    Description = "A list of dependencies shared by all samples. Each dependency must be in format <package>:<version>.",
    AllowMultipleArgumentsPerToken = true,
    CustomParser = ResolveDependencies,
    DefaultValueFactory = ResolveDependencies,
};

Option<List<PackageDependency>> optionalDependenciesOption = new("--optional-dependencies")
{
    Description = "A list of packages some samples rely on. Each dependency must be in format <package>:<version>.",
    AllowMultipleArgumentsPerToken = true,
    CustomParser = ResolvePackageDependencies,
    DefaultValueFactory = ResolvePackageDependencies,
};

RootCommand rootCommand = new("Samples solution generator for Allure.NET test projects.")
{
    assemblyOption,
    frameworkOption,
    dependenciesOption,
    optionalDependenciesOption,
};

rootCommand.SetAction(parseResult =>
{
    string assemblyPath = parseResult.GetValue(assemblyOption)!;
    string framework = parseResult.GetValue(frameworkOption)!;
    List<Dependency> dependencies = parseResult.GetValue(dependenciesOption)!;
    List<PackageDependency> optionalDependencies = parseResult.GetValue(optionalDependenciesOption)!;

    var outputDir = Path.GetDirectoryName(assemblyPath)!;
    var sampleSrcDir = Path.Combine(outputDir, "Samples");
    var projectBinDir = Path.GetDirectoryName(outputDir)!;
    var projectName = Path.GetFileName(projectBinDir);
    var binDir = Path.GetDirectoryName(projectBinDir)!;
    var artifactsDir = Path.GetDirectoryName(binDir)!;
    var samplesDir = Path.Combine(artifactsDir, "samples");
    var projectSamplesDir = Path.Combine(samplesDir, projectName);
    var solutionDir = Path.Combine(projectSamplesDir, framework);
    var localNugetRepository = Path.Combine(artifactsDir, "package", "release");

    var paths = new Paths(solutionDir, artifactsDir);

    var solutionName = $"{projectName}.Samples";

    CreateDirectoryBuildProps(paths, framework, dependencies);
    CreateDirectoryPackagesProps(paths, [..dependencies.OfType<PackageDependency>(), ..optionalDependencies]);
    CreateNugetConfig(paths, localNugetRepository);

    List<string> projects = [];

    foreach (var sample in new DirectoryInfo(sampleSrcDir).EnumerateFileSystemInfos())
    {
        var sampleName = sample switch
        {
            FileInfo => Path.GetFileNameWithoutExtension(sample.Name),
            _ => sample.Name,
        };
        var sampleProjectName = $"{solutionName}.{sampleName}";
        var sampleProjectDir = Path.Combine(solutionDir, sampleProjectName);

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

        if (sample is FileInfo sampleFile)
        {
            File.Copy(sample.FullName, Path.Combine(sampleProjectDir, sample.Name), true);
        }

        projects.Add(sampleProjectName);

        Console.WriteLine($"Found sample {sampleProjectName}");
    }

    CreateSlnx(solutionDir, solutionName, projects);
    return 0;
});

ParseResult parseResult = rootCommand.Parse(args);
return parseResult.Invoke();

static List<Dependency> ResolveDependencies(ArgumentResult result)
{
    return result.Tokens.Select(token =>
    {
        var value = token.Value;
        return value.EndsWith(".csproj")
            ? ResolveProjectDependency(result, value) as Dependency
            : ResolvePackageDependency(result, value);
    }).Where(item => item is not null).ToList()!;
}

static List<PackageDependency> ResolvePackageDependencies(ArgumentResult result)
{
    return result.Tokens.Select(token => ResolvePackageDependency(result, token.Value))
        .Where(item => item is not null).ToList()!;
}

static PackageDependency? ResolvePackageDependency(ArgumentResult result, string specifier)
{
    var indexOfColon = specifier.IndexOf(':');
    if (indexOfColon == -1)
    {
        result.AddError($"Invalid dependency version specifier '{specifier}'. Expected <package>:<version>.");
        return null;
    }

    var package = specifier[..indexOfColon];
    if (string.IsNullOrEmpty(package))
    {
        result.AddError($"Empty package name in dependency version specifier '{specifier}'.");
        return null;
    }

    var versionString = specifier[(indexOfColon + 1)..];
    if (!NuGetVersion.TryParseStrict(versionString, out NuGetVersion? version))
    {
        result.AddError($"Invalid NuGet version in dependency version specifier '{specifier}'.");
        return null;
    }

    return new PackageDependency(package, version);
}

static ProjectDependency? ResolveProjectDependency(ArgumentResult result, string value)
{
    var fullPath = Path.GetFullPath(value);
    if (!File.Exists(fullPath))
    {
        result.AddError($"Project file at '{fullPath}' doesn't exist.");
        return null;
    }

    return new(fullPath);
}

static void CreateNugetConfig(Paths paths, string localNugetRepositoryPath)
{
    WriteXmlFile(
        paths.SolutionDir,
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
                        new XAttribute("value", localNugetRepositoryPath)
                    )
                )
            )
        )
    );
}

static void CreateSlnx(string solutionDir, string solutionName, List<string> projectNames)
{
    WriteXmlFile(
        solutionDir,
        $"{solutionName}.slnx",
        new XDocument(
            new XElement(
                "Solution",
                projectNames.Select(p => new XElement(
                    "Project",
                    new XAttribute("Path", Path.Combine(p, $"{p}.csproj"))
                ))
            )
        )
    );
}

static void CreateDirectoryBuildProps(Paths paths, string framework, List<Dependency> dependencies)
{
    var packages = dependencies.OfType<PackageDependency>().Select(dep => new XElement(
        "PackageReference",
        new XAttribute("Include", dep.Package)
    ));
    var projects = dependencies.OfType<ProjectDependency>().Select(dep => new XElement(
        "ProjectReference",
        new XAttribute(
            "Include",
            $"$([System.IO.Path]::Combine('$(MSBuildThisFileDirectory)', '{Path.GetRelativePath(paths.SolutionDir, dep.Path)}'))"
        )
    ));
    IEnumerable<XElement?> references = [

        new XElement(
            "PropertyGroup",
            new XElement("TargetFramework", framework),
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
        paths.SolutionDir,
        "Directory.Build.props",
        new XDocument(
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
        )
    );
}

static void CreateDirectoryPackagesProps(Paths paths, List<PackageDependency> versions)
{
    WriteXmlFile(
        paths.SolutionDir,
        "Directory.Packages.props",
        new XDocument(
            new XElement(
                "Project",
                new XElement(
                    "PropertyGroup",
                    new XElement("ManagePackageVersionsCentrally", "true")
                ),
                new XElement(
                    "ItemGroup",
                    versions.Select(dep => new XElement(
                        "PackageVersion",
                        new XAttribute("Include", dep.Package),
                        new XAttribute("Version", dep.Version)))
                )
            )
        )
    );
}

static void WriteXmlFile(string directory, string name, XDocument document)
{
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }

    using var writer = XmlWriter.Create(Path.Combine(directory, name), new XmlWriterSettings
    {
        Indent = true,
    });
    document.WriteTo(writer);
}

record class Dependency;

record class PackageDependency(string Package, NuGetVersion Version) : Dependency;

record class ProjectDependency(string Path) : Dependency;

record class Paths(
    string SolutionDir,
    string ArtifactsDir
);
