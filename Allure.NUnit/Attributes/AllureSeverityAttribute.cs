using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllureSeverityAttribute : AllureTestCaseAttribute
    {
        public AllureSeverityAttribute(SeverityLevel severity = SeverityLevel.normal)
        {
            Severity = severity;
        }

        private SeverityLevel Severity { get; }

        public override void UpdateTestResult(TestResult testCaseResult)
        {
            testCaseResult.labels.Add(Label.Severity(Severity));
        }
    }
}