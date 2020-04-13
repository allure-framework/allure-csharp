using System;
using System.IO;
using System.Linq;
using Allure.Commons;
using Allure.SpecFlowPlugin;
using TechTalk.SpecFlow;

namespace Allure.Features
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

        private FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        public Steps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

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

        [StepDefinition("Step with attachment")]
        public void StepWithAttach()
        {
            var path = Guid.NewGuid().ToString();
            File.WriteAllText(path, "hi there");
            allure.AddAttachment(path);
        }

        [StepDefinition("Step with table")]
        public void StepWithTable(Table table)
        {
        }

        [StepDefinition("Step with params: (.*)")]
        public void StepWithArgs(int number, string text)
        {
        }

        [BeforeFeature("beforefeaturepassed", Order = 1)]
        public static void PassedBeforeFeature()
        {
        }

        [AfterFeature("afterfeaturepassed", Order = 1)]
        public static void PassedAfterFeature()
        {
        }

        [BeforeFeature("beforefeaturefailed")]
        public static void FailedBeforeFeature()
        {
            throw new Exception("Failed Before Feature");
        }

        [AfterFeature("afterfeaturefailed")]
        public static void FailedAfterFeature()
        {
            throw new Exception("Failed After Feature");
        }

        [BeforeScenario(tags: "beforescenario")]
        [AfterScenario(tags: "afterscenario")]
        [BeforeStep(tags: "beforestep")]
        [AfterStep(tags: "afterstep")]
        public void HandleIt()
        {
            Handle(scenarioContext.ScenarioInfo.Tags);
        }

        private static void Handle(string[] tags)
        {
            if (tags != null && tags.Contains("attachment"))
            {
                var path = $"{Guid.NewGuid().ToString()}.txt";
                File.WriteAllText(path, "hi there");
                allure.AddAttachment(path);
                allure.AddAttachment(path, "text file");
            }

            if (tags != null)
                if (tags.Any(x => x.EndsWith(Status.failed.ToString()) || x.EndsWith(Status.broken.ToString())))
                    throw new Exception("Wasted");
                else if (tags.Any(x => x == PluginHelper.IGNORE_EXCEPTION))
                    throw new IgnoreException();
        }
    }
}