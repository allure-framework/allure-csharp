using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Allure.Build.SourceGenerators.Assertions;

public record class AttributeProperties(
    string PropertyName,
    string MethodName,
    string JsonName,
    string ItemMethodName,
    string ItemName
)
{
    static readonly Regex propertyNamePattern = new(@"^IAllure(?<name>\w+)Property$");

    static readonly Regex wsBeforeCapitalPattern = new(@"(?<!^)(?=[A-Z])");

    public static AttributeProperties? Resolve(
        INamedTypeSymbol attributeTypeSymbol,
        INamedTypeSymbol interfaceTypeSymbol
    )
    {
        string? propertyName = null;
        string? jsonName = null;
        string? methodName = null;
        string? itemMethodName = null;
        string? itemName = null;

        foreach (var attributeData in interfaceTypeSymbol.GetAttributes())
        {
            if (!attributeTypeSymbol.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
            {
                continue;
            }

            foreach (var kv in attributeData.NamedArguments)
            {
                if (kv.Key == "PropertyName" && kv.Value.Value?.ToString() is { } providedPropertyName)
                {
                    propertyName = providedPropertyName;
                }

                if (kv.Key == "JsonName" && kv.Value.Value?.ToString() is { } providedJsonName)
                {
                    jsonName = providedJsonName;
                }

                if (kv.Key == "MethodName" && kv.Value.Value?.ToString() is { } providedMethodName)
                {
                    methodName = providedMethodName;
                }

                if (kv.Key == "ItemMethodName" && kv.Value.Value?.ToString() is { } providedItemMethodName)
                {
                    itemMethodName = providedItemMethodName;
                }

                if (kv.Key == "ItemName" && kv.Value.Value?.ToString() is { } providedItemName)
                {
                    itemName = providedItemName;
                }
            }
        }

        propertyName ??= GetPropertyName(interfaceTypeSymbol.Name);
        if (propertyName is null)
        {
            return null;
        }

        methodName ??= propertyName;

        itemMethodName ??= methodName[methodName.Length - 1] == 's'
            ? methodName.Substring(0, methodName.Length - 1)
            : $"{methodName}Item";

        return new(
            PropertyName: propertyName,
            MethodName: methodName,
            JsonName: jsonName ?? GetJsonName(propertyName),
            ItemMethodName: itemMethodName,
            ItemName: itemName ?? GetItemName(itemMethodName)
        );
    }

    static string? GetPropertyName(string propertyInterfaceName)
    {
        var propertyNameMatch = propertyNamePattern.Match(propertyInterfaceName);
        if (!propertyNameMatch.Success)
        {
            return null;
        }

        return propertyNameMatch.Groups["name"].Value;
    }

    static string GetJsonName(string propertyName) =>
        char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

    static string GetItemName(string itemMethodName) =>
        wsBeforeCapitalPattern
            .Replace(itemMethodName, " ")
            .ToLowerInvariant();
}