using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Functions;

static class TestMethodIdentifiers
{
    public static string? AssemblyName(TestMethodIdentifierProperty identifier)
    {
        var assemblyName = identifier.AssemblyFullName;
        return assemblyName?.Contains(",") is true
            ? new AssemblyName(assemblyName).Name
            : assemblyName;
    }

    public static string FullName(TestMethodIdentifierProperty identifier) =>
        FullName(identifier, AssemblyName(identifier));

    public static List<string> TitlePath(TestMethodIdentifierProperty identifier) =>
        [.. TitlePath(identifier, AssemblyName(identifier))];

    static string FullName(
        TestMethodIdentifierProperty identifier,
        string? assemblyName
    )
    {
        var result = new StringBuilder();

        if (assemblyName is not null)
        {
            result.Append(assemblyName);
            result.Append(":");
        }

        if (identifier.Namespace is { } @namespace)
        {
            result.Append(@namespace);
            result.Append(".");
        }

        if (identifier.TypeName is { } typeName)
        {
            result.Append(typeName);
            result.Append(".");
        }

        if (identifier.MethodName is { } methodName)
        {
            result.Append(methodName);
        }

        result.Append("(");
        result.Append(string.Join(",", identifier.ParameterTypeFullNames));
        result.Append(")");

        return result.ToString();
    }

    static IEnumerable<string> TitlePath(
        TestMethodIdentifierProperty identifier,
        string? assemblyName
    )
    {
        if (assemblyName is not null)
        {
            yield return assemblyName;
        }

        if (identifier.Namespace is { } @namespace)
        {
            foreach (var namespacePart in @namespace.Split('.'))
            {
                yield return namespacePart;
            }
        }

        if (identifier.TypeName is { } typeName)
        {
            yield return typeName;
        }

        var parameterTypes = string.Join(",", identifier.ParameterTypeFullNames);
        if (parameterTypes.Length > 0)
        {
            yield return $"{identifier.MethodName}({parameterTypes})";
        }
    }
}
