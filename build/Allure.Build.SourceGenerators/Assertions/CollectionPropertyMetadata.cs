namespace Allure.Build.SourceGenerators.Assertions;

public record class CollectionPropertyMetadata(
    string InterfaceName,
    string InterfaceFullName,
    string Name,
    string MethodName,
    string JsonName,
    string ValueType,
    string ItemMethodName,
    string ItemName,
    string ItemType,
    bool ItemHasName
) : PropertyMetadata(
    InterfaceName: InterfaceName,
    InterfaceFullName: InterfaceFullName,
    Name: Name,
    MethodName: MethodName,
    JsonName: JsonName,
    ValueType: ValueType);
