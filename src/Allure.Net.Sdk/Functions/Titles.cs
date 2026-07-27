using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allure.Sdk.Functions;

public static class Titles
{
    /// <summary>
    /// Creates a titlePath: a path to a test class in a tree of test results.
    /// </summary>
    /// <param name="type">A type representing a test class</param>
    /// <remarks>
    /// A titlePath consists of:
    /// <list type="bullet">
    /// <item>assembly name</item>
    /// <item>elements of namespace</item>
    /// <item>name of type (including its declaring types, if any)</item>
    /// <item>type parameters (for generic type definitions)</item>
    /// <item>type arguments (for constructed generic types)</item>
    /// </list>
    /// The type node can be renamed by applying <see cref="AllureNameAttribute"/> to the class.
    /// </remarks>
    public static List<string> PathForType(Type type)
    {
        static IEnumerable<string> ExpandNestness(Type type)
        {
            for (; type.IsNested; type = type.DeclaringType)
                yield return type.Name;
            yield return type.Name;
        }

        var assemblyName = type.Assembly.GetName().Name;
        var namespaceParts = (type.Namespace ?? "").Split('.').Where(s => s.Length > 0);
        var typeNode = type.GetCustomAttribute<AllureNameAttribute>()?.Name
            ?? (string.Join("+", ExpandNestness(type).Reverse()) +
                ReflectionNames.ForTypeArguments(
                    type.GetGenericArguments()));

        return [
            assemblyName,
            .. namespaceParts,
            typeNode,
        ];
    }

    /// <summary>
    /// Creates a titlePath: a path to a test class in a tree of test results.
    /// </summary>
    /// <param name="method">A test method</param>
    /// <remarks>
    /// A titlePath consists of:
    /// <list type="bullet">
    /// <item>assembly name</item>
    /// <item>elements of namespace</item>
    /// <item>name of type (including its declaring types, if any)</item>
    /// <item>type parameters (for generic type definitions)</item>
    /// <item>type arguments (for constructed generic types)</item>
    /// <item>method name with type parameters and parameter types (for parameterized method)</item>
    /// </list>
    /// The type and method nodes can be renamed by applying <see cref="AllureNameAttribute"/>.
    /// </remarks>
    public static List<string> PathForMethod(MethodInfo method)
    {
        var titlePath = PathForType(method.DeclaringType);
        if (method.GetParameters().Length > 0)
        {
            titlePath.Add(
                method.GetCustomAttribute<AllureNameAttribute>()?.Name
                    ?? ReflectionNames.ForMethod(method)
            );
        }
        return titlePath;
    }
}