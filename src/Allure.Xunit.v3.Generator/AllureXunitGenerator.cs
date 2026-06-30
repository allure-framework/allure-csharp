using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Allure.Xunit.Generator;


[Generator]
public class AllureXunitGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var allureXunitOptions = SetupGeneratorOptionsStream(context);
        var hasTestHook = SetupHasAllureXunitAttributeStream(context);
        var selfRegistrations = SetupAddSelfRegisteredExtensionsStream(context);
        var allureIdMethods = SetupAllureIdMethodsStream(context);

        var generationInput = allureXunitOptions
            .Combine(hasTestHook)
            .Combine(selfRegistrations)
            .Combine(allureIdMethods);

        context.RegisterSourceOutput(generationInput, GenerateAllureXunitSources);
    }

    static IncrementalValueProvider<AllureXunitGeneratorOptions> SetupGeneratorOptionsStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context
            .AnalyzerConfigOptionsProvider
            .Select(ReadGeneratorProperties);

    static IncrementalValueProvider<bool> SetupHasAllureXunitAttributeStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context
            .CompilationProvider
            .Select(HasAllureXunitAttribute);

    static IncrementalValueProvider<ImmutableArray<string>> SetupAddSelfRegisteredExtensionsStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.SyntaxProvider.CreateSyntaxProvider(
            predicate: IsSelfRegisteredExtensionsCandidate,
            transform: ToSelfRegistrationMethodExpression
        )
        .Where(static (expression) => expression is not null)
        .Collect()!;

    static IncrementalValueProvider<ImmutableArray<(int, ImmutableArray<string>)>> SetupAllureIdMethodsStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.SyntaxProvider.CreateSyntaxProvider(
            predicate: IsMethodWithAttributes,
            transform: ToAllureIdMethodKeyValuePair
        )
        .Where(static (kv) => kv is not null)
        .Select(static (kv, _) => kv!.Value)
        .Collect()
        .Select(
            static (arr, _) =>
                arr.GroupBy(
                    static (pair) => pair.Item1,
                    static (pair) => pair.Item2,
                    static (k, v) => (k, v.ToImmutableArray())
                )
                .ToImmutableArray()
        );

    static bool IsMethodWithAttributes(SyntaxNode node, CancellationToken _) =>
        node is MethodDeclarationSyntax methodDeclaration
            && methodDeclaration.AttributeLists.Count > 0;

    static (int, string)? ToAllureIdMethodKeyValuePair(GeneratorSyntaxContext ctx, CancellationToken _)
    {
        var semanticModel = ctx.SemanticModel;
        var methodDeclaration = (MethodDeclarationSyntax)ctx.Node;

        var allureIdAttributeType = semanticModel.Compilation.GetTypeByMetadataName(Types.AllureIdAttribute);
        if (allureIdAttributeType is null)
        {
            return null;
        }

        foreach (var attributeList in methodDeclaration.AttributeLists)
        {
            foreach (var attributeApplicationSyntax in attributeList.Attributes)
            {
                if (semanticModel.GetSymbolInfo(attributeApplicationSyntax).Symbol is not IMethodSymbol attributeApplication)
                {
                    continue;
                }

                var attributeType = attributeApplication.ContainingType;

                if (SymbolEqualityComparer.Default.Equals(attributeType, allureIdAttributeType))
                {
                    var method = semanticModel.GetDeclaredSymbol(methodDeclaration);
                    if (method is not null)
                    {
                        foreach (var attributeData in method.GetAttributes())
                        {
                            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, allureIdAttributeType))
                            {
                                continue;
                            }

                            var allureIdObj = attributeData.ConstructorArguments[0].Value;
                            if (allureIdObj is int allureId)
                            {
                                var typeFullName = method.ContainingType.ToDisplayString(FullyQualifiedNoTypeParameters);
                                var methodName = method.Name;
                                return (allureId, $"{typeFullName}.{methodName}");
                            }
                        }
                    }

                    return null;
                }
            }
        }

        return null;
    }

    static AllureXunitGeneratorOptions ReadGeneratorProperties(
        AnalyzerConfigOptionsProvider provider,
        CancellationToken _
    )
    {
        var opts = provider.GlobalOptions;
        return new AllureXunitGeneratorOptions(
            GenerateEntryPoint: ReadBooleanProperty(opts, "build_property.Allure_GenerateXunitEntryPoint"),
            ApplyAttribute: ReadBooleanProperty(opts, "build_property.Allure_ApplyXunitAttribute")
        );

    }

    static bool ReadBooleanProperty(AnalyzerConfigOptions opts, string key) =>
        !opts.TryGetValue(key, out var generateEntryPoint)
            || generateEntryPoint.Equals("true", System.StringComparison.InvariantCultureIgnoreCase);

    static bool IsSelfRegisteredExtensionsCandidate(SyntaxNode node, CancellationToken _)
    {
        if (node is not ClassDeclarationSyntax cds || cds.Identifier.ValueText != "SelfRegisteredExtensions")
        {
            return false;
        }

        foreach (var member in cds.Members)
        {
            if (IsAddSelfRegisteredExtensionsDeclaration(member))
            {
                return true;
            }
        }

        return false;
    }

    static string? ToSelfRegistrationMethodExpression(GeneratorSyntaxContext ctx, CancellationToken token)
    {
        var semanticModel = ctx.SemanticModel;
        var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;

        var expectedReceiverType = semanticModel.Compilation.GetTypeByMetadataName(Types.ITestApplicationBuilder);
        if (expectedReceiverType is null)
        {
            return null;
        }

        foreach (var member in classDeclarationSyntax.Members)
        {
            if (!IsAddSelfRegisteredExtensionsDeclaration(member))
            {
                continue;
            }

            var methodDeclaration = (MethodDeclarationSyntax)member;

            var method = semanticModel.GetDeclaredSymbol(methodDeclaration, token);
            if (!IsAddSelfRegisteredExtensions(method))
            {
                continue;
            }

            var receiverParameter = method.Parameters[0];
            if (!SymbolEqualityComparer.Default.Equals(receiverParameter.Type, expectedReceiverType))
            {
                continue;
            }

            return GetMethodGroupExpression(method);
        }

        return null;
    }

    static bool IsAddSelfRegisteredExtensionsDeclaration(MemberDeclarationSyntax memberDeclaration) =>
        memberDeclaration is MethodDeclarationSyntax
        {
            Identifier.ValueText: "AddSelfRegisteredExtensions",
        };

    static bool IsAddSelfRegisteredExtensions([NotNullWhen(true)] IMethodSymbol? method) =>
        method is IMethodSymbol
        {
            IsStatic: true,
            IsExtensionMethod: true,
            Parameters.Length: 2,
            TypeParameters.Length: 0,
        };

    static string GetMethodGroupExpression(IMethodSymbol method) =>
        $"{method.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{method.Name}";

    static bool HasAllureXunitAttribute(Compilation compilation, CancellationToken _)
    {
        var attributeType =
            compilation.GetTypeByMetadataName(Types.AllureXunitAttribute);

        if (attributeType is null)
        {
            return false;
        }

        foreach (var attr in compilation.Assembly.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeType))
            {
                return true;
            }
        }

        return false;
    }

    static void GenerateAllureXunitSources(SourceProductionContext ctx, (((AllureXunitGeneratorOptions, bool), ImmutableArray<string>), ImmutableArray<(int, ImmutableArray<string>)>) input)
    {
        var (((options, hasAllureXunit), selfRegistrations), allureIdMethods) = input;
        if (options.ApplyAttribute && !hasAllureXunit)
        {
            GenerateAllureXunitAssemblyAttribute(ctx);
        }

        if (options.GenerateEntryPoint)
        {
            GenerateAllureIdRegistry(ctx, allureIdMethods);
            GenerateEntryPoint(ctx, selfRegistrations);
        }

    }

    static void GenerateAllureXunitAssemblyAttribute(SourceProductionContext ctx)
    {
        ctx.AddSource(
            "AllureXunitAssemblyAttributes.g.cs",
            $"[assembly:global::{Types.AllureXunitAttribute}]"
        );
    }

    static void GenerateEntryPoint(SourceProductionContext ctx, ImmutableArray<string> selfRegistrations)
    {
        if (selfRegistrations.Length != 1)
            return;

        ctx.AddSource(
            "AllureXunitEntryPoint.g.cs",
            GetEntryPointSource(selfRegistrations)
        );
    }

    static void GenerateAllureIdRegistry(SourceProductionContext ctx, ImmutableArray<(int, ImmutableArray<string>)> entries)
    {
        ctx.AddSource("AllureIdTestMethodRegistry.g.cs", GetAllureIdRegistrySource(entries));
    }

    static string GetEntryPointSource(ImmutableArray<string> selfRegistrations) =>
        selfRegistrations.Length > 0
            ? selfRegistrations.Length == 1
                ? GetEntryPointSource(selfRegistrations[0])
                : GetEntryPointMultipleSelfRegistrationsSource()
            : GetEntryPointNoSelfRegistrationSource();

    static string GetEntryPointSource(string selfRegistration) =>
        $$"""
        namespace Allure.Xunit
        {
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            internal class AllureXunitAutoGeneratedEntryPoint
            {
                public static async global::System.Threading.Tasks.Task<int> Main(string[] args)
                {
                    if (global::System.Linq.Enumerable.Any(args, arg => arg == "-automated" || arg == "@@"))
                        return await global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run(args);
                    else
                        return await global::{{Types.TestPlatformTestFramework}}.RunAsync(
                            global::{{Types.TestPlanFunctions}}.GetArgsWithPreExecutionFilters(
                                args,
                                {{Singletons.AllureIdRegistry}}
                            ),
                            {{selfRegistration}}
                        );
                }
            }
        }
        """;

    static string GetEntryPointNoSelfRegistrationSource() =>
        """
        namespace Allure.Xunit
        {
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            internal class AllureXunitAutoGeneratedEntryPoint
            {
                public static async global::System.Threading.Tasks.Task<int> Main(string[] args)
                {
                    if (global::System.Linq.Enumerable.Any(args, arg => arg == "-automated" || arg == "@@"))
                        return await global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run(args);
                    else
                    {
                        global::System.Console.Error.WriteLine("Couldn't find the SelfRegisteredExtensions class.");
                        return 1;
                    }
                }
            }
        }
        """;

    static string GetEntryPointMultipleSelfRegistrationsSource() =>
        """
        namespace Allure.Xunit
        {
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            internal class AllureXunitAutoGeneratedEntryPoint
            {
                public static async global::System.Threading.Tasks.Task<int> Main(string[] args)
                {
                    if (global::System.Linq.Enumerable.Any(args, arg => arg == "-automated" || arg == "@@"))
                        return await global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run(args);
                    else
                    {
                        global::System.Console.Error.WriteLine("Multiple SelfRegisteredExtensions classes found. Cannot pick one");
                        return 1;
                    }
                }
            }
        }
        """;

    static string GetAllureIdRegistrySource(ImmutableArray<(int, ImmutableArray<string>)> entries)
    {
        var sb = new StringBuilder(
            $$"""
            namespace Allure.Xunit
            {
                [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
                internal static class AllureIdRegistry
                {
                    internal static global::System.Collections.Generic.IReadOnlyDictionary<int, global::System.Collections.Generic.IReadOnlyList<string>> Registry =
                        new global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.IReadOnlyList<string>>
                        {

            """
        );

        foreach (var (allureId, methods) in entries)
        {
            sb.Append("                { ");
            sb.Append(allureId);
            sb.Append(", new global::System.Collections.Generic.List<string> { ");
            sb.Append(
                SymbolDisplay.FormatLiteral(methods[0], quote: true)
            );
            foreach (var method in methods.Skip(1))
            {
                var methodLiteral = SymbolDisplay.FormatLiteral(method, quote: true);
                sb.Append(", ");
                sb.Append(methodLiteral);
            }
            sb.AppendLine(" } },");
        }

        sb.AppendLine(
            """
                        };
                }
            }
            """
        );
        return sb.ToString();
    }

    static class Types
    {
        public const string ITestApplicationBuilder = "Microsoft.Testing.Platform.Builder.ITestApplicationBuilder";

        public const string AllureXunitAttribute = "Allure.Xunit.AllureXunitAttribute";

        public const string AllureIdAttribute = "Allure.Net.Commons.Attributes.AllureIdAttribute";

        public const string TestPlatformTestFramework = "Xunit.MicrosoftTestingPlatform.TestPlatformTestFramework";

        public const string TestPlanFunctions = "Allure.Xunit.Functions.TestPlanFunctions";
    }

    static class Singletons
    {
        public const string AllureIdRegistry = "Allure.Xunit.AllureIdRegistry.Registry";
    }

    static readonly SymbolDisplayFormat FullyQualifiedNoTypeParameters = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    record struct AllureXunitGeneratorOptions(
        bool GenerateEntryPoint,
        bool ApplyAttribute
    );
}
