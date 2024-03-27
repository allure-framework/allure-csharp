using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureSuiteAttribute : AllureTestCaseAttribute
    {
        public AllureSuiteAttribute(string suite)
        {
            Suite = suite;
        }

        private string Suite { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(Label.Suite(Suite));
        }
    }
}