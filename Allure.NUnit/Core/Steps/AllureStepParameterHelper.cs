using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;

namespace NUnit.Allure.Core.Steps
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
                        value = string.Join(", ", collection.Cast<object>().ToList())
                    };
                default:
                    return new Parameter
                    {
                        name = argument.GetType().Name,
                        value = argument.ToString(),
                    };
            }
        }

        public static string ApplyValuesToPlaceholders(string stepName, MethodBase methodBase, object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(stepName))
            {
                return "";
            }

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
                    //!_! apply {paramPosition} placeholder
                    stepName = stepName?.Replace(match.Value, value1?.ToString() ?? "null");
                }
                else if (parameterIndex.TryGetValue(pattern, out var parameter1) &&
                    TryGetValue(arguments, parameter1.Position, out var value2)
                )
                {
                    //!_! apply {paramName} placeholder
                    stepName = stepName?.Replace(match.Value, value2?.ToString() ?? "null");
                }
                else if (TrySplit(pattern, '.', out var parts) &&
                    parts.Length == 2 &&
                    parameterIndex.TryGetValue(parts[0], out var parameter2) &&
                    TryGetValue(arguments[parameter2.Position], parts[1], out var value3)
                )
                {
                    //!_! apply {paramName.fieldOrProperty} placeholder
                    stepName = stepName?.Replace(match.Value, value3);
                }
            }

            return stepName;
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
        private static bool TryGetValue(object obj, string name, out string value)
        {
            value = Unknown;
            if (obj == null) return false;

            var field = obj.GetType()?.GetField(name);
            if (field != null)
            {
                value = field.GetValue(obj)?.ToString() ?? Null;
                return true;
            }

            var prop = obj.GetType()?.GetProperty(name);
            if (prop != null)
            {
                value = prop.GetValue(obj, null)?.ToString() ?? Null;
                return true;
            }

            value = Unknown;
            return false;
        }
    }
}