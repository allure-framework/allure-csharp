namespace Allure.Build.SourceGenerators.Assertions;

public record class PropertyMetadata(
    string InterfaceName,
    string InterfaceFullName,
    string Name,
    string MethodName,
    string JsonName,
    string ValueType
);
