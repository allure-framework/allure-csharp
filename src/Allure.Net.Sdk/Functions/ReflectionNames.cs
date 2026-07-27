using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allure.Sdk.Functions;

public static class ReflectionNames
{
    public static string ForType(Type type) =>
        type.IsGenericParameter ? type.Name : SerializeNonParameterType(type);

    public static string ForTypeMethod(MethodInfo method)
    {
        if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
        {
            method = method.GetGenericMethodDefinition();
        }

        var methodName = method.Name;
        var typeParameters = method.GetGenericArguments();
        var typeParametersDecl = ForTypeArguments(typeParameters);
        var parameterTypes = ForParameters(method.GetParameters());
        return $"{methodName}{typeParametersDecl}({parameterTypes})";
    }

    public static string ForMethod(MethodInfo method) =>
        $"{ForType(method.DeclaringType)}.{ForMethod(method)}";

    public static string ForParameters(
        IEnumerable<ParameterInfo> parameters
    ) =>
        SerializeTypeList(
            parameters.Select(p => p.ParameterType)
        );

    public static string ForTypeArguments(IEnumerable<Type> types) =>
        types.Any() ? SerializeNonEmptyTypeArgumentList(types) : "";

    static string SerializeNonEmptyTypeArgumentList(IEnumerable<Type> types) =>
        "[" + SerializeTypeList(types) + "]";

    static string SerializeTypeList(
        IEnumerable<Type> types
    ) =>
        string.Join(
            ",",
            types.Select(ForType)
        );

    static string SerializeNonParameterType(Type type) =>
        GetUniqueTypeName(type) + ForTypeArguments(
            type.GetGenericArguments()
        );

    static string GetUniqueTypeName(Type type) =>
        IsSystemType(type)
            ? ConstructFullName(type)
            : GetTypeNameWithAssembly(type);

    static string ConstructFullName(Type type) =>
        type.IsNested
            ? ConstructFullNameOfNestedType(type)
            : ConstructFullNameOfOutmostType(type);

    static string ConstructFullNameOfNestedType(Type type) =>
        ConstructFullName(type.DeclaringType) + "+" + type.Name;

    static string ConstructFullNameOfOutmostType(Type type) =>
        string.IsNullOrEmpty(type.Namespace)
            ? type.Name
            : $"{type.Namespace}.{type.Name}";

    static string GetTypeNameWithAssembly(Type type) =>
        $"{type.Assembly.GetName().Name}:" + ConstructFullName(type);

    static bool IsSystemType(Type type) =>
        type.Assembly == systemTypesAssembly;

    static readonly Assembly systemTypesAssembly = typeof(object).Assembly;
}