using System;
using Allure.Net.Commons;

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AllureLinkAttribute : AllureTestCaseAttribute
    {
        public AllureLinkAttribute(string name, string url)
        {
            Link = new Link {name = name, type = "link", url = url};
        }

        public AllureLinkAttribute(string url)
        {
            Link = new Link {name = url, type = "link", url = url};
        }

        private Link Link { get; }

        public override void UpdateTestResult(TestResult testResult)
        {
            testResult.links.Add(Link);
        }
    }
}