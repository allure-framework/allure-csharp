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

            return $"__{id}";
        }

        internal static string GetScenarioId(FeatureInfo featureInfo, ScenarioInfo scenarioInfo) => NewId();
            //(scenarioInfo != null) ?
            //    scenarioInfo.GetHashCode().ToString() :
            //    GetFeatureContainerId(featureInfo);

        internal static string GetScenarioContainerId(FeatureInfo featureInfo, ScenarioInfo scenarioInfo) => $"_{NewId()}";
            //$"{GetScenarioId(featureInfo, scenarioInfo)}_";

        internal static string NewId() => Guid.NewGuid().ToString("N");

        internal static FixtureResult GetFixtureResult(HookBinding hook) => new FixtureResult()
        {
            name = $"{ hook.Method.Name} [{hook.HookOrder}]"
        };

        internal static TestResult GetTestResult(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            featureInfo = featureInfo ?? emptyFeatureInfo;
            scenarioInfo = scenarioInfo ?? emptyScenarioInfo;

            var testResult = new TestResult()
            {
                uuid = GetScenarioId(featureInfo, scenarioInfo),
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

            return testResult;
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
