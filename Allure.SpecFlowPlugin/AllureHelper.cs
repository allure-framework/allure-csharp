using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Allure.SpecFlowPlugin
{
    static class AllureHelper
    {
        private static ScenarioInfo emptyScenarioInfo = new ScenarioInfo(string.Empty);

        private static FeatureInfo emptyFeatureInfo = new FeatureInfo(
            CultureInfo.CurrentCulture, string.Empty, string.Empty, new string[0]);

        internal static string GetFeatureContainerId(FeatureInfo featureInfo)
        {
            var id = (featureInfo != null) ?
                featureInfo.GetHashCode().ToString() :
                emptyFeatureInfo.GetHashCode().ToString();

            return id;
        }

        internal static string NewId() => Guid.NewGuid().ToString("N");

        internal static FixtureResult GetFixtureResult(HookBinding hook) => new FixtureResult()
        {
            name = $"{ hook.Method.Name} [{hook.HookOrder}]"
        };

        internal static TestResult StartTestCase(string containerId, FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            var featureInfo = featureContext?.FeatureInfo ?? emptyFeatureInfo;
            var scenarioInfo = scenarioContext?.ScenarioInfo ?? emptyScenarioInfo;

            var testResult = new TestResult()
            {
                uuid = NewId(),
                historyId = scenarioInfo.Title,
                name = scenarioInfo.Title,
                fullName = scenarioInfo.Title,
                labels = new List<Label>()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Suite(featureInfo.Title),
                }
                .Union(GetTags(featureInfo, scenarioInfo)).ToList()
            };

            AllureLifecycle.Instance.StartTestCase(containerId, testResult);
            scenarioContext?.Set(testResult);
            featureContext.Get<HashSet<TestResult>>().Add(testResult);

            return testResult;
        }
        internal static TestResult GetCurrentTestCase(ScenarioContext context)
        {
            context.TryGetValue( out TestResult testresult);
            return testresult;
        }

        internal static TestResultContainer StartTestContainer(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            var containerId = GetFeatureContainerId(featureContext?.FeatureInfo);

            var scenarioContainer = new TestResultContainer()
            {
                uuid = NewId()
            };
            AllureLifecycle.Instance.StartTestContainer(containerId, scenarioContainer);
            scenarioContext?.Set(scenarioContainer);
            featureContext.Get<HashSet<TestResultContainer>>().Add(scenarioContainer);

            return scenarioContainer;
        }

        internal static TestResultContainer GetCurrentTestConainer(ScenarioContext context)
        {
            context.TryGetValue(out TestResultContainer testresultContainer);
            return testresultContainer;
        }
        private static List<Label> GetTags(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            return scenarioInfo.Tags
                .Union(featureInfo.Tags)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Select(x => Label.Tag(x))
                .ToList();
        }
    }
}
