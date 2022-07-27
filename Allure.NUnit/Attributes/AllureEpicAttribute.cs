using System;
using Allure.Net.Commons;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureEpicAttribute : AllureTestCaseAttribute
    {
        public AllureEpicAttribute(string epic)
        {
            Epic = epic;
        }

        private string Epic { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(Label.Epic(Epic));
        }
    }
}