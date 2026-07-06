using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Allure.Build.SourceGenerators.Samples;

[Generator]
public sealed class AllureSampleRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<RegistrySource> sampleSourceStream
            = SetupGroupedSampleSourcesStream(context);

        context.RegisterSourceOutput(sampleSourceStream, GenerateSampleRegistry);
    }

    static IncrementalValuesProvider<RegistrySource> SetupGroupedSampleSourcesStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(ReadSampleMetadata)
            .Where(static (sample) => sample is not null)
            .Collect()
            .Select(GroupAndSortEntries!)
            .SelectMany(static (registries, _) => registries)
            .WithComparer(RegistrySourceComparer.Instance);

    static RegistryEntry? ReadSampleMetadata(
        (AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair,
        CancellationToken _
    )
    {
        var opts = pair.Right.GetOptions(pair.Left);

        if (opts.TryGetValue(Constants.SAMPLE_NAME_METADATA_NAME, out var sampleName)
            && opts.TryGetValue(Constants.REGISTRY_NAMESPACE_METADATA_NAME, out var registryNamespace)
            && opts.TryGetValue(Constants.PROJECT_FILE_PATH_METADATA_NAME, out var projectFilePath)
            && opts.TryGetValue(Constants.PROJECT_RELATIVE_PATH_METADATA_NAME, out var projectRelativePath)
            && opts.TryGetValue(Constants.RESULTS_DIRECTORY_METADATA_NAME, out var resultsDirectory))
        {
            return new RegistryEntry(
                RegistryNamespace: registryNamespace,
                SampleName: sampleName,
                SourcePath: pair.Left.Path,
                ProjectFilePath: projectFilePath,
                ProjectRelativePath: projectRelativePath,
                ResultsDirectory: resultsDirectory
            );
        }
        return null;
    }

    static ImmutableArray<RegistrySource> GroupAndSortEntries(
        ImmutableArray<RegistryEntry> registryEntries,
        CancellationToken _
    ) =>
        [
            .. registryEntries
                .GroupBy(
                    static (sample) => sample.RegistryNamespace,
                    static (registryNamespace, registrySamples) => new RegistrySource(
                        registryNamespace,
                        [
                            .. registrySamples
                                .OrderBy(static (sample) => sample.SampleName, StringComparer.Ordinal)
                        ]
                    ),
                    StringComparer.Ordinal
                )
                .OrderBy(static registry => registry.Namespace, StringComparer.Ordinal)
        ];

    static void GenerateSampleRegistry(
        SourceProductionContext productionContext,
        RegistrySource sampleRegistry
    )
    {
        var (registryNamespace, registryEntries) = sampleRegistry;
        var code = GetSampleRegistryCode(registryNamespace, registryEntries);
        var path = Path.Combine(registryNamespace, Constants.REGISTRY_FILENAME);
        productionContext.AddSource(path, code);
    }

    static string GetSampleRegistryCode(
        string registryNamespace,
        ImmutableArray<RegistryEntry> entries
    )
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace { registryNamespace };");
        sb.AppendLine();
        AppendRegistryClass(sb, registryNamespace, entries);
        return sb.ToString();
    }

    static void AppendRegistryClass(
        StringBuilder sb,
        string registryNamespace,
        ImmutableArray<RegistryEntry> entries
    )
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
        AppendRegistryEntryProperties(sb, registryNamespace, entries);
        sb.AppendLine("}");
    }

    static void AppendRegistryEntryProperties(
        StringBuilder sb,
        string registryNamespace,
        ImmutableArray<RegistryEntry> entries
    )
    {
        if (entries.Length > 0)
        {
            AppendRegistryEntryProperty(sb, registryNamespace, entries[0]);
        }

        foreach (var entry in entries.Skip(1))
        {
            sb.AppendLine();
            AppendRegistryEntryProperty(sb, registryNamespace, entry);
        }
    }

    static void AppendRegistryEntryProperty(
        StringBuilder sb,
        string registryNamespace,
        RegistryEntry entry
    )
    {
        var sampleName = entry.SampleName;
        var sourcePath = entry.SourcePath;
        var projectFilePath = entry.ProjectFilePath;
        var projectRelativePath = entry.ProjectRelativePath;
        var resultsDirectory = entry.ResultsDirectory;

        var resultsDirectoryName = Path.GetFileName(resultsDirectory);

        sb.Append(
            $$"""
                /// <summary>
                /// Source: <a href="file://{{ sourcePath }}">
                /// {{ Path.GetFileName(sourcePath) }}
                /// </a>
                /// </summary>
                /// <remarks>
                /// How to run: <c>dotnet test {{ projectRelativePath }}</c>.
                /// <br></br>
                /// Default results directory: <a href="file://{{ resultsDirectory }}">
                /// {{ resultsDirectoryName }}
                /// </a>
                /// </remarks>
                public static {{ Constants.REGISTRY_ENTRY_CLASSNAME_FULL }} {{ sampleName }} { get; }
                    = new(
                        TestingPlatform: {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_TESTING_PLATFORM }} switch
                        {
                            null or "" or {{ SymbolDisplay.FormatLiteral(Constants.TESTING_PLATFORM_VSTEST, true) }} =>
                                {{ Constants.TESTING_PLATFORM_VSTEST_FULL }},

                            {{ SymbolDisplay.FormatLiteral(Constants.TESTING_PLATFORM_MTP, true) }} =>
                                {{ Constants.TESTING_PLATFORM_MTP_FULL }},

                            var value => throw new {{ Constants.ARGUMENT_EXCEPTION_CLASSNAME_FULL }}(
                                $"The testing platform value '{value}' is not supported."
                            ),
                        },
                        RegistryId: "{{ registryNamespace }}",
                        SampleId: "{{ sampleName }}",
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
}
