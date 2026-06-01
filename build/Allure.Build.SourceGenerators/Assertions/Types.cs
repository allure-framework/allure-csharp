namespace Allure.Build.SourceGenerators.Assertions;

public static class Types
{
    public const string CallerArgumentExpression =
        $"global::System.Runtime.CompilerServices.CallerArgumentExpression";

    public static string IEquatable(string type) =>
        $"global::System.IEquatable<{type}>";

    public static string IEqualityComparer(string type) =>
        $"global::System.Collections.Generic.IEqualityComparer<{type}>";

    public static string EqualityComparer(string type) =>
        $"global::System.Collections.Generic.EqualityComparer<{type}>";

    public static string Func(string parameterType, string returnType) =>
        $"global::System.Func<{parameterType}, {returnType}>";

    public static string ScalarConstraint(string type) =>
        Func(IAssertionSource(type), IAssertion);

    public static string CollectionConstraint(string type) =>
        Func(CollectionAssertion(type), IAssertion);

    public static string OptionalScalarConstraint(string type) =>
        $"{Func(IAssertionSource(type), $"{IAssertion}?")}?";

    public static string OptionalCollectionConstraint(string type) =>
        $"{Func(IAssertionSource(type), $"{IAssertion}?")}?";

    public const string IAssertion =
        "global::TUnit.Assertions.Core.IAssertion";

    public static string IAssertionSource(string typeArgument) =>
        $"global::TUnit.Assertions.Core.IAssertionSource<{typeArgument}>";

    public static string CollectionAssertion(string typeArgument) =>
        $"global::TUnit.Assertions.Sources.CollectionAssertion<{typeArgument}>";

    public const string GenerateAllureAssertionsAttribute = "Allure.Testing.Assertions.GenerateAllureAssertionsAttribute";

    public static string NoJsonPropertyAssertion(string target) =>
        $"global::Allure.Testing.Assertions.NoJsonPropertyAssertion<{target}>";

    public static string HasComparableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasComparableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasEquatableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasEquatableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasItemByCriteriaAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasItemByCriteriaAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasOneComparableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasOneComparableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasOneEquatableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasOneEquatableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasOneItemByCriteriaAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasOneItemByCriteriaAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasNoComparableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasNoComparableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasNoEquatableItemAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasNoEquatableItemAssertion<{property.ValueType}, {property.ItemType}>";

    public static string HasNoItemByCriteriaAssertion(CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.HasNoItemByCriteriaAssertion<{property.ValueType}, {property.ItemType}>";

    public static string NarrowToJsonPropertyAssertion(string target, PropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.NarrowToJsonPropertyAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

    public static string JsonPropertyEquatableAssertion(string target, PropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.JsonPropertyEquatableAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

    public static string JsonPropertyComparerAssertion(string target, PropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.JsonPropertyComparerAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

    public static string JsonPropertyCriteriaAssertion(string target, PropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.JsonPropertyCriteriaAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}>";

    public static string JsonCollectionPropertyCriteriaAssertion(string target, CollectionPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.JsonCollectionPropertyCriteriaAssertion<{target}, {property.InterfaceFullName}<TObject>, {property.ValueType}, {property.ItemType}>";

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

    public static string NarrowCollectionToCollectionAssertion(CollectionOfCollectionsPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

    public static string NarrowCollectionToCollectionByCriteriaAssertion(CollectionOfCollectionsPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionByCriteriaAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

    public static string NarrowCollectionToCollectionByIndexAssertion(CollectionOfCollectionsPropertyMetadata property) =>
        $"global::Allure.Testing.Assertions.NarrowCollectionToCollectionByIndexAssertion<{property.ValueType}, {property.ItemType}, {property.ItemItemType}>";

    public static class Open
    {
        public const string IEquatable = "System.IEquatable<T>";

        public const string IAllureProperty = "Allure.Testing.Assertions.Model.Properties.IAllureProperty<TValue, TSelf>";

        public const string IAllureArrayProperty = "Allure.Testing.Assertions.Model.Properties.IAllureArrayProperty<TElement, TFactory, TSelf>";

        public const string IAllureNameProperty = "Allure.Testing.Assertions.Model.Properties.IAllureNameProperty<TSelf>";

        public const string IReadOnlyList = "System.Collections.Generic.IReadOnlyList<T>";
    }

}
