using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Allure.Sdk.Functions;

/// <summary>
/// Creates title paths used to organize tests in Allure Report.
/// </summary>
public static class Titles
{
    /// <summary>
    /// Creates a title path to a test class in the test result tree.
    /// </summary>
    /// <param name="type">The type representing a test class.</param>
    /// <remarks>
    /// A title path consists of:
    /// <list type="bullet">
    /// <item>assembly name</item>
    /// <item>elements of namespace</item>
    /// <item>name of type (including its declaring types, if any)</item>
    /// <item>type parameters (for generic type definitions)</item>
    /// <item>type arguments (for constructed generic types)</item>
    /// </list>
    /// The type node can be renamed by applying <see cref="AllureNameAttribute"/> to the class.
    /// </remarks>
    public static List<string> PathFor(Type type)
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
    /// Creates a title path to a test method in the test result tree.
    /// </summary>
    /// <param name="method">The test method.</param>
    /// <remarks>
    /// A title path consists of:
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
    public static List<string> PathFor(MethodInfo method)
    {
        var titlePath = PathFor(method.DeclaringType);
        if (method.GetParameters().Length > 0)
        {
            titlePath.Add(
                method.GetCustomAttribute<AllureNameAttribute>()?.Name
                    ?? ReflectionNames.ForMethodSignature(method)
            );
        }
        return titlePath;
    }
}
