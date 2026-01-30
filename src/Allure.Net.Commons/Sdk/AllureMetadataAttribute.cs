using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable

namespace Allure.Net.Commons.Sdk;

/// <summary>
/// A base class for attributes that apply metadata to test results.
/// </summary>
public abstract class AllureMetadataAttribute : Attribute
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
    public static IEnumerable<AllureMetadataAttribute> GetMethodAttributes(MethodInfo method)
        => method
            .GetCustomAttributes<AllureMetadataAttribute>()
            .Reverse();

    /// <summary>
    /// Returns metadata attributes of a <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    /// Here are the guarantees about the order:
    /// <list type="number">
    /// <item>Attributes of interfaces before attributes of classes/structs.</item>
    /// <item>
    /// Attributes of base classes/structs before attributes of derived classes/structs.
    /// </item>
    /// </list>
    /// </remarks>
    public static IEnumerable<AllureMetadataAttribute> GetTypeAttributes(Type type)
        => type
            .GetCustomAttributes<AllureMetadataAttribute>()
            .Concat(
                type
                    .GetInterfaces()
                    .SelectMany(static (iFace) =>
                        iFace.GetCustomAttributes<AllureMetadataAttribute>()))
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
    /// Attributes of base classes/structs before attributes of derived classes/structs.
    /// </item>
    /// <item>Attributes of classes/structs before attributes of methods</item>
    /// <item>Attributes of base methods before attributes of methods overrides.</item>
    /// </list>
    /// </remarks>
    public static IEnumerable<AllureMetadataAttribute> GetAllAttributes(MethodInfo method)
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
    public static void ApplyMethodAttributes(TestResult testResult, MethodInfo method)
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
    /// <item>Base classes/structs are handled before derived classes/structs.</item>
    /// </list>
    /// </remarks>
    public static void ApplyTypeAttributes(TestResult testResult, Type type)
    {
        foreach (var attr in GetTypeAttributes(type))
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
    /// <item>Base classes/structs are handled before derived classes/structs.</item>
    /// <item>Classes/structs are handled before methods</item>
    /// <item>Base methods are handled before methods overrides.</item>
    /// </list>
    /// </remarks>
    public static void ApplyAllAttributes(TestResult testResult, MethodInfo method)
    {
        foreach (var attr in GetAllAttributes(method))
        {
            attr.Apply(testResult);
        }
    }

    /// <summary>
    /// Applies <paramref name="attributes"/> to <paramref name="testResult"/>.
    /// </summary>
    public static void ApplyAttributes(
        TestResult testResult,
        IEnumerable<AllureMetadataAttribute> attributes
    )
    {
        foreach (var attribute in attributes)
        {
            attribute.Apply(testResult);
        }
    }
}