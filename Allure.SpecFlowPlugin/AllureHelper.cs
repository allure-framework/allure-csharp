using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Allure.SpecFlowPlugin
{
    static class AllureHelper
    {
        static ScenarioInfo emptyScenarioInfo = new ScenarioInfo(string.Empty);

        static FeatureInfo emptyFeatureInfo = new FeatureInfo(
            CultureInfo.CurrentCulture, string.Empty, string.Empty);

        static PluginConfiguration config = new PluginConfiguration(AllureLifecycle.Instance.Configuration);

        internal static string GetFeatureContainerId(FeatureInfo featureInfo)
        {
            var id = (featureInfo != null)
                ? featureInfo.GetHashCode().ToString()
                : emptyFeatureInfo.GetHashCode().ToString();

            return id;
        }

        internal static string NewId() => Guid.NewGuid().ToString("N");

        internal static FixtureResult GetFixtureResult(HookBinding hook) => new FixtureResult
        {
            name = $"{hook.Method.Name} [{hook.HookOrder}]"
        };

        internal static TestResult StartTestCase(string containerId, FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            var featureInfo = featureContext?.FeatureInfo ?? emptyFeatureInfo;
            var scenarioInfo = scenarioContext?.ScenarioInfo ?? emptyScenarioInfo;
            var tags = GetTags(featureInfo, scenarioInfo);
            var testResult = new TestResult
            {
                uuid = NewId(),
                historyId = scenarioInfo.Title,
                name = scenarioInfo.Title,
                fullName = scenarioInfo.Title,
                labels = new List<Label>
                    {
                        Label.Thread(),
                        Label.Host(),
                        Label.Feature(featureInfo.Title)
                    }
                    .Union(tags)
                    .Union(GetCustomLabelsFromTags(tags)).ToList(),
                links = GetLinksFromTags(tags)
            };

            AllureLifecycle.Instance.StartTestCase(containerId, testResult);
            scenarioContext?.Set(testResult);
            featureContext?.Get<HashSet<TestResult>>().Add(testResult);

            return testResult;
        }

        internal static TestResult GetCurrentTestCase(ScenarioContext context)
        {
            context.TryGetValue(out TestResult testresult);
            return testresult;
        }

        internal static TestResultContainer StartTestContainer(FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            var containerId = GetFeatureContainerId(featureContext?.FeatureInfo);

            var scenarioContainer = new TestResultContainer
            {
                uuid = NewId()
            };
            AllureLifecycle.Instance.StartTestContainer(containerId, scenarioContainer);
            scenarioContext?.Set(scenarioContainer);
            featureContext?.Get<HashSet<TestResultContainer>>().Add(scenarioContainer);

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
                .Select(Label.Tag)
                .ToList();
        }

        private static List<Link> GetLinksFromTags(List<Label> tags)
        {
            var links = new List<Link>();
            foreach (var tag in tags.Select(x => x.value))
            {
                if (TryUpdateValueByMatch(config.IssueRegex, tag))
                    links.Add(Link.Issue(tag));

                if (TryUpdateValueByMatch(config.TmsRegex, tag))
                    links.Add(Link.Tms(tag));
            }
            return links;

        }
        private static List<Label> GetCustomLabelsFromTags(List<Label> tags)
        {
            var labels = new List<Label>();
            var label = new Label();
            foreach (var tag in tags.Select(x => x.value))
            {
                if (TryUpdateValueByMatch(config.ParentSuiteRegex, tag))
                    labels.Add(Label.ParentSuite(tag));
                if (TryUpdateValueByMatch(config.SuiteRegex, tag))
                    labels.Add(Label.Suite(tag));
                if (TryUpdateValueByMatch(config.SubSuiteRegex, tag))
                    labels.Add(Label.SubSuite(tag));

                if (TryUpdateValueByMatch(config.EpicRegex, tag))
                    labels.Add(Label.Epic(tag));
                if (TryUpdateValueByMatch(config.StoryRegex, tag))
                    labels.Add(Label.Story(tag));

                if (TryUpdateValueByMatch(config.PackageRegex, tag))
                    labels.Add(Label.Package(tag));
                if (TryUpdateValueByMatch(config.TestClassRegex, tag))
                    labels.Add(Label.TestClass(tag));
                if (TryUpdateValueByMatch(config.TestMethodRegex, tag))
                    labels.Add(Label.TestMethod(tag));

                if (TryUpdateValueByMatch(config.OwnerRegex, tag))
                    labels.Add(Label.Owner(tag));
                if (TryUpdateValueByMatch(config.SeverityRegex, tag) && Enum.TryParse(tag, out SeverityLevel level))
                    labels.Add(Label.Severity(level));
            }

            return labels;
        }

        private static bool TryUpdateValueByMatch(Regex regex, string value)
        {
            if (regex == null)
                return false;

            if (regex.IsMatch(value))
            {
                var groups = regex.Match(value).Groups;
                if (groups?.Count == 1)
                    value = groups[0].Value;
                else
                    value = groups[1].Value;

                return true;
            }
            else
                return false;
        }
    }
}