using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using Allure.Xunit.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Allure.Xunit.Generator;


[Generator]
public sealed class AllureXunitGenerator : IIncrementalGenerator
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
            $"[assembly:{FqTypes.AllureXunitAttribute}]"
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

    static string GetEntryPointSource(ImmutableArray<string> selfRegistrations)
    {
        Lazy<string> errorMessage = new(() => selfRegistrations switch
        {
            { IsEmpty: true } =>
                "Couldn't find the SelfRegisteredExtensions class.",

            _ =>
                $"Multiple self-registration classes found: [{
                    string.Join(", ", selfRegistrations)
                }]. Couldn't pick one. You may define a new entry point with the "
                    + "'StartupObject' MSBuild property and call "
                    + "'Allure.Xunit.AllureXunitEntryPoint.RunAsync' from it.",
        });

        var (registrationResolved, registration) = selfRegistrations is { Length: >0 }
            ? (true, selfRegistrations[0])
            : (false, "");

        var sb = new StringBuilder(
            $$"""
            {{Preamble}}

            namespace Allure.Xunit
            {
                /// <summary>
                /// Defines the default entry point to xUnit.net v3 with Allure enabled. Also, provides
                /// helper functions to define your own entry point.
                /// </summary
                [{{FqTypes.ExcludeFromCodeCoverage}}]
                internal class AllureXunitEntryPoint
                {
                    /// <summary>
                    /// The default entry point applies the test plan pre-execution filter and calls
                    /// xUnit's <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" />
                    /// with the self-registered MTP extensions.
                    /// </summary>
                    /// <remarks>
                    /// If <c>-automated</c> or <c>@@</c> argument is provided, delegates to
                    /// <see cref="{{SeeCrefs.ConsoleRunner_Run}}" />,
                    /// which does not register MTP extensions. Allure never runs in such a case.
                    /// <br/>
                    /// Otherwise, if Allure self-registration is enabled, it will be registered
                    /// with the default settings.
                    /// </remarks>
                    /// <param name="args">Command line arguments</param>
                    public static async {{FqTypes.Task_Int}} Main(string[] args)
                    {
                        if ({{Methods.Enumerable_Any}}(args, arg => arg == "-automated" || arg == "@@"))
                            return await {{Methods.ConsoleRunner_Run}}(args);
                        else

            """
        );

        if (registrationResolved)
        {
            sb.AppendLine(
                $$"""
                                return await RunAsync({{registration}}, args);
                """
            );
        }
        else
        {
            sb.AppendLine(
                $$"""
                            {
                                {{Methods.Error_WriteLine}}("{{errorMessage.Value}}");
                                return 1;
                            }
                """
            );
        }

        sb.AppendLine(
            $$"""
                    }

                    /// <summary>
                    /// Applies the test plan pre-execution filter and calls xUnit's
                    /// <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" /> with the
                    /// self-registered MTP extensions and Allure enabled. Uses a custom registration function
                    /// to set Allure up.
                    /// </summary>
                    /// <remarks>
                    /// If you're calling this function, make sure Allure.Xunit.v3 self-registration is disabled
                    /// (the 'Allure_XunitEnableSelfRegistration' MSBuild property is set to 'false').
                    /// </remarks>
                    /// <param name="allureRegistration">
                    /// A function that sets up Allure.Xunit.v3.
                    /// </param>
                    /// <param name="args">Command line arguments</param>
                    public static async {{FqTypes.Task_Int}} RunAsync(
                        {{FqTypes.Action(FqTypes.IStandaloneAllureRegistrationContext)}} allureRegistration,
                        string[] args
                    )
                    {
            """
        );

        if (registrationResolved)
        {
            sb.AppendLine(
                $$"""
                            return await RunAsync((builder, args) =>
                            {
                                {{registration}}(builder, args);
                                {{Methods.AddAllureXunit}}(builder, allureRegistration);
                            }, args);
                """
            );
        }
        else
        {
            sb.AppendLine(
                $$"""
                            {{Methods.Error_WriteLine}}("{{errorMessage.Value}}");
                            return 1;
                """
            );
        }

        sb.AppendLine(
            $$"""
                    }

                    /// <summary>
                    /// Applies the test plan pre-execution filter and calls xUnit's
                    /// <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" /> with the
                    /// provided MTP extension registration function.
                    /// </summary>
                    /// <param name="extensionRegistration">
                    /// A function that registers MTP extensions, including Allure
                    /// (see <see cref="{{SeeCrefs.AddAllureXunit}}" />).
                    /// </param>
                    /// <param name="args">Command line arguments</param>
                    public static async {{FqTypes.Task_Int}} RunAsync(
                        {{FqTypes.Action(FqTypes.ITestApplicationBuilder, "string[]")}} extensionRegistration,
                        string[] args
                    )
                    {
                        return await {{FqTypes.TestPlatformTestFramework}}.RunAsync(
                            {{FqTypes.TestPlanFunctions}}.GetArgsWithPreExecutionFilters(
                                args,
                                {{Singletons.AllureIdRegistry}}
                            ),
                            extensionRegistration
                        );
                    }

                    /// <summary>
                    /// Returns a new array that includes the original arguments plus
                    /// <c>--filter-method</c> xUnit arguments for each test method
                    /// selected by the current test plan.
                    /// </summary>
                    /// <remarks>
                    /// Use this function to enhance the CLI arguments before passing them to
                    /// <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" /> from
                    /// your own entry point if none of the above functions suit your needs.
                    /// </remarks>
                    /// <param name="originalArguments">
                    /// An array of the original command line arguments.
                    /// </param>
                    public static string[] AddPreExecutionFilter(string[] originalArguments) =>
                        {{FqTypes.TestPlanFunctions}}.GetArgsWithPreExecutionFilters(
                            originalArguments,
                            {{Singletons.AllureIdRegistry}}
                        );
                }
            }
            """
        );

        return sb.ToString();
    }

    static string GetAllureIdRegistrySource(ImmutableArray<(int, ImmutableArray<string>)> entries)
    {
        var sb = new StringBuilder(
            $$"""
            namespace Allure.Xunit
            {
                [{{FqTypes.ExcludeFromCodeCoverage}}]
                internal static class AllureIdRegistry
                {
                    internal static {{FqTypes.IReadOnlyDictionary_Int_StringList}} Registry =
                        new {{FqTypes.Dictionary_Int_StringList}}
                        {

            """
        );

        foreach (var (allureId, methods) in entries)
        {
            sb.Append("                { ");
            sb.Append(allureId);
            sb.Append($", new {FqTypes.List_String} {{ ");
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

    static string Preamble =>
        $$"""
        // <auto-generated/>
        // This file was generated by {{typeof(AllureXunitGenerator).AssemblyQualifiedName}}.
        // Do not edit this file manually; changes may be overwritten.
        """;

    static readonly SymbolDisplayFormat FullyQualifiedNoTypeParameters = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    record struct AllureXunitGeneratorOptions(bool GenerateEntryPoint, bool ApplyAttribute);
}
