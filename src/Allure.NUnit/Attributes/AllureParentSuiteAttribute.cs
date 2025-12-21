using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureParentSuiteAttribute : AllureTestCaseAttribute
    {
        public AllureParentSuiteAttribute(string parentSuite)
        {
            ParentSuite = parentSuite;
        }

        private string ParentSuite { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(Label.ParentSuite(ParentSuite));
        }
    }
}