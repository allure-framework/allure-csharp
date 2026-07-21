using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Allure.Net.Tests.Infrastructure;

static class PublicApiSnapshot
{
    static readonly NullabilityInfoContext nullability = new();

    public static string Create(params IEnumerable<Type> types) =>
        string.Join("\n", types
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .SelectMany(TypeLines));

    public static string Hash(string snapshot) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(snapshot)));

    static IEnumerable<string> TypeLines(Type type)
    {
        yield return $"type {TypeName(type)} : {string.Join(", ", type.GetInterfaces().Select(TypeName).Order(StringComparer.Ordinal))}";

        foreach (var constructor in type.GetConstructors().OrderBy(Signature))
        {
            yield return $"  ctor({Parameters(constructor)})";
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).OrderBy(property => property.Name))
        {
            var accessors = $"{(property.CanRead ? "get;" : "")}{(property.CanWrite ? "set;" : "")}";
            yield return $"  property {NullableType(property.PropertyType, nullability.Create(property).ReadState)} {property.Name} {{{accessors}}}";
        }

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .OrderBy(Signature))
        {
            var generic = method.IsGenericMethodDefinition
                ? $"<{string.Join(",", method.GetGenericArguments().Select(argument => argument.Name))}>"
                : "";
            yield return $"  method {NullableType(method.ReturnType, nullability.Create(method.ReturnParameter).ReadState)} {method.Name}{generic}({Parameters(method)})";
        }
    }

    static string Parameters(MethodBase method) =>
        string.Join(", ", method.GetParameters().Select(parameter =>
        {
            var modifier = parameter.IsOut ? "out " : parameter.ParameterType.IsByRef ? "ref " : "";
            var type = parameter.ParameterType.IsByRef
                ? parameter.ParameterType.GetElementType()!
                : parameter.ParameterType;
            return $"{modifier}{NullableType(type, nullability.Create(parameter).ReadState)} {parameter.Name}";
        }));

    static string NullableType(Type type, NullabilityState state) =>
        TypeName(type) + (state == NullabilityState.Nullable && !type.IsValueType ? "?" : "");

    static string TypeName(Type type)
    {
        if (type.IsGenericParameter) return type.Name;
        if (type.IsArray) return $"{TypeName(type.GetElementType()!)}[]";
        if (!type.IsGenericType) return type.FullName ?? type.Name;

        var name = type.GetGenericTypeDefinition().FullName!;
        name = name[..name.IndexOf('`')];
        return $"{name}<{string.Join(",", type.GetGenericArguments().Select(TypeName))}>";
    }

    static string Signature(MethodBase method) =>
        $"{method.Name}`{(method.IsGenericMethod ? method.GetGenericArguments().Length : 0)}({string.Join(",", method.GetParameters().Select(parameter => parameter.ParameterType.FullName))})";
}
