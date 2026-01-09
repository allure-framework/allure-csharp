using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Allure.Build.SourceGenerators;

[Generator]
public class AllureMsbuildPropsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var allureMsbuildProps = context
            .AnalyzerConfigOptionsProvider
            .SelectMany(static (c, _) =>
                c.GlobalOptions.TryGetValue("build_property.Allure_PropertyNames", out var exports)
                    ? exports
                        .Split(':')
                        .Select(k =>
                            c.GlobalOptions.TryGetValue($"build_property.{k}", out var value)
                                ? (k, value)
                                : (k, null))
                        .Where(static v => v.value is not null) as IEnumerable<(string, string)>
                    : [])
            .Collect();

        context.RegisterSourceOutput(
            allureMsbuildProps,
            static (spc, data) =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("namespace Allure.Testing;");
                sb.AppendLine();
                sb.AppendLine("internal static class AllureBuildProperties");
                sb.AppendLine("{");
                bool emitNewLine = false;
                foreach (var (key, value) in data)
                {
                    if (emitNewLine)
                    {
                        sb.AppendLine();
                    }
                    sb.AppendFormat("    public static string {0} {{ get; }}", key);
                    sb.AppendLine();
                    sb.AppendFormat("        = global::System.Environment.GetEnvironmentVariable($\"{0}\")", key);
                    sb.AppendLine();
                    sb.AppendFormat("            ?? {0};", SymbolDisplay.FormatLiteral(value, true));
                    sb.AppendLine();
                }
                sb.AppendLine("}");


                spc.AddSource("AllureBuildProperties.g.cs", sb.ToString());
            }
        );
    }
}
