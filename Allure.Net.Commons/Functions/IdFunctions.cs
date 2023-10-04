using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allure.Net.Commons.Functions;

/// <summary>
/// Functions to create UUIDs, full names, etc.
/// </summary>
public static class IdFunctions
{
    /// <summary>
    /// Generates a UUID for a test or container.
    /// </summary>
    public static string CreateUUID() => Guid.NewGuid().ToString();

    /// <summary>
    /// Creates a string that unuquely describes a given method.
    /// </summary>
    /// <remarks>
    /// For a given test method the full name includes:
    /// <list type="bullet">
    /// <item>
    /// fully-qualified name of the declaring type (including type parameters)
    /// </item>
    /// <item>name of the method</item>
    /// <item>generic parameters of the method</item>
    /// <item>
    /// fully-qualified names of the parameter types, (including parameter
    /// modifiers, if any)
    /// </item>
    /// </list>
    /// A fully-qualified name of a type includes the assembly name, the
    /// namespace and the class name (can be a nested class).
    /// </remarks>
    public static string CreateFullName(MethodBase method)
    {
        var className = SerializeType(method.DeclaringType);
        var methodName = method.Name;
        var typeParameters = method.GetGenericArguments();
        var typeParametersDecl = SerializeTypeParameterTypeList(typeParameters);
        var parameterTypes = SerializeParameterTypes(method.GetParameters());
        return $"{className}.{methodName}{typeParametersDecl}({parameterTypes})";
    }

    static string SerializeParameterTypes(
        IEnumerable<ParameterInfo> parameters
    ) =>
        SerializeTypeList(
            parameters.Select(p => p.ParameterType)
        );

    static string SerializeTypeParameterTypeList(IEnumerable<Type> types) =>
        types.Any() ? SerializeNonEmptyTypeParameterTypeList(types) : "";

    static string SerializeNonEmptyTypeParameterTypeList(IEnumerable<Type> types) =>
        "[" + SerializeTypeList(types) + "]";

    static string SerializeTypeList(
        IEnumerable<Type> types
    ) =>
        string.Join(
            ",",
            types.Select(SerializeType)
        );

    static string SerializeType(Type type)
    {
        if (type.IsGenericParameter)
        {
            return type.Name;
        }
        return GetUniqueTypeName(type) + SerializeTypeParameterTypeList(
            type.GetGenericArguments()
        );
    }

    static string GetUniqueTypeName(Type type) =>
        IsSystemType(type)
            ? ResolveFullName(type)
            : GetTypeNameWithAssembly(type);

    static string ResolveFullName(Type type) =>
        type.FullName ?? ConstructFullName(type);

    static string ConstructFullName(Type type) =>
        type.DeclaringType is null
            ? ConstructFullNameOfRootClass(type)
            : ConstructFullNameOfNestedClass(type);

    static string ConstructFullNameOfNestedClass(Type type) =>
        ConstructFullName(type.DeclaringType) + "+" + type.Name;

    static string ConstructFullNameOfRootClass(Type type) =>
        string.IsNullOrEmpty(type.Namespace)
            ? type.Name
            : $"{type.Namespace}.{type.Name}";

    static string GetTypeNameWithAssembly(Type type) =>
        $"{type.Assembly.GetName().Name}:" + ResolveFullName(type);

    static bool IsSystemType(Type type) =>
        type.Assembly == systemTypesAssembly;

    static readonly Assembly systemTypesAssembly = typeof(object).Assembly;
}