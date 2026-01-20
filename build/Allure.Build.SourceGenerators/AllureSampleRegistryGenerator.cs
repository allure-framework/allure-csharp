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

using SampleProjectSources = ImmutableArray<(string, ImmutableArray<string>)>;

[Generator]
public class AllureSampleRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<SampleProjectSources> sampleSourceStream
            = SetupGroupedSampleSourcesStream(context);

        context.RegisterSourceOutput(sampleSourceStream, GenerateSampleRegistry);
    }

    static IncrementalValueProvider<SampleProjectSources> SetupGroupedSampleSourcesStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(PairWithSuffix)
            .Where(IsSampleSourceWellDefined)
            .Collect()
            .Select(GroupSampleSourcesBySuffix);

    static (string, string) PairWithSuffix(
        (AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair,
        CancellationToken _
    ) =>
        pair.Right
            .GetOptions(pair.Left)
            .TryGetValue(Constants.PROJECT_SUFFIX_METADATA_NAME, out var value)
                && SyntaxFacts.IsValidIdentifier(value)
                ? (value, pair.Left.Path)
                : ("", "");

    static bool IsSampleSourceWellDefined((string, string) pair) =>
        pair.Item1 is not "" && pair.Item2 is not "";

    static SampleProjectSources GroupSampleSourcesBySuffix(
        ImmutableArray<(string, string)> sampleSources,
        CancellationToken _
    ) =>
        [
            .. sampleSources
                .GroupBy(
                    (pair) => pair.Item1,
                    (pair) => pair.Item2,
                    (key, values) => (key, values.ToImmutableArray())
                )
        ];

    static void GenerateSampleRegistry(
        SourceProductionContext productionContext,
        SampleProjectSources sampleSources
    )
    {
        var code = GetSampleRegistryCode(sampleSources);

        productionContext.AddSource(Constants.REGISTRY_FILENAME, code);
    }

    static string GetSampleRegistryCode(SampleProjectSources sampleSources)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace { Constants.NAMESPACE_NAME };");
        sb.AppendLine();
        AppendRegistryClass(sb, sampleSources);
        return sb.ToString();
    }

    static void AppendRegistryClass(StringBuilder sb, SampleProjectSources sampleSources)
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
        AppendRegistryEntryProperties(sb, sampleSources);
        sb.AppendLine("}");
    }

    static void AppendRegistryEntryProperties(StringBuilder sb, SampleProjectSources sampleSources)
    {
        if (sampleSources.Length > 0)
        {
            var (suffix, files) = sampleSources[0];
            AppendRegistryEntryProperty(sb, suffix, files);
        }

        foreach (var (suffix, files) in sampleSources.Skip(1))
        {
            sb.AppendLine();
            AppendRegistryEntryProperty(sb, suffix, files);
        }
    }

    static void AppendRegistryEntryProperty(StringBuilder sb, string suffix, ImmutableArray<string> files)
    {
        var pathPrefix = files.Length == 1 ? files[0] : GetGreatestCommonPrefix(files);

        sb.Append(
            $$"""
                /// <summary>
                /// <a href="file://{{ pathPrefix }}">
                /// {{ pathPrefix }}
                /// </a>
                /// </summary>
                public static {{ Constants.REGISTRY_ENTRY_CLASSNAME_FULL }} {{ suffix }} { get; }
                    = new(
                        "{{ suffix }}",
                        global::System.IO.Path.Combine(
                            {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_SOLUTION_DIR }},
                            string.Format(
                                "{0}.{{ suffix }}",
                                {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_SOLUTION_NAME }}
                            ),
                            string.Format(
                                "{0}.{{ suffix }}.csproj",
                                {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_SOLUTION_NAME }}
                            )
                        ),
                        string.Format(
                            {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_RESULTS_DIRECTORY_FMT }},
                            "{{ suffix }}"
                        ),
                        {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_TARGET_FRAMEWORK }},
                        {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_CONFIGURATION }},
                        global::System.StringComparer.OrdinalIgnoreCase.Equals(
                            {{ Constants.MSBUILD_PROPS_CLASSNAME_FULL }}.{{ Constants.PROP_PRERUN_FLOW }},
                            "true"
                        )
                    );

            """
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
}
