using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

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
    /// Creates a fully-qualified class name that uniquely identifies a given
    /// class.
    /// </summary>
    /// <remarks>
    /// A fully-qualified name of a type includes the assembly name, the
    /// namespace and the class name (can be a nested class).
    /// </remarks>
    /// <param name="targetClass">The type of a class.</param>
    public static string CreateFullName(Type targetClass) =>
        SerializeNonParameterClass(targetClass);

    /// <summary>
    /// Creates a string that unuquely identifies a given method.
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

    /// <summary>
    /// Creates a testCaseId value. testCaseId has a fixed length and depends
    /// only on a given fullName. The fullName shouldn't depend on test parameters.
    /// </summary>
    public static string CreateTestCaseId(string fullName) =>
        ToMD5(fullName);

    /// <summary>
    /// Creates a historyId value to be used by Allure Reporter. historyId has a
    /// fixed length and depends on a fullName and parameters of a test.
    /// Howewer, it doesn't depend on parametrs order as well as on parameter
    /// names in general. Parameters are sorted alphabetically by their names.
    /// Then, only the values are used to produce the final historyId value.
    /// </summary>
    /// <param name="fullName">The fullName of a test.</param>
    /// <param name="parameters">The parameters of a test.</param>
    public static string CreateHistoryId(
        string fullName,
        IEnumerable<Parameter> parameters
    ) =>
        ToMD5(
            JsonConvert.SerializeObject(
                new
                {
                    fullName,
                    parameters = parameters.Where(p => !p.excluded)
                        .OrderBy(p => p.name)
                        .Select(p => p.value)
                }
            )
        );

    static string ToMD5(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        var outputBytes = md5.ComputeHash(inputBytes);
        return ToHexString(outputBytes);
    }

    static string ToHexString(byte[] inputBytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in inputBytes)
        {
            sb.Append(
                b.ToString("x2")
            );
        }
        return sb.ToString();
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
        return SerializeNonParameterClass(type);
    }

    static string SerializeNonParameterClass(Type type) =>
        GetUniqueTypeName(type) + SerializeTypeParameterTypeList(
            type.GetGenericArguments()
        );

    static string GetUniqueTypeName(Type type) =>
        IsSystemType(type)
            ? ConstructFullName(type)
            : GetTypeNameWithAssembly(type);

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
        $"{type.Assembly.GetName().Name}:" + ConstructFullName(type);

    static bool IsSystemType(Type type) =>
        type.Assembly == systemTypesAssembly;

    static readonly Assembly systemTypesAssembly = typeof(object).Assembly;
}