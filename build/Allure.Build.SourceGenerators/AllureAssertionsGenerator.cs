using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Allure.Build.SourceGenerators;

[Generator]
public class AllureAssertionsGenerator : IIncrementalGenerator
{
    record class MethodNames(
        string PropertyExistsAnyValue,
        string PropertyEquals,
        string PropertyEqualsCustom,
        string PropertySatisfiesConstraints,
        string SingleItem = "",
        string SingleItemByCriteria = "",
        string SingleItemByName = "",
        string SingleItemByNameComparator = "",
        string ItemByIndex = "",
        string ItemsSatisfyConstraints = ""
    )
    {
        public static MethodNames ForFactory(PropertyMetadata property) => property switch
        {
            CollectionPropertyMetadata ccProperty =>
                new(
                    PropertyExistsAnyValue: ccProperty.PropertyName,
                    PropertyEquals: ccProperty.PropertyName,
                    PropertyEqualsCustom: ccProperty.PropertyName,
                    PropertySatisfiesConstraints: ccProperty.PropertyName,
                    SingleItem: $"Single{ccProperty.ItemNamePascalCase}",
                    SingleItemByCriteria: $"OnlyOne{ccProperty.ItemNamePascalCase}",
                    SingleItemByName: $"OnlyOne{ccProperty.ItemNamePascalCase}",
                    SingleItemByNameComparator: $"OnlyOne{ccProperty.ItemNamePascalCase}",
                    ItemByIndex: $"{ccProperty.ItemNamePascalCase}At",
                    ItemsSatisfyConstraints: $"{ccProperty.ItemNamePascalCase}"
                ),
            _ => new(
                PropertyExistsAnyValue: property.PropertyName,
                    PropertyEquals: property.PropertyName,
                    PropertyEqualsCustom: property.PropertyName,
                    PropertySatisfiesConstraints: property.PropertyName
            ),
        };

        public static MethodNames ForAssertionSource(PropertyMetadata property) => property switch
        {
            CollectionPropertyMetadata ccProperty =>
                new(
                    PropertyExistsAnyValue: $"Has{ccProperty.PropertyName}",
                    PropertyEquals: $"Has{ccProperty.PropertyName}",
                    PropertyEqualsCustom: $"Has{ccProperty.PropertyName}",
                    PropertySatisfiesConstraints: $"Has{ccProperty.PropertyName}",
                    SingleItem: $"HasSingle{ccProperty.ItemNamePascalCase}",
                    SingleItemByCriteria: $"HasOnlyOne{ccProperty.ItemNamePascalCase}",
                    SingleItemByName: $"HasOnlyOne{ccProperty.ItemNamePascalCase}",
                    SingleItemByNameComparator: $"HasOnlyOne{ccProperty.ItemNamePascalCase}",
                    ItemByIndex: $"Has{ccProperty.ItemNamePascalCase}At",
                    ItemsSatisfyConstraints: $"Has{ccProperty.ItemNamePascalCase}"
                ),
            _ => new(
                PropertyExistsAnyValue: $"Has{property.PropertyName}",
                    PropertyEquals: $"Has{property.PropertyName}",
                    PropertyEqualsCustom: $"Has{property.PropertyName}",
                    PropertySatisfiesConstraints: $"Has{property.PropertyName}"
            ),
        };
    }

    record class PropertyMetadata(
        string InterfaceName,
        string InterfaceFullName,
        string PropertyName,
        string ValueType,
        ImmutableArray<string> EquatableTypes
    );

    record class CollectionPropertyMetadata(
        string InterfaceName,
        string InterfaceFullName,
        string PropertyName,
        string ValueType,
        ImmutableArray<string> EquatableTypes,
        string ItemName,
        string ItemNamePascalCase,
        string ItemType,
        bool ItemHasName
    ) : PropertyMetadata(
        InterfaceName: InterfaceName,
        InterfaceFullName: InterfaceFullName,
        PropertyName: PropertyName,
        ValueType: ValueType,
        EquatableTypes: EquatableTypes);

