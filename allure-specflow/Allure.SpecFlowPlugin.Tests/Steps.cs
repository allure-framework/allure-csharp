using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace Allure.SpecFlowPlugin.Tests
{
    public enum TestOutcome { passed, failed, broken, hang }

    [Binding]
    public class TestSteps
    {
        [Given(@"Step is '(.*)'")]
        [When(@"Step is '(.*)'")]
        [Then(@"Step is '(.*)'")]
        public void StepResultIs(TestOutcome outcome)
        {
            Thread.Sleep(50);
            switch (outcome)
            {
                case TestOutcome.passed:
                    break;
                case TestOutcome.failed:
                    throw new AssertionException("This test is failed");
                case TestOutcome.broken:
                    throw new Exception("This test has error");
                case TestOutcome.hang:
                    Thread.Sleep(1000);
                    break;
                default:
                    throw new ArgumentException("value is not supported");
            }
        }

    }
}
