using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;

using HeyRed.Mime;

namespace Allure.SpecFlowPlugin
{
    public class Allure
    {
        private AllureLifecycle cycle;

        public Allure Attach(string path, string name = null)
        {
            name = name ?? Path.GetFileName(path);
            var type = MimeTypesMap.GetMimeType(path);
            cycle.AddAttachment(name, type, path);
            return this;
        }
        internal AllureLifecycle Lifecycle =>
            cycle = cycle ?? new AllureLifecycle();
        internal static TestResultContainer CreateScenarioContainer(ScenarioInfo scenarioInfo) =>
            new TestResultContainer()
            {
                uuid = $"_{scenarioInfo.GetHashCode().ToString()}"
            };

        internal static string GetStepId(ScenarioContext context) =>
            context.StepContext.StepInfo.GetHashCode().ToString();

        internal static TestResultContainer CreateContainer() =>
            new TestResultContainer()
            {
                uuid = Guid.NewGuid().ToString()
            };

        internal static TestResult CreateTestResult(FeatureInfo featureInfo, string name)
        {
            return CreateTestResult(featureInfo, new ScenarioInfo(name, new string[0]));
        }
        internal static TestResult CreateTestResult(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            featureInfo = featureInfo ?? new FeatureInfo(CultureInfo.CurrentCulture, string.Empty, string.Empty, new string[0]);
            var testResult = new TestResult()
            {
                uuid = scenarioInfo.GetHashCode().ToString(),
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
