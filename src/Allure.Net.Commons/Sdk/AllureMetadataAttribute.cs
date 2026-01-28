using System;
using System.Linq;
using System.Reflection;

#nullable enable

namespace Allure.Net.Commons.Sdk;

/// <summary>
/// A base class for attributes that apply metadata to test results.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Method
        | AttributeTargets.Interface,
    AllowMultiple = true,
    Inherited = true
)]
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
    /// Applies metadata attributes of a <paramref name="method"/> and its base methods to
    /// <paramref name="testResult"/>.
    /// </summary>
    public static void ApplyMethodAttributes(TestResult testResult, MethodInfo method)
    {
        var methodAttributes
            = method
                .GetCustomAttributes<AllureMetadataAttribute>()
                .Reverse();

        foreach (var attr in methodAttributes)
        {
            attr.Apply(testResult);
        }
    }

    /// <summary>
    /// Applies metadata attributes of a <paramref name="type"/>, its base types, and implemented
    /// interfaces to <paramref name="testResult"/>.
    /// </summary>
    public static void ApplyTypeAttributes(TestResult testResult, Type type)
    {
        var methodAttributes = type.GetCustomAttributes<AllureMetadataAttribute>(true);

        var interfaceAttributes
            = type
                .GetInterfaces()
                .SelectMany(static (iFace) =>
                    iFace.GetCustomAttributes<AllureMetadataAttribute>(true));

        var allAttributes
            = methodAttributes
                .Concat(interfaceAttributes)
                .Reverse();

        foreach (var attr in allAttributes)
        {
            attr.Apply(testResult);
        }
    }

    /// <summary>
    /// Applies metadata attributes of a <paramref name="method"/> and its declaring type
    /// to a test result.
    /// </summary>
    /// <remarks>
    /// Some applications are order-dependent (e.g., the application of
    /// <see cref="Attributes.AllureDescriptionAttribute"/>). Here are the rules that define the
    /// order:
    /// <list type="number">
    /// <item>Interfaces are handled before classes/structs.</item>
    /// <item>Base classes/structs are handled before derived classes/structs.</item>
    /// <item>Classes/structs are handled before methods</item>
    /// <item>Base methods are handled before methods overrides.</item>
    /// </list>
    /// </remarks>
    public static void ApplyAllAttributes(TestResult testResult, MethodInfo method)
    {
        ApplyTypeAttributes(testResult, method.DeclaringType);
        ApplyMethodAttributes(testResult, method);
    }
}