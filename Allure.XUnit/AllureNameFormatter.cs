using System;
using System.Linq;
using Allure.Net.Commons;

namespace Allure.XUnit
{
    public class AllureNameFormatter
    {
        private static readonly Func<string, TestResult, string>[] Formatters = {
            (nameTemplate, testResult) => nameTemplate.Replace(
                oldValue: "{params}",
                newValue: string.Join(", ", testResult.parameters.Select(x => $"{x.name}"))),
            
            (nameTemplate, testResult) => nameTemplate.Replace(
                oldValue: "{args}",
                newValue: string.Join(", ", testResult.parameters.Select(x => $"{x.name}: {x.value}"))),
        };
        
        public static string Format(string nameTemplate, TestResult testResult)
        {
            return Formatters.Aggregate(nameTemplate, (current, formatter) => formatter(current, testResult));
        }
    }
}