    record class CollectionCollectionPropertyMetadata(
        string InterfaceName,
        string InterfaceFullName,
        string PropertyName,
        string ValueType,
        ImmutableArray<string> EquatableTypes,
        string ItemName,
        string ItemNamePascalCase,
        string ItemType,
        bool ItemHasName,
        string ItemItemType
    ) : CollectionPropertyMetadata(
        InterfaceName: InterfaceName,
        InterfaceFullName: InterfaceFullName,
        PropertyName: PropertyName,
        ValueType: ValueType,
        EquatableTypes: EquatableTypes,
        ItemName: ItemName,
        ItemNamePascalCase: ItemNamePascalCase,
        ItemType: ItemType,
        ItemHasName: ItemHasName);

    static readonly Regex propertyNamePattern = new(@"^IAllure(?<name>\w+)Property$");

    static readonly Regex wsBeforeCapitalPattern = new(@"(?<!^)(?=[A-Z])");

    static readonly SymbolDisplayFormat FullyQualifiedNoTypeParameters = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        DefineAttribute(context);
        var propertyInterfaces = CreatePropertyMetadataProvider(context);
        var cSharpVersion = CreateCSharpVersionProvider(context);

        context.RegisterSourceOutput(propertyInterfaces.Combine(cSharpVersion), ProduceAssertionExtensionBlocks);
    }

    static void DefineAttribute(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddEmbeddedAttributeDefinition();
            ctx.AddSource(
                "AssertableProperty.g.cs",
                """
                namespace Allure.Testing.Assertions
                {
                    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
                    internal class GenerateAllureAssertionsAttribute: global::System.Attribute { }
                }
                """
            );
        });
    }

    static IncrementalValuesProvider<PropertyMetadata?> CreatePropertyMetadataProvider(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is InterfaceDeclarationSyntax iFace
                        && iFace.AttributeLists.Count > 0,
                transform: TransformToPropertyMetadata
            );

    static IncrementalValueProvider<LanguageVersion?> CreateCSharpVersionProvider(
        IncrementalGeneratorInitializationContext context
    ) =>
        context.CompilationProvider
            .Select(static (compilation, _)  =>
            {
            LanguageVersion? cSharpVersion = compilation is CSharpCompilation cSharpCompilation
                ? cSharpCompilation.LanguageVersion
                : null;

            return cSharpVersion;
            });

    static PropertyMetadata? TransformToPropertyMetadata(GeneratorSyntaxContext ctx, CancellationToken _)
    {
        var iFaceDeclarationSyntax = (InterfaceDeclarationSyntax)ctx.Node;

        foreach (AttributeListSyntax attributeListSyntax in iFaceDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == Types.GenerateAllureAssertionsAttribute)
                {
                    return GetMetadataFromPropertyInterface(ctx, iFaceDeclarationSyntax);
                }
            }
        }

        return null;
    }

    static void ProduceAssertionExtensionBlocks(SourceProductionContext ctx, (PropertyMetadata? Left, LanguageVersion? Right) data)
    {
        if (data is (_, null) or (null, _) or (_, < LanguageVersion.CSharp14))
        {
            return;
        }

        var (property, _) = data;

        var sb = new StringBuilder(
            $$"""
            namespace Allure.Testing;

            #nullable enable

            public static partial class AllureAssertionExtensions
            {

            """
        );

        AddFactoryExtensionBlock(sb, property);
        sb.AppendLine();
        AddAssertionSourceExtensionBlock(sb, property);

        sb.AppendLine("}");

        ctx.AddSource(
            $"AllureAssertionExtensions.{property.PropertyName}.g.cs",
            sb.ToString()
        );
    }

    static PropertyMetadata? GetMetadataFromPropertyInterface(
        GeneratorSyntaxContext ctx,
        InterfaceDeclarationSyntax propertyInterfaceSyntax
    )
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(propertyInterfaceSyntax) is not INamedTypeSymbol propertyInterfaceSymbol)
        {
            return null;
        }

        var propertyInterfaceName = propertyInterfaceSymbol.Name;

        var propertyNameMatch = propertyNamePattern.Match(propertyInterfaceName);
        if (!propertyNameMatch.Success)
        {
            return null;
        }
        var propertyName = propertyNameMatch.Groups["name"].Value;

        var interfaceFullName = propertyInterfaceSymbol.ToDisplayString(FullyQualifiedNoTypeParameters);
        var propertyInterface = propertyInterfaceSymbol
            .AllInterfaces
            .FirstOrDefault(static i => i.OriginalDefinition.ToString() == Types.Open.IAllureProperty);

        if (propertyInterface is not null)
        {
            var valueType = propertyInterface.TypeArguments[0];

            var equatableTo = valueType
                .AllInterfaces
                .Where(static i => i.OriginalDefinition.ToString() == Types.Open.IEquatable)
                .Select(static i => i.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .ToImmutableArray();

            var valueTypeName =
                propertyInterface
                    .TypeArguments[0]
                    .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var propertyArrayInterface = propertyInterfaceSymbol
                .AllInterfaces
                .FirstOrDefault(static i => i.OriginalDefinition.ToString() == Types.Open.IAllureArrayProperty);

            if (propertyArrayInterface is null)
            {
                return new PropertyMetadata(
                    InterfaceName: propertyInterfaceName,
                    InterfaceFullName: interfaceFullName,
                    PropertyName: propertyName,
                    ValueType: valueTypeName,
                    EquatableTypes: equatableTo
                );
            }

            var itemType = propertyArrayInterface.TypeArguments[0];

            var itemHasName = itemType
                .AllInterfaces
                .Select(i => i.OriginalDefinition.ToString())
                .Contains(Types.Open.IAllureNameProperty);

            var itemTypeName = itemType
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var itemNamePascalCase =
                propertyName[propertyName.Length - 1] == 's'
                    ? propertyName.Substring(0, propertyName.Length - 1)
                    : $"{propertyName}Item";

            var itemName = wsBeforeCapitalPattern.Replace(propertyName, " ").ToLowerInvariant();

            var itemItemTypeName =
                itemType
                    .AllInterfaces
                    .FirstOrDefault(static i => i.OriginalDefinition.ToString() == Types.Open.IReadOnlyList)
                    ?.TypeArguments[0]
                    ?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            return itemItemTypeName is null
                ? new CollectionPropertyMetadata(
                    InterfaceName: propertyInterfaceName,
                    InterfaceFullName: interfaceFullName,
                    PropertyName: propertyName,
                    ValueType: valueTypeName,
                    EquatableTypes: equatableTo,
                    ItemName: itemName,
                    ItemNamePascalCase: itemNamePascalCase,
                    ItemType: itemTypeName,
                    ItemHasName: itemHasName)
                : new CollectionCollectionPropertyMetadata(
                    InterfaceName: propertyInterfaceName,
                    InterfaceFullName: interfaceFullName,
                    PropertyName: propertyName,
                    ValueType: valueTypeName,
                    EquatableTypes: equatableTo,
                    ItemName: itemName,
                    ItemNamePascalCase: itemNamePascalCase,
                    ItemType: itemTypeName,
                    ItemHasName: itemHasName,
                    ItemItemType: itemItemTypeName);
        }

        return null;
    }

    static void AddFactoryExtensionBlock(StringBuilder sb, PropertyMetadata property)
    {
        sb.AppendLine(
            $$"""
                extension<TObject> ({{Types.PropertyAssertionFactory("TObject")}} source)
                    where TObject : {{Types.IAllureModelObject("TObject")}}, {{property.InterfaceFullName}}<TObject>
                {
            """
        );

        var methodNames = MethodNames.ForFactory(property);
        AddExtensionMethods(sb, methodNames, property);

        sb.AppendLine("    }");
    }

    static void AddAssertionSourceExtensionBlock(StringBuilder sb, PropertyMetadata property)
    {
        sb.AppendLine(
            $$"""
                extension<TObject> ({{Types.IAssertionSource("TObject")}} source)
                    where TObject : {{Types.IAllureModelObject("TObject")}}, {{property.InterfaceFullName}}<TObject>
                {
            """
        );

        var methodNames = MethodNames.ForAssertionSource(property);
        AddExtensionMethods(sb, methodNames, property);

        sb.AppendLine("    }");
    }

    static void AddExtensionMethods(StringBuilder sb, MethodNames methodNames, PropertyMetadata property)
    {
        if (property is CollectionPropertyMetadata collectionProperty)
        {
            AddMethodsForCollectionProperty(sb, methodNames, collectionProperty);
        }
        else
        {
            AddMethodsForScalarProperty(sb, methodNames, property);
        }
    }

    static void AddMethodsForScalarProperty(StringBuilder sb, MethodNames methodNames, PropertyMetadata property)
    {
        AddScalarPropertyExistsMethod(sb, methodNames.PropertyExistsAnyValue, property);
        sb.AppendLine();
        AddScalarPropertyEqualsMethods(sb, methodNames.PropertyEquals, property);
        sb.AppendLine();
        AddScalarPropertyEqualsByComparerMethods(sb, methodNames.PropertyEqualsCustom, property);
        sb.AppendLine();
        AddScalarPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
    }

    static void AddMethodsForCollectionProperty(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddCollectionPropertyExistsMethod(sb, methodNames.PropertyExistsAnyValue, property);
        sb.AppendLine();
        AddScalarPropertyEqualsMethods(sb, methodNames.PropertyEquals, property);
        sb.AppendLine();
        AddScalarPropertyEqualsByComparerMethods(sb, methodNames.PropertyEqualsCustom, property);
        sb.AppendLine();
        AddScalarPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
        sb.AppendLine();
        AddCollectionSpecificMethods(sb, methodNames, property);
    }

    static void AddScalarPropertyExistsMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowToJsonPropertyAssertion("TObject", property)}} {{methodName}}()
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                        return new (ctx);
                    }
            """
        );

    static void AddScalarPropertyEqualsMethods(StringBuilder sb, string methodName, PropertyMetadata property)
    {
        if (property.EquatableTypes.Any())
        {
            AddScalarPropertyEqualsMethod(sb, methodName, property, property.EquatableTypes[0]);
        }

        foreach (var equatableType in property.EquatableTypes.Skip(1))
        {
            sb.AppendLine();
            AddScalarPropertyEqualsMethod(sb, methodName, property, equatableType);
        }
    }

    static void AddScalarPropertyEqualsMethod(StringBuilder sb, string methodName, PropertyMetadata property, string equatableType) =>
        sb.AppendLine(
            $$"""
                    public {{Types.JsonPropertyEquatableAssertion("TObject", property, equatableType)}} {{methodName}}(
                        {{equatableType}} expectedValue,
                        {{Attributes.CallerArgumentExpressionFor("expectedValue")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        return new (ctx, expectedValue);
                    }
            """
        );

    static void AddScalarPropertyEqualsByComparerMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.JsonPropertyComparerAssertion("TObject", property)}} {{methodName}}(
                        {{property.ValueType}} expected{{property.PropertyName}},
                        {{Types.IEqualityComparer(property.ValueType)}} comparer,
                        {{Attributes.CallerArgumentExpressionFor($"expected{property.PropertyName}")}} string? expected{{property.PropertyName}}Expression = null,
                        {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expected{{property.PropertyName}}Expression ?? "..."}, {comparerExpression ?? "..."})");

                        return new (ctx, expected{{property.PropertyName}}, comparer);
                    }
            """
        );

    static void AddScalarPropertyConstrainedMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                        {{Types.Constraint(property.ValueType)}} constraints,
                        {{Attributes.CallerArgumentExpressionFor("constraints")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        return new (ctx, constraints);
                    }
            """
        );

    static void AddCollectionPropertyExistsMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}} {{methodName}}()
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                        return new (ctx);
                    }
            """
        );

    static void AddCollectionSpecificMethods(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        if (property is CollectionCollectionPropertyMetadata collectionCollectionProperty)
        {
            AddCollectionOfScalarsMethods(sb, methodNames, collectionCollectionProperty);
        }
        else
        {
            AddCollectionOfCollectionsMethods(sb, methodNames, property);
        }
    }

    static void AddCollectionOfCollectionsMethods(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddSingleScalarMethod(sb, methodNames.SingleItem, property);
        sb.AppendLine();
        AddOneScalarByCriteriaMethod(sb, methodNames.SingleItemByCriteria, property);
        if (property.ItemHasName)
        {
            sb.AppendLine();
            AddOneScalarByNameMethod(sb, methodNames.SingleItemByName, property);
            sb.AppendLine();
            AddOneScalarByNameWithComparerMethod(sb, methodNames.SingleItemByNameComparator, property);
        }
        sb.AppendLine();
        AddOneScalarByIndexMethod(sb, methodNames.ItemByIndex, property);
        sb.AppendLine();
        AddConstrainedScalarsMethod(sb, methodNames.ItemsSatisfyConstraints, property);
    }

    static void AddCollectionOfScalarsMethods(StringBuilder sb, MethodNames methodNames, CollectionCollectionPropertyMetadata property)
    {
        AddSingleCollectionMethod(sb, methodNames.SingleItem, property);
        sb.AppendLine();
        AddOneCollectionByCriteriaMethod(sb, methodNames.SingleItemByCriteria, property);
        sb.AppendLine();
        AddOneCollectionByIndexMethod(sb, methodNames.ItemByIndex, property);
        sb.AppendLine();
        AddConstrainedScalarsMethod(sb, methodNames.ItemsSatisfyConstraints, property);
    }

    static void AddSingleScalarMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionAssertion(property)}} {{methodName}}()
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneScalarByCriteriaMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                        {{Types.Constraint(property.ItemType)}} criteria,
                        {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, criteria, "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneScalarByNameMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                        string name,
                        {{Attributes.CallerArgumentExpressionFor("name")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, e => e.HasName(name), "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneScalarByNameWithComparerMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                        string name,
                        {{Types.IEqualityComparer("string")}} comparer,
                        {{Attributes.CallerArgumentExpressionFor("name")}} string? nameExpression = null,
                        {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, e => e.HasName(name, comparer), "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneScalarByIndexMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionByIndexAssertion(property)}} {{methodName}}(
                        int index,
                        {{Attributes.CallerArgumentExpressionFor("index")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, index, "{{property.ItemName}}");
                    }
            """
        );

    static void AddConstrainedScalarsMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.CollectionItemConstraintsAssertion(property)}} {{methodName}}(
                        {{Types.Constraint(property.ItemType)}}[] constraints,
                        {{Attributes.CallerArgumentExpressionFor("constraints")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, constraints, "{{property.ItemName}}");
                    }
            """
        );

    static void AddSingleCollectionMethod(StringBuilder sb, string methodName, CollectionCollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionToCollectionAssertion(property)}} {{methodName}}()
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneCollectionByCriteriaMethod(StringBuilder sb, string methodName, CollectionCollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionToCollectionByCriteriaAssertion(property)}} {{methodName}}(
                        {{Types.Constraint(property.ItemType)}} criteria,
                        {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, criteria, "{{property.ItemName}}");
                    }
            """
        );

    static void AddOneCollectionByIndexMethod(StringBuilder sb, string methodName, CollectionCollectionPropertyMetadata property) =>
        sb.AppendLine(
            $$"""
                    public {{Types.NarrowCollectionToCollectionByIndexAssertion(property)}} {{methodName}}(
                        int index,
                        {{Attributes.CallerArgumentExpressionFor("index")}} string? expression = null
                    )
                    {
                        var ctx = source.Context;
                        ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                        var propertyAssertion =
                            new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                                source.Context);

                        var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                            propertyAssertion.And
                        );

                        narrowedContext.ExpressionBuilder.Length -= 4;

                        return new(narrowedContext, index, "{{property.ItemName}}");
                    }
            """
        );

    static class Types
    {
        public const string GenerateAllureAssertionsAttribute = "Allure.Testing.Assertions.GenerateAllureAssertionsAttribute";

        public static string NarrowToJsonPropertyAssertion(string target, PropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowToJsonPropertyAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

        public static string JsonPropertyEquatableAssertion(string target, PropertyMetadata property, string equatableType) =>
            $"global::Allure.Testing.Assertions.JsonPropertyEquatableAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}, {equatableType}>";

        public static string JsonPropertyComparerAssertion(string target, PropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.JsonPropertyComparerAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

        public static string JsonPropertyCriteriaAssertion(string target, PropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.JsonPropertyCriteriaAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

        public const string CallerArgumentExpression =
            $"global::System.Runtime.CompilerServices.CallerArgumentExpression";

        public static string IEqualityComparer(string type) =>
            $"global::System.Collections.Generic.IEqualityComparer<{type}>";

        public const string IAssertion =
            "global::TUnit.Assertions.Core.IAssertion";

        public static string IAssertionSource(string typeArgument) =>
            $"global::TUnit.Assertions.Core.IAssertionSource<{typeArgument}>";

        public static string Func(string parameterType, string returnType) =>
            $"global::System.Func<{parameterType}, {returnType}>";

        public static string Constraint(string type) =>
            Func(IAssertionSource(type), IAssertion);

        public static string NarrowToJsonCollectionPropertyAssertion(string target, CollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowToJsonCollectionPropertyAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}, {property.ItemType}>";

        public static string NarrowCollectionAssertion(CollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionAssertion<{property.ValueType}, {property.ItemType}>";

        public static string AssertionAccessors(string type) =>
            $"global::Allure.Testing.Internal.TUnitAccessors.AssertionAccessors<{type}>";

        public static string PropertyAssertionFactory(string type) =>
            $"global::Allure.Testing.Assertions.PropertyAssertionFactory<{type}>";

        public static string IAllureModelObject(string type) =>
            $"global::Allure.Testing.Assertions.Model.IAllureModelObject<{type}>";

        public static string NarrowCollectionByCriteriaAssertion(CollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionByCriteriaAssertion<{property.ValueType}, {property.ItemType}>";

        public static string NarrowCollectionByIndexAssertion(CollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionByIndexAssertion<{property.ValueType}, {property.ItemType}>";

        public static string CollectionItemConstraintsAssertion(CollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.CollectionItemConstraintsAssertion<{property.ValueType}, {property.ItemType}>";

        public static string NarrowCollectionToCollectionAssertion(CollectionCollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

        public static string NarrowCollectionToCollectionByCriteriaAssertion(CollectionCollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionByCriteriaAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

        public static string NarrowCollectionToCollectionByIndexAssertion(CollectionCollectionPropertyMetadata property) =>
            $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionByIndexAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

        public static class Open
        {
            public const string IEquatable = "System.IEquatable<T>";

            public const string IAllureProperty = "Allure.Testing.Assertions.Model.Properties.IAllureProperty<TValue, TSelf>";

            public const string IAllureArrayProperty = "Allure.Testing.Assertions.Model.Properties.IAllureArrayProperty<TElement, TSelf>";

            public const string IAllureNameProperty = "Allure.Testing.Assertions.Model.Properties.IAllureNameProperty<TSelf>";

            public const string IReadOnlyList = "System.Collections.Generic.IReadOnlyList<T>";
        }

    }

    static class Attributes
    {
        public static string CallerArgumentExpressionFor(string parameter) =>
            $"[{Types.CallerArgumentExpression}(nameof({parameter}))]";
    }
}
