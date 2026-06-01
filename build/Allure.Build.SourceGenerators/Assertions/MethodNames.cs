namespace Allure.Build.SourceGenerators.Assertions;

public record class MethodNames(
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
                PropertyExistsAnyValue: ccProperty.MethodName,
                PropertyEquals: ccProperty.MethodName,
                PropertyEqualsCustom: ccProperty.MethodName,
                PropertySatisfiesConstraints: ccProperty.MethodName,
                SingleItem: $"Single{ccProperty.ItemMethodName}",
                SingleItemByCriteria: $"OnlyOne{ccProperty.ItemMethodName}",
                SingleItemByName: $"OnlyOne{ccProperty.ItemMethodName}",
                SingleItemByNameComparator: $"OnlyOne{ccProperty.ItemMethodName}",
                ItemByIndex: $"{ccProperty.ItemMethodName}At",
                ItemsSatisfyConstraints: $"{ccProperty.MethodName}"
            ),
        _ => new(
            PropertyExistsAnyValue: property.MethodName,
            PropertyEquals: property.MethodName,
            PropertyEqualsCustom: property.MethodName,
            PropertySatisfiesConstraints: property.MethodName
        ),
    };

    public static MethodNames ForAssertionSource(PropertyMetadata property) => property switch
    {
        CollectionPropertyMetadata ccProperty =>
            new(
                PropertyExistsAnyValue: $"Has{ccProperty.MethodName}",
                PropertyEquals: $"Has{ccProperty.MethodName}",
                PropertyEqualsCustom: $"Has{ccProperty.MethodName}",
                PropertySatisfiesConstraints: $"Has{ccProperty.MethodName}",
                SingleItem: $"HasSingle{ccProperty.ItemMethodName}",
                SingleItemByCriteria: $"HasOnlyOne{ccProperty.ItemMethodName}",
                SingleItemByName: $"HasOnlyOne{ccProperty.ItemMethodName}",
                SingleItemByNameComparator: $"HasOnlyOne{ccProperty.ItemMethodName}",
                ItemByIndex: $"Has{ccProperty.ItemMethodName}At",
                ItemsSatisfyConstraints: $"Has{ccProperty.MethodName}"
            ),
        _ => new(
            PropertyExistsAnyValue: $"Has{property.MethodName}",
                PropertyEquals: $"Has{property.MethodName}",
                PropertyEqualsCustom: $"Has{property.MethodName}",
                PropertySatisfiesConstraints: $"Has{property.MethodName}"
        ),
    };
}