using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Internal;

public static class InternalAllureExtensions
{
    extension (Severity severity)
    {
        internal string ToLabelValue() => severity switch
        {
            Severity.Blocker => "blocker",
            Severity.Critical => "critical",
            Severity.Normal => "normal",
            Severity.Minor => "minor",
            Severity.Trivial => "trivial",
            _ => throw new InvalidOperationException(
                $"Unknown severity {severity}"
            ),
        };
    }

    extension (Exception exception)
    {
        internal StatusDetails ToAllureStatusDetails() =>
            new()
            {
                Message = exception.Message is { Length: >0 } message
                    ? message
                    : exception.GetType().Name,
                Trace = exception.ToString()
            };

        internal GlobalError ToAllureGlobalError() =>
            new()
            {
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Message = exception.Message is { Length: >0 } message
                    ? message
                    : exception.GetType().Name,
                Trace = exception.ToString()
            };
    }

    extension (MethodBase method)
    {
        internal string? GetAllureNameFormat<T>()
            where T : Attribute, IAllureNameSource
        =>
            method.GetCustomAttribute<T>()?.Name;

        internal string? ConstructAllureName<T>(
            IAllureApiEndpoint endpoint,
            params IEnumerable<object?> arguments
        )
            where T : Attribute, IAllureNameSource
        =>
            GetAllureNameFormat<T>(method) is { } nameFormat
                ? ConstructAllureName(method, endpoint, nameFormat, arguments)
                : null;

        internal string ConstructAllureName(
            IAllureApiEndpoint endpoint,
            string nameFormat,
            params IEnumerable<object?> arguments
        )
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return "";
            }

            var parameterToArgumentMap = method.GetParameters()
                .Zip(arguments, static (p, a) => new KeyValuePair<string, object?>(p.Name, a))
                .ToImmutableDictionary();

            var argTextCache = new Dictionary<string, string>();

            return placeholderPattern.Replace(
                nameFormat,
                (match) =>
                {
                    var name = match.Groups[1].Value;
                    if (argTextCache.TryGetValue(name, out var cachedText))
                    {
                        return cachedText;
                    }

                    if (parameterToArgumentMap.TryGetValue(name, out var value))
                    {
                        var newText = endpoint.ParameterSerializer.Serialize(value);
                        argTextCache[name] = newText;
                        return newText;
                    }

                    return match.Value;
                }
            );
        }

        internal string? ConstructAllureName<T>(params IEnumerable<string> serializedArguments)
            where T : Attribute, IAllureNameSource
        =>
            GetAllureNameFormat<T>(method) is { } nameFormat
                ? ConstructAllureName(method, nameFormat, serializedArguments)
                : null;

        internal string ConstructAllureName(string nameFormat, params IEnumerable<string> serializedArguments)
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return method.Name;
            }

            var parameterToValueMap = method.GetParameters()
                .Zip(serializedArguments, static (p, a) => new KeyValuePair<string, string>(p.Name, a))
                .ToImmutableDictionary();

            return placeholderPattern.Replace(
                nameFormat,
                (match) =>
                {
                    var name = match.Groups[1].Value;
                    if (parameterToValueMap.TryGetValue(name, out var value))
                    {
                        return value;
                    }

                    return match.Value;
                }
            );
        }

        internal string ConstructAllureName(
            string nameFormat,
            params IEnumerable<Parameter> allureParameters
        ) =>
            ConstructAllureName(
                method,
                nameFormat,
                allureParameters.Select(static (p) => p.Value)
            );

        internal List<Parameter> ConstructAllureParameters(
            IAllureApiEndpoint endpoint,
            params IEnumerable<object?> arguments
        ) => [
            .. method
                .GetParameters()
                .Zip(arguments, static (p, a) => (
                    parameter: p,
                    argument: a,
                    data: p.GetCustomAttribute<AllureParameterAttribute>()))
                .Where(static (t) => !t.data.Ignore)
                .Select((t) => new Parameter
                {
                    Name = t.data.Name ?? t.parameter.Name,
                    Value = endpoint.ParameterSerializer.Serialize(t.argument),
                    Mode = t.data.Mode,
                    Excluded = t.data.Excluded,
                }),
        ];
    }

    static readonly Regex placeholderPattern = new(@"\{([^}]+)\}", RegexOptions.Compiled);
}