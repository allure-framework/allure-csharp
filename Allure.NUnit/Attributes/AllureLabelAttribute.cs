using System;
using Allure.Net.Commons;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureLabelAttribute : AllureTestCaseAttribute
    {
        public AllureLabelAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        private string Name { get; }
        private string Value { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(new Label {name = Name, value = Value});
        }
    }
}