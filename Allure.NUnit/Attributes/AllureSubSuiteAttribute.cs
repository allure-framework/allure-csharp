using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureSubSuiteAttribute : AllureTestCaseAttribute
    {
        public AllureSubSuiteAttribute(string subSuite)
        {
            SubSuite = subSuite;
        }

        private string SubSuite { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(Label.SubSuite(SubSuite));
        }
    }
}