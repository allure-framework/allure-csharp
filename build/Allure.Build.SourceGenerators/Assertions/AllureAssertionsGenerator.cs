using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Allure.Build.SourceGenerators.Assertions;

[Generator]
public class AllureAssertionsGenerator : IIncrementalGenerator
{
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
                AttributeDefinitions.GenerateAllureAssertionsAttribute
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

                    return GetMetadataFromPropertyInterface(ctx, iFaceDeclarationSyntax, attributeContainingTypeSymbol);
                }
            }
        }

        return null;
    }

    static PropertyMetadata? GetMetadataFromPropertyInterface(
        GeneratorSyntaxContext ctx,
        InterfaceDeclarationSyntax propertyInterfaceSyntax,
        INamedTypeSymbol attributeTypeSymbol
    )
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(propertyInterfaceSyntax) is not INamedTypeSymbol propertyInterfaceSymbol)
        {
            return null;
        }

        var nameProperties = AttributeProperties.Resolve(attributeTypeSymbol, propertyInterfaceSymbol);
        if (nameProperties is null)
        {
            return null;
        }

        var propertyInterfaceName = propertyInterfaceSymbol.Name;

        var interfaceFullName = propertyInterfaceSymbol.ToDisplayString(FullyQualifiedNoTypeParameters);
        var propertyInterface = propertyInterfaceSymbol
            .AllInterfaces
            .FirstOrDefault(static i => i.OriginalDefinition.ToString() == Types.Open.IAllureProperty);

        if (propertyInterface is not null)
        {
            var valueType = propertyInterface.TypeArguments[0];

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
                    Name: nameProperties.PropertyName,
                    MethodName: nameProperties.MethodName,
                    JsonName: nameProperties.JsonName,
                    ValueType: valueTypeName
                );
            }

            var itemType = propertyArrayInterface.TypeArguments[0];

            var itemHasName = itemType
                .AllInterfaces
                .Select(i => i.OriginalDefinition.ToString())
                .Contains(Types.Open.IAllureNameProperty);

            var itemTypeName = itemType
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

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
                    Name: nameProperties.PropertyName,
                    MethodName: nameProperties.MethodName,
                    JsonName: nameProperties.JsonName,
                    ValueType: valueTypeName,
                    ItemMethodName: nameProperties.ItemMethodName,
                    ItemName: nameProperties.ItemName,
                    ItemType: itemTypeName,
                    ItemHasName: itemHasName)
                : new CollectionOfCollectionsPropertyMetadata(
                    InterfaceName: propertyInterfaceName,
                    InterfaceFullName: interfaceFullName,
                    Name: nameProperties.PropertyName,
                    MethodName: nameProperties.MethodName,
                    JsonName: nameProperties.JsonName,
                    ValueType: valueTypeName,
                    ItemMethodName: nameProperties.ItemMethodName,
                    ItemName: nameProperties.ItemName,
                    ItemType: itemTypeName,
                    ItemHasName: itemHasName,
                    ItemItemType: itemItemTypeName);
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
            $"AllureAssertionExtensions.{property.InterfaceName}.g.cs",
            sb.ToString()
        );
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
        AddPropertyEqualsMethod(sb, methodNames.PropertyEquals, property);
        sb.AppendLine();
        AddPropertyEqualsByComparerMethods(sb, methodNames.PropertyEqualsCustom, property);
        sb.AppendLine();
        AddPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
    }

    static void AddMethodsForCollectionProperty(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddCollectionPropertyExistsMethod(sb, methodNames.PropertyExistsAnyValue, property);
        sb.AppendLine();
        AddPropertyEqualsMethod(sb, methodNames.PropertyEquals, property);
        sb.AppendLine();
        AddPropertyEqualsByComparerMethods(sb, methodNames.PropertyEqualsCustom, property);
        sb.AppendLine();
        AddPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
        sb.AppendLine();
        AddCollectionSpecificMethods(sb, methodNames, property);
    }

    static void AddCollectionSpecificMethods(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        if (property is CollectionOfCollectionsPropertyMetadata collectionCollectionProperty)
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
        AddConstrainedItemsMethod(sb, methodNames.ItemsSatisfyConstraints, property);
    }

    static void AddCollectionOfScalarsMethods(StringBuilder sb, MethodNames methodNames, CollectionOfCollectionsPropertyMetadata property)
    {
        AddSingleCollectionMethod(sb, methodNames.SingleItem, property);
        sb.AppendLine();
        AddOneCollectionByCriteriaMethod(sb, methodNames.SingleItemByCriteria, property);
        sb.AppendLine();
        AddOneCollectionByIndexMethod(sb, methodNames.ItemByIndex, property);
        sb.AppendLine();
        AddConstrainedItemsMethod(sb, methodNames.ItemsSatisfyConstraints, property);
    }

    static void AddScalarPropertyExistsMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.ScalarPropertyExists(methodName, property)
        );

    static void AddPropertyEqualsMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.PropertyEquals(methodName, property)
        );

    static void AddPropertyEqualsByComparerMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.PropertyEqualsByComparer(methodName, property)
        );

    static void AddPropertyConstrainedMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.ScalarPropertyConstrained(methodName, property)
        );

    static void AddCollectionPropertyExistsMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.CollectionPropertyExists(methodName, property)
        );

    static void AddSingleScalarMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.SingleScalar(methodName, property)
        );

    static void AddOneScalarByCriteriaMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneScalarByCriteria(methodName, property)
        );

    static void AddOneScalarByNameMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneScalarByName(methodName, property)
        );

    static void AddOneScalarByNameWithComparerMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneScalarByNameWithComparer(methodName, property)
        );

    static void AddOneScalarByIndexMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneScalarByIndex(methodName, property)
        );

    static void AddConstrainedItemsMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.ConstrainedItems(methodName, property)
        );

    static void AddSingleCollectionMethod(StringBuilder sb, string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        sb.AppendLine(
            Methods.SingleCollection(methodName, property)
        );

    static void AddOneCollectionByCriteriaMethod(StringBuilder sb, string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneCollectionByCriteria(methodName, property)
        );

    static void AddOneCollectionByIndexMethod(StringBuilder sb, string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneCollectionByIndex(methodName, property)
        );
}
