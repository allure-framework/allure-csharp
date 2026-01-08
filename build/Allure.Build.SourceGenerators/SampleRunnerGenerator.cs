using Microsoft.CodeAnalysis;

namespace Allure.Build.SourceGenerators;

// [Generator]
public class SampleRunnerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var rootNamespace = context
            .AnalyzerConfigOptionsProvider
            .Select(static (c, _) =>
                c.GlobalOptions.TryGetValue("build_property.RootNamespace", out var value)
                    ? value
                    : null);

        context.RegisterSourceOutput(rootNamespace, static (spc, data) =>
        {

        });
    }
}