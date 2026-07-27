using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Sdk.Functions;

public static class Parameters
{
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
