using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Internal;

static class InternalAllureExtensions
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
        internal string? GetAllureNameFormat<TAttribute>()
            where TAttribute : Attribute, IAllureNameSource
        =>
            method.GetCustomAttribute<TAttribute>()?.Name;

        internal string? ConstructAllureName<TAttribute>(
            IAllureRuntimeEndpoint endpoint,
            IEnumerable<object?> arguments
        )
            where TAttribute : Attribute, IAllureNameSource
        =>
            GetAllureNameFormat<TAttribute>(method) is { } nameFormat
                ? ConstructAllureName(method, endpoint, nameFormat, arguments)
                : null;

        internal ImmutableArray<(ParameterInfo, Lazy<string>)> PrepareParametersForSerialization(
            IAllureRuntimeEndpoint endpoint,
            IEnumerable<object?> arguments
        ) =>
            [.. method.GetParameters()
                .Zip(
                    arguments,
                    (p, a) => (p, new Lazy<string>(
                        () => endpoint.ParameterSerializer.Serialize(a))))];

        internal string ConstructAllureName(
            IAllureRuntimeEndpoint endpoint,
            string nameFormat,
            IEnumerable<object?> arguments
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

        internal string? ConstructAllureName<TAttribute>(
            IEnumerable<(ParameterInfo parameter, Lazy<string> argument)> preparedParameters
        )
            where TAttribute : Attribute, IAllureNameSource
        =>
            GetAllureNameFormat<TAttribute>(method) is { } nameFormat
                ? ConstructAllureName(method, nameFormat, preparedParameters)
                : null;

        internal string ConstructAllureName(
            string nameFormat,
            IEnumerable<(ParameterInfo parameter, Lazy<string> argument)> preparedParameters
        )
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return method.Name;
            }

            var parameterToValueMap = preparedParameters
                .ToImmutableDictionary(
                    keySelector: static (t) => t.parameter.Name,
                    elementSelector: static (t) => t.argument
                );

            return placeholderPattern.Replace(
                nameFormat,
                (match) =>
                {
                    var name = match.Groups[1].Value;
                    if (parameterToValueMap.TryGetValue(name, out var lazy))
                    {
                        return lazy.Value;
                    }

                    return match.Value;
                }
            );
        }
    }

    static readonly Regex placeholderPattern = new(@"\{([^}]+)\}", RegexOptions.Compiled);
}
