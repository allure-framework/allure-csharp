using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Sdk.Functions;

/// <summary>
/// Creates Allure parameters from method parameters and argument values.
/// </summary>
public static class Parameters
{
    /// <summary>
    /// Creates Allure parameters using metadata from reflected method parameters.
    /// </summary>
    /// <param name="parameters">The reflected parameter metadata.</param>
    /// <param name="values">The corresponding argument values.</param>
    /// <param name="parameterSerializer">The serializer used for argument values.</param>
    /// <returns>The created parameters.</returns>
    public static IEnumerable<Parameter> Create(
        IEnumerable<ParameterInfo> parameters,
        IEnumerable<object?> values,
        IAllureParameterSerializer parameterSerializer
    ) =>
        Create(
            parameters.Select(static p => p.Name),
            parameters.Select(static p =>
                p.GetCustomAttribute<AllureParameterAttribute>()),
            values,
            parameterSerializer
        );

    /// <summary>
    /// Creates Allure parameters from separate names, attributes, and values.
    /// </summary>
    /// <param name="parameterNames">The source parameter names.</param>
    /// <param name="attributes">The corresponding Allure parameter attributes.</param>
    /// <param name="values">The corresponding argument values.</param>
    /// <param name="parameterSerializer">The serializer used for argument values.</param>
    /// <returns>The created parameters.</returns>
    public static IEnumerable<Parameter> Create(
        IEnumerable<string> parameterNames,
        IEnumerable<AllureParameterAttribute?> attributes,
        IEnumerable<object?> values,
        IAllureParameterSerializer parameterSerializer
    ) =>
        parameterNames
            .Zip(attributes, static (n, a) => (name: n, attr: a))
            .Zip(values, static (p, v) => (p.name, p.attr, value: v))
            .Where(static (tuple) => tuple.attr?.Ignore is not true)
            .Select((tuple) =>
                CreateParameter(tuple.name, tuple.attr, tuple.value, parameterSerializer));

    static Parameter CreateParameter(
        string parameterName,
        AllureParameterAttribute? attribute,
        object? value,
        IAllureParameterSerializer parameterSerializer
    ) =>
        new()
        {
            Name = attribute?.Name ?? parameterName,
            Value = parameterSerializer.Serialize(value),
            Excluded = attribute?.Excluded == true,
            Mode = ResolveParameterMode(attribute)
        };

    static ParameterMode? ResolveParameterMode(AllureParameterAttribute? attribute) =>
        attribute is AllureParameterAttribute { Mode: ParameterMode mode and not ParameterMode.Default }
            ? mode
            : null;
}
