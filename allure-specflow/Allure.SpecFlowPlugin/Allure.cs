using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;

using HeyRed.Mime;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Bindings;

namespace Allure.SpecFlowPlugin
{
    public static class Allure
    {
        public static void Attach(string path, string name = null)
        {
            name = name ?? Path.GetFileName(path);
            var type = MimeTypesMap.GetMimeType(path);
            AllureLifecycle.Instance.AddAttachment(name, type, path);
        }

        internal static string FeatureContainerId(FeatureContext context) => context?.FeatureInfo.GetHashCode().ToString();
        internal static string ScenarioContainerId(ScenarioContext context) => context?.ScenarioInfo.GetHashCode().ToString();
        internal static string ScenarioId(ScenarioContext context) => $"{ScenarioContainerId(context)}_";
        internal static string NewId() => Guid.NewGuid().ToString();
        internal static FixtureResult GetFixtureResult(HookBinding hook) => new FixtureResult()
        {
            name = $"{ hook.Method.Name} [order = {hook.HookOrder}]"
        };
        internal static TestResult GetTestResult(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            var featureInfo = featureContext?.FeatureInfo ?? new FeatureInfo(CultureInfo.CurrentCulture, string.Empty, string.Empty, new string[0]);
            var testResult = new TestResult()
            {
                uuid = ScenarioId(scenarioContext),
                historyId = scenarioContext.ScenarioInfo.Title,
                name = scenarioContext.ScenarioInfo.Title,
                fullName = scenarioContext.ScenarioInfo.Title,
                labels = new List<Label>()
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Suite(featureInfo.Title),
                }
                .Union(GetTags(featureInfo, scenarioContext.ScenarioInfo)).ToList()
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
