using System;
using Allure.Net.Commons;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureDescriptionAttribute : AllureTestCaseAttribute
    {
        public AllureDescriptionAttribute(string description, bool html = false)
        {
            TestDescription = description;
            IsHtml = html;
        }

        private string TestDescription { get; }
        private bool IsHtml { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            if (IsHtml)
            {
                testResult.descriptionHtml += TestDescription;
            }
            else
            {
                testResult.description += TestDescription;
            }
        }
    }
}