using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// A base class for attributes that apply metadata to test results.
/// </summary>
public abstract class AllureApiAttribute : Attribute
{
    /// <summary>
    /// Default targets for Allure metadata attributes.
    /// </summary>
    public const AttributeTargets ALLURE_METADATA_TARGETS
        = AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Method
            | AttributeTargets.Interface;

    /// <summary>
    /// Applies the attribute to a test result.
    /// </summary>
    public abstract void Apply(TestResult testResult);

    /// <summary>
    /// Returns metadata attributes of a <paramref name="method"/> and its base methods.
    /// </summary>
    /// <remarks>
    /// Attributes of the base methods (virtual or abstract) are guaranteed to appear
    /// before attributes of the derived ones.
    /// </remarks>
    public static IEnumerable<AllureApiAttribute> GetMethodAttributes(MethodInfo method)
        => method
            .GetCustomAttributes<AllureApiAttribute>()
            .Reverse();

    /// <summary>
    /// Returns metadata attributes of a <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    /// Here are the guarantees about the order:
    /// <list type="number">
    /// <item>Attributes of interfaces before attributes of classes/structs.</item>
    /// <item>
    /// Attributes of base classes before attributes of derived classes.
    /// </item>
    /// </list>
    /// </remarks>
    public static IEnumerable<AllureApiAttribute> GetTypeAttributes(Type type)
        => type
            .GetCustomAttributes<AllureApiAttribute>()
            .Concat(
                type
                    .GetInterfaces()
                    .SelectMany(static (iFace) =>
                        iFace.GetCustomAttributes<AllureApiAttribute>()))
            .Reverse();

    /// <summary>
    /// Returns metadata attributes of a <paramref name="method"/> and its declaring
    /// type.
    /// </summary>
    /// <remarks>
    /// Here are the guarantees about the order:
    /// <list type="number">
    /// <item>Attributes of interfaces before attributes of classes/structs.</item>
    /// <item>
    /// Attributes of base classes before attributes of derived classes.
    /// </item>
    /// <item>Attributes of classes/structs before attributes of methods</item>
    /// <item>Attributes of base methods before attributes of methods overrides.</item>
    /// </list>
    /// </remarks>
    public static IEnumerable<AllureApiAttribute> GetAllAttributes(MethodInfo method)
        => GetTypeAttributes(method.DeclaringType)
            .Concat(
                GetMethodAttributes(method)
            );

    /// <summary>
    /// Applies metadata attributes of a <paramref name="method"/> and its base methods to
    /// <paramref name="testResult"/>.
    /// </summary>
    /// <remarks>
    /// Attributes of the base methods (virtual or abstract) are guaranteed to be applied before
    /// attributes of the derived ones.
    /// </remarks>
    public static void ApplyMethodAttributes(MethodInfo method, TestResult testResult)
    {
        foreach (var attr in GetMethodAttributes(method))
        {
            attr.Apply(testResult);
        }
    }

    /// <summary>
    /// Applies metadata attributes of a <paramref name="type"/>, its base types, and implemented
    /// interfaces to <paramref name="testResult"/>.
    /// </summary>
    /// <remarks>
    /// The following is guaranteed about the order of application:
    /// <list type="number">
    /// <item>Interfaces are handled before classes/structs.</item>
    /// <item>Base classes are handled before derived classes.</item>
    /// </list>
    /// </remarks>
    public static void ApplyTypeAttributes(Type type, TestResult testResult)
    {
        var typeAttributesToApply
            = GetTypeAttributes(type)
                .Where(static (a) => a is not AllureNameAttribute);

        foreach (var attr in typeAttributesToApply)
        {
            attr.Apply(testResult);
        }
    }

    /// <summary>
    /// Applies metadata attributes of a <paramref name="method"/> and its declaring type
    /// to a test result.
    /// </summary>
    /// <remarks>
    /// The following is guaranteed about the order of application:
    /// <list type="number">
    /// <item>Interfaces are handled before classes/structs.</item>
    /// <item>Base classes are handled before derived classes.</item>
    /// <item>Classes/structs are handled before methods</item>
    /// <item>Base methods are handled before methods overrides.</item>
    /// </list>
    /// </remarks>
    public static void ApplyAllAttributes(MethodInfo method, TestResult testResult)
    {
        ApplyTypeAttributes(method.DeclaringType, testResult);
        ApplyMethodAttributes(method, testResult);
    }

    /// <summary>
    /// Applies <paramref name="attributes"/> to <paramref name="testResult"/>.
    /// </summary>
    public static void ApplyAttributes(
        IEnumerable<AllureApiAttribute> attributes,
        TestResult testResult
    )
    {
        foreach (var attribute in attributes)
        {
            attribute.Apply(testResult);
        }
    }
}