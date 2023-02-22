using System;
using Allure.Net.Commons;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureSuitesAttribute : AllureTestCaseAttribute
    {
        public AllureSuitesAttribute(params string[] suites)
        {
            Suites = suites;
            Prefix = "Suite";
        }
		
        private string[] Suites { get; }
        private string Prefix { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            for (var i = 0; i < Suites.Length; i++)
            {
                testResult.labels.Add(new Label{name = $"{Prefix}{i + 1}", value = Suites[i]});
            }
        }
    }
}