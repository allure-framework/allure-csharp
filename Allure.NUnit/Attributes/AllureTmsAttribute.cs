using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureTmsAttribute : AllureTestCaseAttribute
    {
        public AllureTmsAttribute(string name, string url)
        {
            TmsLink = new Link {name = name, type = "tms", url = url};
        }

        public AllureTmsAttribute(string name)
        {
            TmsLink = new Link {name = name, type = "tms", url = name};
        }

        private Link TmsLink { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.links.Add(TmsLink);
        }
    }
}