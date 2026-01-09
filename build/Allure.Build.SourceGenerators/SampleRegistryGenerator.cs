using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.SourceGenerators;

[Generator]
public class SampleRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<string>> sampleProjectSuffixes = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(static (pair, _) =>
                pair.Right
                    .GetOptions(pair.Left)
                    .TryGetValue("build_metadata.AdditionalFiles.Allure_ProjectSuffix", out var value)
                        && SyntaxFacts.IsValidIdentifier(value)
                        ? value
                        : null)
            .Where(static (suffix) => !string.IsNullOrEmpty(suffix))
            .Collect()!;


        context.RegisterSourceOutput(sampleProjectSuffixes, static (spc, suffixes) =>
        {
            var code = GenerateSampleRegistryCode(suffixes);

            spc.AddSource("AllureSampleRegistry.g.cs", code);
        });
    }

    static string GenerateSampleRegistryCode(IEnumerable<string> sampleSuffixes)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace Allure.Testing;");
        sb.AppendLine();
        AppendRegistryEntryClass(sb);
        sb.AppendLine();
        AppendRegistryClass(sb, sampleSuffixes);
        return sb.ToString();
    }

    static void AppendRegistryEntryClass(StringBuilder sb)
    {
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Contains the data required to run a specific sample project and");
        sb.AppendLine("/// access its result files.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("/// <remarks>");
        sb.AppendLine("/// Use the instances exposed by");
        sb.AppendLine("/// <see cref=\"global::Allure.Testing.AllureSampleRegistry\"/>");
        sb.AppendLine("/// instead of creating your own.");
        sb.AppendLine("/// </remarks>");
        sb.AppendLine("internal record class AllureSampleRegistryEntry(");
        sb.AppendLine("    string Name,");
        sb.AppendLine("    string ProjectPath,");
        sb.AppendLine("    string DefaultResultsPath");
        sb.AppendLine(");");
    }

    static void AppendRegistryClass(StringBuilder sb, IEnumerable<string> suffixes)
    {
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Exposes a set of testing samples available to this project");
        sb.AppendLine("/// via a set of static properties.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("/// <remarks>");
        sb.AppendLine("/// Pass a selected sample to");
        sb.AppendLine("/// <see cref=\"global::Allure.Testing.AllureSampleRunner\"/> methods");
        sb.AppendLine("/// to run it and access the test results.");
        sb.AppendLine("/// </remarks>");
        sb.AppendLine("internal static class AllureSampleRegistry");
        sb.AppendLine("{");
        AppendRegistryEntryProperties(sb, suffixes);
        sb.AppendLine("}");
    }

    static void AppendRegistryEntryProperties(StringBuilder sb, IEnumerable<string> suffixes)
    {
        var first = suffixes.FirstOrDefault();
        if (first is not null)
        {
            AppendRegistryEntryProperty(sb, first);
        }

        foreach (var rest in suffixes.Skip(1))
        {
            sb.AppendLine();
            AppendRegistryEntryProperty(sb, rest);
        }
    }

    static void AppendRegistryEntryProperty(StringBuilder sb, string suffix)
    {
        sb.AppendFormat("    public static global::Allure.Testing.AllureSampleRegistryEntry {0} {{ get; }}", suffix);
        sb.AppendLine();
        sb.AppendLine("        = new(");
        sb.AppendFormat("            \"{0}\",", suffix);
        sb.AppendLine();
        sb.AppendLine("            global::System.IO.Path.Combine(");
        sb.AppendLine("                global::Allure.Testing.AllureBuildProperties.Allure_SampleSolutionDir,");
        sb.AppendLine("                string.Format(");
        sb.AppendFormat("                    \"{{0}}.{0}\",", suffix);
        sb.AppendLine();
        sb.AppendLine("                    global::Allure.Testing.AllureBuildProperties.Allure_SampleSolutionName");
        sb.AppendLine("                ),");
        sb.AppendLine("                string.Format(");
        sb.AppendFormat("                    \"{{0}}.{0}.csproj\",", suffix);
        sb.AppendLine();
        sb.AppendLine("                    global::Allure.Testing.AllureBuildProperties.Allure_SampleSolutionName");
        sb.AppendLine("                )");
        sb.AppendLine("            ),");
        sb.AppendLine("            string.Format(");
        sb.AppendLine("                global::Allure.Testing.AllureBuildProperties.Allure_SampleResultsDirectoryFormat,");
        sb.AppendFormat("                \"{0}\"", suffix);
        sb.AppendLine();
        sb.AppendLine("            )");
        sb.AppendLine("        );");
    }
}
