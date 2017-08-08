using Allure.Commons;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin.Tests
{
    public enum TestOutcome { passed, failed, hang }

    [Binding]
    public class Hooks
    {
        static AllureLifecycle allure = AllureLifecycle.Instance;
        FeatureContext featureContext;
        ScenarioContext scenarioContext;
        public Hooks(FeatureContext featureContext, ScenarioContext scenarioContext)
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
                    throw new AssertionException("This test is failed");
                case TestOutcome.hang:
                    Thread.Sleep(500);
                    break;
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

        [StepDefinition("Step with params: (.*), (.*), (.*)")]
        public void StepWithArgs(int number, string text, DateTime date)
        {
        }


        [BeforeTestRun]
        public static void SetTestFolderForNUnit()
        {
            var dir = Path.GetDirectoryName(typeof(Hooks).Assembly.Location);
            Environment.CurrentDirectory = dir;
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
            if (tags != null && tags.Any(x => x.EndsWith("failed")))
                throw new Exception("Wasted");
        }

    }
}
