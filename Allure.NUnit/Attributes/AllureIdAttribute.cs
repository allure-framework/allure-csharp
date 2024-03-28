using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureIdAttribute : AllureTestCaseAttribute
    {
        public AllureIdAttribute(int id)
        {
            Id = id;
        }

        private int Id { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.labels.Add(new Label {name = "ALLURE_ID", value = Id.ToString()});
        }
    }
}