using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Allure.Build.SourceGenerators;

using RegistrySample = (string, ImmutableArray<AllureSample>);
using RegistrySamples
    = ImmutableArray<(string, ImmutableArray<AllureSample>)>;
using SampleProjectRegistries
    = ImmutableArray<(string, ImmutableArray<(string, ImmutableArray<AllureSample>)>)>;

[Generator]
public class AllureSampleRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<SampleProjectRegistries> sampleSourceStream
            = SetupGroupedSampleSourcesStream(context);

        context.RegisterSourceOutput(sampleSourceStream, GenerateSampleRegistries);
    }

    static IncrementalValueProvider<SampleProjectRegistries> SetupGroupedSampleSourcesStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(ReadSampleMetadata)
            .Where(static (sample) => sample is not null)
            .Collect()
            .Select(GroupSampleSourcesByRegistry!);

    static AllureSample? ReadSampleMetadata(
        (AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair,
        CancellationToken _
    )
    {
        var opts = pair.Right.GetOptions(pair.Left);

        if (opts.TryGetValue(Constants.SAMPLE_NAME_METADATA_NAME, out var sampleName)
            && SyntaxFacts.IsValidIdentifier(sampleName)
            && opts.TryGetValue(Constants.REGISTRY_NAMESPACE_METADATA_NAME, out var registryNamespace)
            && IsValidNamespace(registryNamespace)
            && opts.TryGetValue(Constants.PROJECT_FILE_PATH_METADATA_NAME, out var projectFilePath)
            && !string.IsNullOrEmpty(projectFilePath)
            && opts.TryGetValue(Constants.PROJECT_RELATIVE_PATH_METADATA_NAME, out var projectRelativePath)
            && !string.IsNullOrEmpty(projectRelativePath)
            && opts.TryGetValue(Constants.RESULTS_DIRECTORY_METADATA_NAME, out var resultsDirectory)
            && !string.IsNullOrEmpty(resultsDirectory))
        {
            return new AllureSample(
                Path: pair.Left.Path,
                SampleName: sampleName,
                RegistryNamespace: registryNamespace,
                ProjectFilePath: projectFilePath,
                ProjectRelativePath: projectRelativePath,
                ResultsDirectory: resultsDirectory
            );
        }
        return null;
    }

    static bool IsValidNamespace(string ns)
        => ns.Split('.').All(SyntaxFacts.IsValidIdentifier);

    static SampleProjectRegistries GroupSampleSourcesByRegistry(
        ImmutableArray<AllureSample> sampleSources,
        CancellationToken _
    ) =>
        [
            .. sampleSources
                .GroupBy(
                    static (sample) => sample.RegistryNamespace,
                    static (registryNamespace, registrySamples) => (
                        registryNamespace,
                        registrySamples.GroupBy(
                            static (sample) => sample.SampleName,
                            static (name, sampleFiles) => (name, sampleFiles.ToImmutableArray())
                        ).ToImmutableArray()
                    )
                )
        ];

    static void GenerateSampleRegistries(
        SourceProductionContext productionContext,
        SampleProjectRegistries sampleRegistries
    )
    {
        foreach (var registry in sampleRegistries)
        {
            GenerateSampleRegistry(productionContext, registry);
        }
    }

    static void GenerateSampleRegistry(
        SourceProductionContext productionContext,
        (string, RegistrySamples) registry
    )
    {
        var (registryNamespace, registrySamples) = registry;
        var code = GetSampleRegistryCode(registryNamespace, registrySamples);
        var path = Path.Combine(registryNamespace, Constants.REGISTRY_FILENAME);
        productionContext.AddSource(path, code);
    }

    static string GetSampleRegistryCode(string registryNamespace, RegistrySamples samples)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace { registryNamespace };");
        sb.AppendLine();
        AppendRegistryClass(sb, registryNamespace, samples);
        return sb.ToString();
    }

    static void AppendRegistryClass(StringBuilder sb, string registryNamespace, RegistrySamples samples)
    {
        sb.Append(
            $$"""
            /// <summary>
            /// Exposes a set of testing samples available to this project via a set of static properties.
            /// </summary>
            /// <remarks>
            /// Pass a selected sample to <see cref="{{ Constants.RUNNER_CLASSNAME_FULL }}"/> methods
            /// to run it and access the test results.
            /// </remarks>
            internal static class {{ Constants.REGISTRY_CLASSNAME }}
            {

            """
        );
        AppendRegistryEntryProperties(sb, registryNamespace, samples);
        sb.AppendLine("}");
    }

    static void AppendRegistryEntryProperties(StringBuilder sb, string registryNamespace, RegistrySamples samples)
    {
        if (samples.Length > 0)
        {
            AppendRegistryEntryProperty(sb, registryNamespace, samples[0]);
        }

        foreach (var pair in samples.Skip(1))
        {
            sb.AppendLine();
            AppendRegistryEntryProperty(sb, registryNamespace, pair);
        }
    }

    static void AppendRegistryEntryProperty(StringBuilder sb, string registryNamespace, RegistrySample sample)
    {
        var (name, files) = sample;

        var projectFilePaths = files.Select(static (s) => s.ProjectFilePath).ToImmutableHashSet();
        if (projectFilePaths.Count != 1)
        {
            return;
        }
        var projectFilePath = projectFilePaths.First();

        var projectRelativePaths = files.Select(static (s) => s.ProjectRelativePath).ToImmutableHashSet();
        if (projectRelativePaths.Count != 1)
        {
            return;
        }
        var projectRelativePath = projectRelativePaths.First();

        var resultDirectories = files.Select(static (s) => s.ResultsDirectory).ToImmutableHashSet();
        if (resultDirectories.Count != 1)
        {
            return;
        }
        var resultsDirectory = resultDirectories.First();
        var resultsDirectoryName = Path.GetFileName(resultsDirectory);

        var pathPrefix = files.Length == 1 ? files[0].Path : GetGreatestCommonPrefix(files);

        sb.Append(
            $$"""
                /// <summary>
                /// Source: <a href="file://{{ pathPrefix }}">
                /// {{ Path.GetFileName(pathPrefix) }}
                /// </a>
                /// </summary>
                /// <remarks>
                /// How to run: <c>dotnet test {{ projectRelativePath }}</c>.
                /// <br></br>
                /// Default results directory: <a href="file://{{ resultsDirectory }}">
                /// {{ resultsDirectoryName }}
                /// </a>
                /// </remarks>
                public static {{ Constants.REGISTRY_ENTRY_CLASSNAME_FULL }} {{ name }} { get; }
                    = new(
                        RegistryId: "{{ registryNamespace }}",
                        SampleId: "{{ name }}",
                        ProjectFilePath: {{ SymbolDisplay.FormatLiteral(projectFilePath, true) }},
                        DefaultResultsPath: {{ SymbolDisplay.FormatLiteral(resultsDirectory, true) }},
                        TargetFramework: {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_TARGET_FRAMEWORK }},
                        BuildConfiguration: {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_CONFIGURATION }},
                        IsPreRunFlow: global::System.StringComparer.OrdinalIgnoreCase.Equals(
                            {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_PRERUN_FLOW }},
                            "true"
                        )
                    );

            """
        );
    }

    static string GetGreatestCommonPrefix(IEnumerable<AllureSample> sampleFiles)
    {
        var paths = sampleFiles.Select(static (s) => s.Path);
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
}
