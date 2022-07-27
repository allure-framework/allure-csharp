using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;

namespace NUnit.Allure.Core.Steps
{
    public static class AllureStepParameterHelper
    {
        private const string Null = "null",
            Unknown = "Unknown";

        public static List<Parameter> CreateParameters(IEnumerable<object> arguments)
            => arguments.Select(CreateParameters).ToList();

        public static Parameter CreateParameters(object argument)
        {
            switch (argument)
            {
                case null:
                    return new Parameter {name = Unknown, value = Null};
                case ICollection collection:
                    return new Parameter
                    {
                        name = argument.GetType().Name,
                        value = string.Join(", ", collection.Cast<object>().ToList())
                    };
                default: return new Parameter {name = argument.GetType().Name, value = argument.ToString()};
            }
        }
    }
}