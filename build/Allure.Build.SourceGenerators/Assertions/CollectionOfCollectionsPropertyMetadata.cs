using System.Collections.Immutable;

namespace Allure.Build.SourceGenerators.Assertions;

public record class CollectionOfCollectionsPropertyMetadata(
    string InterfaceName,
    string InterfaceFullName,
    string Name,
    string MethodName,
    string JsonName,
    string ValueType,
    ImmutableArray<string> EquatableTypes,
    string ItemMethodName,
    string ItemName,
    string ItemType,
    bool ItemHasName,
    string ItemItemType
) : CollectionPropertyMetadata(
    InterfaceName: InterfaceName,
    InterfaceFullName: InterfaceFullName,
    Name: Name,
    MethodName: MethodName,
    JsonName: JsonName,
    ValueType: ValueType,
    EquatableTypes: EquatableTypes,
    ItemMethodName: ItemMethodName,
    ItemName: ItemName,
    ItemType: ItemType,
    ItemHasName: ItemHasName);