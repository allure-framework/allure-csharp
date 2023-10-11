using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Allure.Net.Commons.Functions;

namespace Allure.Net.Commons.Steps
{
    public static class AllureStepParameterHelper
    {
        private const string Null = "null";
        private const string Unknown = "Unknown";

        private static readonly Regex argumentPattern = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);

        public static List<Parameter> CreateParameters(IEnumerable<object> arguments)
            => arguments.Select(CreateParameters).ToList();

        public static Parameter CreateParameters(object argument)
        {
            switch (argument)
            {
                case null:
                    return new Parameter
                    {
                        name = Unknown,
                        value = Null,
                    };
                case ICollection collection:
                    return new Parameter
                    {
                        name = argument.GetType().Name,
                        value = string.Join(", ", collection.Cast<object>().ToList()),
                    };
                default:
                    return new Parameter
                    {
                        name = argument.GetType().Name,
                        value = argument.ToString(),
                    };
            }
        }

        public static string GetStepName(
            string stepName,
            MethodBase methodBase,
            object[] arguments,
            IReadOnlyDictionary<Type, ITypeFormatter> formatters
        )
        {
            var initialStepName = stepName;

            if (string.IsNullOrWhiteSpace(stepName))
            {
                return "";
            }

            var showIndexWarning = false;

            var parameters = methodBase.GetParameters();
            var parameterIndex = parameters.ToDictionary(x => x.Name);

            var matches = argumentPattern.Matches(stepName);
            foreach (Match match in matches)
            {
                var pattern = match.Groups[1].Value;

                if (int.TryParse(pattern, out var index) &&
                    TryGetValue(arguments, index, out var value1)
                )
                {
                    //!_! apply {paramIndex} placeholder - i.e. {0}, {1}, ...
                    stepName = stepName?.Replace(
                        match.Value,
                        FormatFunctions.Format(value1, formatters)
                    );
                    showIndexWarning = true;
                }
                else if (parameterIndex.TryGetValue(pattern, out var parameter1) &&
                    TryGetValue(arguments, parameter1.Position, out var value2)
                )
                {
                    //!_! apply {paramName} placeholder - i.e. {requestId}, {isDelete}, ...
                    stepName = stepName?.Replace(
                        match.Value,
                        FormatFunctions.Format(value2, formatters)
                    );
                }
                else if (TrySplit(pattern, '.', out var parts) &&
                    parts.Length == 2 &&
                    parameterIndex.TryGetValue(parts[0], out var parameter2) &&
                    TryGetValue(
                        formatters,
                        arguments[parameter2.Position],
                        parts[1],
                        out var value3
                    )
                )
                {
                    //!_! apply {paramName.fieldOrProperty} placeholder - i.e. {request.Id}, ...
                    stepName = stepName?.Replace(match.Value, value3);
                }
            }

            if (showIndexWarning)
            {
                // TODO: provide error description link
                Console.Error.WriteLine(
                    "Indexed step arguments are obsolete. " +
                    $"Use named arguments in step name '{initialStepName}' instead."
                );
            }

            return stepName;
        }
        
        public static List<Parameter> GetStepParameters(
            MethodBase metadata,
            object[] args,
            IReadOnlyDictionary<Type, ITypeFormatter> formatters
        )
        {
            return metadata.GetParameters()
                .Select(x => (
                    name: x.GetCustomAttribute<AbstractNameAttribute>()?.Name ?? x.Name,
                    skip: x.GetCustomAttribute<AbstractSkipAttribute>() != null))
                .Zip(args,
                    (parameter, value) => parameter.skip
                        ? null
                        : new Parameter
                        {
                            name = parameter.name,
                            value = FormatFunctions.Format(value, formatters)
                        })
                .Where(x => x != null)
                .ToList();
        }

        private static bool TrySplit(string s, char separator, out string[] parts)
        {
            parts = s.Split(separator);
            return parts.Length > 0;
        }

        private static bool TryGetValue(object[] array, int index, out object value)
        {
            if (index < 0 || index >= array.Length)
            {
                value = null;
                return false;
            }

            value = array[index];
            return true;
        }

        /// <summary> Getting the value of field or property </summary>
        private static bool TryGetValue(
            IReadOnlyDictionary<Type, ITypeFormatter> formatters,
            object obj,
            string name,
            out string value
        )
        {
            value = Unknown;
            if (obj == null) return false;

            var field = obj.GetType()?.GetField(name);
            if (field != null)
            {
                value = FormatFunctions.Format(
                    field.GetValue(obj),
                    formatters
                );
                return true;
            }

            var prop = obj.GetType()?.GetProperty(name);
            if (prop != null)
            {
                value = FormatFunctions.Format(
                    prop.GetValue(obj, null),
                    formatters
                );
                return true;
            }

            value = Unknown;
            return false;
        }
    }
}