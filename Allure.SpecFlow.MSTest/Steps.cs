using System;
using Allure.Commons;
using TechTalk.SpecFlow;

namespace Allure.SpecFlow.MSTest
{
    public enum TestOutcome
    {
        passed,
        failed
    }

    [Binding]
    public class Steps
    {
        private static readonly AllureLifecycle allure = AllureLifecycle.Instance;


        [StepDefinition(@"Step is '(.*)'")]
        public void StepResultIs(TestOutcome outcome)
        {
            switch (outcome)
            {
                case TestOutcome.passed:
                    break;
                case TestOutcome.failed:
                    throw new Exception("This test is failed",
                        new InvalidOperationException("Internal message",
                            new ArgumentException("One more message")));
                default:
                    throw new ArgumentException("value is not supported");
            }
        }


        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AllureLifecycle.Instance.CleanupResultDirectory();
        }
    }
}