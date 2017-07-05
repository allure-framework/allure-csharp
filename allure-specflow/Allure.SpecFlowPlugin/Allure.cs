using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin
{
    class Allure
    {
        private static readonly object lockObj = new object();
        private static AllureLifecycle cycle;

        public static AllureLifecycle Lifecycle => cycle;

        static Allure()
        {
            lock (lockObj)
            {
                cycle = new AllureLifecycle();
            }
        }

        public static string ScenarioUuid(FeatureInfo FeatureInfo, ScenarioInfo ScenarioInfo) =>
            $"{FeatureInfo.Title}.{ScenarioInfo.Title}";
        public static string ContainerUuid(FeatureContext featureContext) =>
            $"{featureContext.FeatureInfo.Title}";

        public static string Uuid => Guid.NewGuid().ToString();

        public static TestResultContainer CreateContainer()
        {
            return new TestResultContainer()
            {
                uuid = Uuid
            };
        }
        public static TestResult GetScenario(FeatureInfo featureInfo, string name)
        {
            return GetScenario(featureInfo, new ScenarioInfo(name, new string[0]));
        }
        public static TestResult GetScenario(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            featureInfo = featureInfo ?? new FeatureInfo(CultureInfo.CurrentCulture, string.Empty, string.Empty, new string[0]);
            var testResult = new TestResult()
            {
                uuid = Allure.Uuid,
                name = scenarioInfo.Title,
                fullName = scenarioInfo.Title,
                labels = new List<Label>()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Suite(featureInfo.Title)
                }
            };

            return testResult;
        }
    }
}
