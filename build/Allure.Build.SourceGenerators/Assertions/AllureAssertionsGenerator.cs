using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Allure.Build.SourceGenerators.Assertions;

[Generator]
public sealed class AllureAssertionsGenerator : IIncrementalGenerator
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
        AddCommonPropertyMethods(sb, methodNames, property);
        sb.AppendLine();

        if (property is CollectionPropertyMetadata collectionProperty)
        {
            AddMethodsForCollectionProperty(sb, methodNames, collectionProperty);
        }
        else
        {
            AddMethodsForScalarProperty(sb, methodNames, property);
        }
    }

    static void AddCommonPropertyMethods(StringBuilder sb, MethodNames methodNames, PropertyMetadata property)
    {
        AddNoPropertyMethod(sb, methodNames.NoProperty, property);
        sb.AppendLine();
        AddPropertyEquatableMethod(sb, methodNames.PropertyEquatable, property);
        sb.AppendLine();
        AddPropertyEqualsMethods(sb, methodNames.PropertyEquals, property);
        sb.AppendLine();
        AddPropertyEqualsByComparerMethods(sb, methodNames.PropertyEqualsCustom, property);
    }

    static void AddMethodsForScalarProperty(StringBuilder sb, MethodNames methodNames, PropertyMetadata property)
    {
        AddScalarPropertyExistsMethod(sb, methodNames.PropertyExistsAnyValue, property);
        sb.AppendLine();
        AddScalarPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
    }

    static void AddMethodsForCollectionProperty(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddCollectionPropertyExistsMethod(sb, methodNames.PropertyExistsAnyValue, property);
        sb.AppendLine();
        AddCollectionPropertyConstrainedMethods(sb, methodNames.PropertySatisfiesConstraints, property);
        sb.AppendLine();
        AddCollectionSpecificMethods(sb, methodNames, property);
    }

    static void AddCollectionSpecificMethods(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddCommonCollectionSpecificMethods(sb, methodNames, property);

        sb.AppendLine();

        if (property is CollectionOfCollectionsPropertyMetadata collectionCollectionProperty)
        {
            AddCollectionOfScalarsMethods(sb, methodNames, collectionCollectionProperty);
        }
        else
        {
            AddCollectionOfCollectionsMethods(sb, methodNames, property);
        }
    }

    static void AddCommonCollectionSpecificMethods(StringBuilder sb, MethodNames methodNames, CollectionPropertyMetadata property)
    {
        AddConstrainedItemsMethod(sb, methodNames.ItemsSatisfyConstraints, property);

        sb.AppendLine();
        AddOneComparableItemMethod(sb, methodNames.OneComparableItem, property);
        sb.AppendLine();
        AddComparableItemMethod(sb, methodNames.ComparableItem, property);
        sb.AppendLine();
        AddNoComparableItemMethod(sb, methodNames.NoComparableItem, property);

        sb.AppendLine();
        AddOneCustomComparableItemMethod(sb, methodNames.OneCustomComparableItem, property);
        sb.AppendLine();
        AddCustomComparableItemMethod(sb, methodNames.CustomComparableItem, property);
        sb.AppendLine();
        AddNoCustomComparableItemMethod(sb, methodNames.NoCustomComparableItem, property);

        sb.AppendLine();
        AddOneEquatableItemMethod(sb, methodNames.OneEquatableItem, property);
        sb.AppendLine();
        AddEquatableItemMethod(sb, methodNames.EquatableItem, property);
        sb.AppendLine();
        AddNoEquatableItemMethod(sb, methodNames.NoEquatableItem, property);

        sb.AppendLine();
        AddOneItemByCriteriaMethod(sb, methodNames.OneItemByCriteria, property);
        sb.AppendLine();
        AddItemByCriteriaMethod(sb, methodNames.ItemByCriteria, property);
        sb.AppendLine();
        AddNoItemByCriteriaMethod(sb, methodNames.NoItemByCriteria, property);

        if (property.ItemHasName)
        {
            sb.AppendLine();
            AddOneItemByNameMethod(sb, methodNames.OneItemByName, property);
            sb.AppendLine();
            AddItemByNameMethod(sb, methodNames.ItemByName, property);
            sb.AppendLine();
            AddNoItemByNameMethod(sb, methodNames.NoItemByName, property);

            sb.AppendLine();
            AddOneItemByNameComparatorMethod(sb, methodNames.OneItemByNameComparator, property);
            sb.AppendLine();
            AddItemByNameComparatorMethod(sb, methodNames.ItemByNameComparator, property);
            sb.AppendLine();
            AddNoItemByNameComparatorMethod(sb, methodNames.NoItemByNameComparator, property);
        }
    }

    static void AddOneComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneComparableItem(methodName, property)
        );

    static void AddOneCustomComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneCustomComparableItem(methodName, property)
        );

    static void AddOneEquatableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneEquatableItem(methodName, property)
        );

    static void AddOneItemByCriteriaMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneItemByCriteria(methodName, property)
        );

    static void AddOneItemByNameMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneItemByName(methodName, property)
        );

    static void AddOneItemByNameComparatorMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.OneItemByNameComparator(methodName, property)
        );

    static void AddComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.ComparableItem(methodName, property)
        );

    static void AddCustomComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.CustomComparableItem(methodName, property)
        );

    static void AddEquatableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.EquatableItem(methodName, property)
        );

    static void AddItemByCriteriaMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.ItemByCriteria(methodName, property)
        );

    static void AddItemByNameMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.ItemByName(methodName, property)
        );

    static void AddItemByNameComparatorMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.ItemByNameComparator(methodName, property)
        );

    static void AddNoComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoComparableItem(methodName, property)
        );

    static void AddNoCustomComparableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoCustomComparableItem(methodName, property)
        );

    static void AddNoEquatableItemMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoEquatableItem(methodName, property)
        );

    static void AddNoItemByCriteriaMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoItemByCriteria(methodName, property)
        );

    static void AddNoItemByNameMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoItemByName(methodName, property)
        );

    static void AddNoItemByNameComparatorMethod(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoItemByNameComparator(methodName, property)
        );

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
    }

    static void AddCollectionOfScalarsMethods(StringBuilder sb, MethodNames methodNames, CollectionOfCollectionsPropertyMetadata property)
    {
        AddSingleCollectionMethod(sb, methodNames.SingleItem, property);
        sb.AppendLine();
        AddOneCollectionByCriteriaMethod(sb, methodNames.SingleItemByCriteria, property);
        sb.AppendLine();
        AddOneCollectionByIndexMethod(sb, methodNames.ItemByIndex, property);
    }

    static void AddNoPropertyMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.NoProperty(methodName, property)
        );

    static void AddScalarPropertyExistsMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.ScalarPropertyExists(methodName, property)
        );

    static void AddPropertyEquatableMethod(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.PropertyEquatable(methodName, property)
        );

    static void AddPropertyEqualsMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.PropertyEquals(methodName, property)
        );

    static void AddPropertyEqualsByComparerMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.PropertyEqualsByComparer(methodName, property)
        );

    static void AddScalarPropertyConstrainedMethods(StringBuilder sb, string methodName, PropertyMetadata property) =>
        sb.AppendLine(
            Methods.ScalarPropertyConstrained(methodName, property)
        );

    static void AddCollectionPropertyConstrainedMethods(StringBuilder sb, string methodName, CollectionPropertyMetadata property) =>
        sb.AppendLine(
            Methods.CollectionPropertyConstrained(methodName, property)
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
