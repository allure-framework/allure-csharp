namespace Allure.Build.SourceGenerators.Assertions;

record class PropertyMetadata(
    string InterfaceName,
    string InterfaceFullName,
    string Name,
    string MethodName,
    string JsonName,
    string ValueType
);
