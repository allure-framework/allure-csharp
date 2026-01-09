using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Allure.Build.SourceGenerators;

using AllureBuildPropertiesData = ImmutableArray<(string, string)>;

[Generator]
public class AllureMsbuildPropsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var allureMsbuildProps = SetupMsbuildPropsArrayStream(context);

        context.RegisterSourceOutput(allureMsbuildProps, GenerateAllureBuildProperties);
    }

    static IncrementalValueProvider<AllureBuildPropertiesData> SetupMsbuildPropsArrayStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context
            .AnalyzerConfigOptionsProvider
            .SelectMany(GetExportedAllureProperties)
            .Collect();

    static IEnumerable<string> GetAllurePropertyNames(AnalyzerConfigOptions options) =>
        options.TryGetValue(Constants.EDITOR_PROP_PROPERTY_NAMES, out var value)
            ? value.Split(':')
            : [];

    static IEnumerable<(string, string)> GetExportedAllureProperties(
        AnalyzerConfigOptionsProvider provider,
        CancellationToken _
    )
    {
        var options = provider.GlobalOptions;
        foreach (var propertyName in GetAllurePropertyNames(options))
        {
            var edConfPropertyName = $"build_property.{propertyName}";
            if (options.TryGetValue(edConfPropertyName, out var propertyValue))
            {
                yield return (propertyName, propertyValue);
            }
        }
    }

    static void GenerateAllureBuildProperties(
        SourceProductionContext productionContext,
        AllureBuildPropertiesData data
    )
    {
        var sourceCode = GetAllureBuildPropertiesSourceCode(data);
        productionContext.AddSource(Constants.MSBUILD_PROPS_FILENAME, sourceCode);
    }

    static string GetAllureBuildPropertiesSourceCode(AllureBuildPropertiesData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {Constants.NAMESPACE_NAME};");
        sb.AppendLine();
        sb.AppendLine($"internal static class {Constants.MSBUILD_PROPS_CLASSNAME}");
        sb.AppendLine("{");
        AppendClassProperties(sb, data);
        sb.AppendLine("}");

        return sb.ToString();
    }

    static void AppendClassProperties(StringBuilder sb, AllureBuildPropertiesData data)
    {
        if (data.Length > 0)
        {
            AppendOneClassProperty(sb, data[0]);
        }

        foreach (var rest in data.Skip(1))
        {
            sb.AppendLine();
            AppendOneClassProperty(sb, rest);
        }
    }

    static void AppendOneClassProperty(StringBuilder sb, (string, string) data)
    {
        var (key, value) = data;
        var valueCodeLiteral = SymbolDisplay.FormatLiteral(value, true);

        sb.Append(
            $$"""
                public static string {{ key }} { get; }
                    = global::System.Environment.GetEnvironmentVariable($"{{ key }}")
                        ?? {{ valueCodeLiteral }};

            """
        );
    }
}
