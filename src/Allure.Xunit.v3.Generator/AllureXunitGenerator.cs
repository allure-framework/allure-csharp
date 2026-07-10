using System;
using System.Collections.Immutable;
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
        var options = SetupGeneratorOptionsStream(context);
        var hasTestHook = SetupHasAllureXunitAttributeStream(context);
        var selfRegistration = SetupSelfRegistrationStream(context);
        var allureIdMethods = SetupAllureIdMethodStream(context);

        context.RegisterPostInitializationOutput(GenerateAllureXunitRunner);
        context.RegisterSourceOutput(options.Combine(hasTestHook), GenerateAllureXunitAssemblyAttribute);
        context.RegisterSourceOutput(options.Combine(selfRegistration), GenerateAllureXunitEntryPoint);
        context.RegisterSourceOutput(allureIdMethods, GenerateAllureXunitTestPlan);
    }

    static IncrementalValueProvider<string> SetupSelfRegistrationStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.AnalyzerConfigOptionsProvider
            .Select(ResolveSelfRegisteredExtensionsTypeName)
            .Combine(context.CompilationProvider)
            .Select(VerifySelfRegistration);

    static string ResolveSelfRegisteredExtensionsTypeName(
        AnalyzerConfigOptionsProvider optionsProvider,
        CancellationToken token
    ) =>
        optionsProvider.GlobalOptions
            .TryGetValue(Options.RootNamespace, out var rootNamespace)
                && !string.IsNullOrEmpty(rootNamespace)
                    ? $"{rootNamespace}.{TypeNames.SelfRegisteredExtensions}"
                    : TypeNames.SelfRegisteredExtensions;

    static IncrementalValueProvider<ImmutableArray<(int, string)>> SetupAllureIdMethodStream(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsMethodWithAttributes,
                transform: ToAllureIdMethodKeyValuePair
            )
            .Where(static (kv) => kv is not null)
            .Select(static (kv, _) => kv!.Value)
            .Collect();

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

    static string VerifySelfRegistration(
        (string selfRegistrationTypeName, Compilation compilation) input,
        CancellationToken token
    )
    {
        var (selfRegistrationTypeName, compilation) = input;

        var builderType = compilation.GetTypeByMetadataName(
            "Microsoft.Testing.Platform.Builder.ITestApplicationBuilder"
        );
        if (builderType is null)
        {
            return "";
        }

        var selfRegisteredExtensionsType =
            compilation.GetTypeByMetadataName(selfRegistrationTypeName);

        if (selfRegisteredExtensionsType is null)
        {
            return "";
        }

        foreach (var member in selfRegisteredExtensionsType.GetMembers(MethodNames.AddSelfRegisteredExtensions))
        {
            if (ToRegistrationMethod(builderType, member) is { } method)
            {
                return GetMethodGroupExpression(method);
            }
        }

        return "";
    }

    static IMethodSymbol? ToRegistrationMethod(INamedTypeSymbol builderType, ISymbol? member) =>
        member is IMethodSymbol
        {
            IsStatic: true,
            IsExtensionMethod: true,
            ReturnsVoid: true,
            DeclaredAccessibility: Accessibility.Public,
            Parameters: { Length: 2 } parameters,
        } method
            && SymbolEqualityComparer.Default.Equals(parameters[0].Type, builderType)
            && parameters[1].Type is IArrayTypeSymbol
            {
                ElementType.SpecialType: SpecialType.System_String,
            }
                ? method
                : null;


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

    static AllureXunitGeneratorOptions ReadGeneratorProperties(
        AnalyzerConfigOptionsProvider provider,
        CancellationToken _
    )
    {
        var opts = provider.GlobalOptions;
        return new AllureXunitGeneratorOptions(
            GenerateEntryPoint: ReadBooleanProperty(opts, Options.GenerateEntryPoint),
            ApplyAttribute: ReadBooleanProperty(opts, Options.ApplyAttribute)
        );

    }

    static bool ReadBooleanProperty(AnalyzerConfigOptions opts, string key) =>
        !opts.TryGetValue(key, out var generateEntryPoint)
            || generateEntryPoint.Equals("true", StringComparison.InvariantCultureIgnoreCase);

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

    static void GenerateAllureXunitRunner(
        IncrementalGeneratorPostInitializationContext ctx
    )
    {
        ctx.AddSource("AllureXunitRunner.g.cs", AllureXunitRunnerSource);
    }

    static void GenerateAllureXunitAssemblyAttribute(
        SourceProductionContext ctx,
        (AllureXunitGeneratorOptions, bool) input
    )
    {
        var (options, hasAllureXunit) = input;
        if (!options.ApplyAttribute || hasAllureXunit)
        {
            return;
        }

        ctx.AddSource(
            "AllureXunitAssemblyAttributes.g.cs",
            $$"""
            {{Preamble}}

            [assembly:{{FqTypes.AllureXunitAttribute}}]
            """
        );
    }

    static void GenerateAllureXunitEntryPoint(
        SourceProductionContext ctx,
        (AllureXunitGeneratorOptions, string) input
    )
    {
        var (options, selfRegistrationMethod) = input;

        if (!options.GenerateEntryPoint)
        {
            return;
        }

        ctx.AddSource("AllureXunitEntryPoint.g.cs", GetEntryPointSource(selfRegistrationMethod));
    }

    static void GenerateAllureXunitTestPlan(
        SourceProductionContext ctx,
        ImmutableArray<(int, string)> allureIdMethods
    )
    {
        ctx.AddSource("AllureXunitTestPlan.g.cs", GetAllureXunitTestPlanSource(allureIdMethods));
    }

    static string GetAllureXunitTestPlanSource(ImmutableArray<(int, string)> allureIdMethods)
    {
        var sb = new StringBuilder(
            $$"""
            {{Preamble}}
            namespace Allure.Xunit
            {
                [{{FqTypes.ExcludeFromCodeCoverage}}]
                internal static class AllureXunitTestPlan
                {
                    /// <summary>
                    /// Returns a new array that includes the original arguments plus
                    /// <c>--filter-method</c> xUnit arguments for each test method
                    /// selected by the current test plan.
                    /// </summary>
                    /// <param name="originalArguments">
                    /// An array of the original command line arguments.
                    /// </param>
                    public static string[] {{MethodNames.AddXunitPreExecutionFilterArguments}}(string[] originalArguments)
                    {
                        return {{FqTypes.TestPlanFunctions}}.{{MethodNames.AddXunitPreExecutionFilterArguments}}(originalArguments, AllureIdRegistry);
                    }

            """
        );

        AddAllureIdRegistry(sb, allureIdMethods);

        sb.AppendLine(
            """
                }
            }
            """
        );

        return sb.ToString();
    }

    static void AddAllureIdRegistry(StringBuilder sb, ImmutableArray<(int, string)> entries)
    {
        sb.AppendLine(
            $$"""

                    /// <summary>
                    /// Gets a mapping from Allure ID values to the fully qualified names of methods annotated
                    /// with <see cref="{{FqTypes.AllureIdAttribute}}"/> using those IDs.
                    /// </summary>
                    /// <remarks>
                    /// Method names include the namespace, containing type name, and method name.
                    /// </remarks>
                    internal static {{FqTypes.ImmutableDictionary_Int_ImmutableArray_String}} AllureIdRegistry { get; }

                    static AllureXunitTestPlan()
                    {
                        {{FqTypes.ImmutableDictionaryBuilder_Int_ImmutableArray_String}} builder = {{FqMethods.ImmutableDictionary_CreateBuilder_Int_String}}();
            """
        );

        foreach (var (allureId, methodName) in entries)
        {
            sb.AppendLine(
                $$"""
                            AddAllureIdMethodEntry(builder, {{allureId}}, {{SymbolDisplay.FormatLiteral(methodName, quote: true)}});
                """
            );
        }

        sb.AppendLine(
            $$"""
                        AllureIdRegistry = builder.ToImmutable();
                    }

                    static void AddAllureIdMethodEntry({{FqTypes.ImmutableDictionaryBuilder_Int_ImmutableArray_String}} builder, int allureId, string methodName)
                    {
                        {{FqTypes.ImmutableArray_String}} value;
                        builder[allureId] = builder.TryGetValue(allureId, out value)
                            ? value.Add(methodName)
                            : {{FqMethods.ImmutableArray_Create}}(methodName);
                    }
            """
        );
    }

    static string AllureXunitRunnerSource =>
        $$"""
        {{Preamble}}
        namespace Allure.Xunit
        {
            /// <summary>
            /// Defines a function to run xUnit with Allure from an arbitrary entry point.
            /// </summary>
            [{{FqTypes.ExcludeFromCodeCoverage}}]
            internal static class AllureXunitRunner
            {
                /// <summary>
                /// Applies the test plan pre-execution filter and calls xUnit's
                /// <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" /> with an
                /// explicit extension registration function.
                /// </summary>
                /// <param name="extensionRegistration">
                /// A function that registers MTP extensions. The function must call
                /// <see cref="{{SeeCrefs.AddAllureXunit}}" /> in order to enable Allure.
                /// </param>
                /// <param name="args">Command line arguments</param>
                public static async {{FqTypes.Task_Int}} RunAsync(
                    {{FqTypes.Action(FqTypes.ITestApplicationBuilder, "string[]")}} extensionRegistration,
                    string[] args
                )
                {
                    return await {{FqTypes.TestPlatformTestFramework}}.RunAsync(
                        {{FqMethods.AddXunitPreExecutionFilterArguments}}(args),
                        extensionRegistration
                    );
                }
            }
        }
        """;

    static string GetEntryPointSource(string addSelfRegisteredExtensions)
    {
        var selfRegistrationExists = addSelfRegisteredExtensions is { Length: >0 };

        var sb = new StringBuilder(
            $$"""
            {{Preamble}}

            namespace Allure.Xunit
            {
                /// <summary>
                /// Defines the functions for running xUnit.net v3 with Allure and other self-registered
                /// MTP extensions enabled.
                /// </summary>
                [{{FqTypes.ExcludeFromCodeCoverage}}]
                internal class AllureXunitEntryPoint
                {
                    /// <summary>
                    /// Applies the test plan pre-execution filter and calls xUnit's
                    /// <see cref="{{FqTypes.TestPlatformTestFramework}}.RunAsync" />
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
                        if ({{FqMethods.Enumerable_Any}}(args, arg => arg == "-automated" || arg == "@@"))
                            return await {{FqMethods.ConsoleRunner_Run}}(args);
                        else

            """
        );

        if (selfRegistrationExists)
        {
            sb.AppendLine(
                $$"""
                                return await {{FqMethods.AllureXunitRunner_RunAsync}}({{addSelfRegisteredExtensions}}, args);
                """
            );
        }
        else
        {
            sb.AppendLine(
                $$"""
                            {
                                {{FqMethods.Error_WriteLine}}("{{Messages.SelfRegistrationNotFound}}");
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

        if (selfRegistrationExists)
        {
            sb.AppendLine(
                $$"""
                            return await {{FqMethods.AllureXunitRunner_RunAsync}}((builder, args) =>
                            {
                                {{addSelfRegisteredExtensions}}(builder, args);
                                {{FqMethods.AddAllureXunit}}(builder, allureRegistration);
                            }, args);
                """
            );
        }
        else
        {
            sb.AppendLine(
                $$"""
                            {{FqMethods.Error_WriteLine}}("{{Messages.SelfRegistrationNotFound}}");
                            return 1;
                """
            );
        }

        sb.AppendLine(
            $$"""
                    }
                }
            }
            """
        );

        return sb.ToString();
    }

    static string Preamble =>
        $$"""
        // <auto-generated>
        //   This file was generated by {{typeof(AllureXunitGenerator).AssemblyQualifiedName}}.
        //   Do not edit this file manually; changes may be overwritten.
        // </auto-generated>
        """;

    static readonly SymbolDisplayFormat FullyQualifiedNoTypeParameters = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    readonly record struct AllureXunitGeneratorOptions(
        bool GenerateEntryPoint,
        bool ApplyAttribute
    );
}
